using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using plcView.Config;

namespace plcView.Data
{
    public class DataLogger
    {
        private readonly ProjectSettings _settings;
        private string _currentFilePath = "";
        private readonly object _fileLock = new object();
        private const long MaxFileSize = 100 * 1024 * 1024; // 100MB
        private int _fileSequence = 1;

        // 差分保存用の前回データキャッシュ
        // キー: ポイントのNo、値: 読み出したワードデータの配列(ushort[])
        private readonly Dictionary<int, ushort[]> _lastDataCache = new Dictionary<int, ushort[]>();
        private DateTime _lastWriteTime = DateTime.MinValue;
        private readonly TimeSpan _heartbeatInterval = TimeSpan.FromMinutes(10); // 10分ハートビート

        public DataLogger(ProjectSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// 1回分の収集データを保存します。差分判定や容量分割を自動で行います。
        /// </summary>
        /// <param name="timestamp">収集時刻</param>
        /// <param name="data">Noごとの読出しデータ（ushort配列）</param>
        public void WriteData(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            if (string.IsNullOrEmpty(_settings.OutputFolderPath)) return;

            // 1. 差分判定
            bool hasChange = CheckChangeAndCache(data);
            bool forceHeartbeat = (timestamp - _lastWriteTime) >= _heartbeatInterval;

            if (!hasChange && !forceHeartbeat && _lastWriteTime != DateTime.MinValue)
            {
                // 変化がなく、かつハートビート周期にも達していない場合は保存をスキップ
                return;
            }

            lock (_fileLock)
            {
                // 2. 保存ファイルの決定・分割チェック
                CheckAndPrepareFile(timestamp);

                // 3. データ書き込み
                if (_settings.IsCsvMode)
                {
                    WriteCsvLine(timestamp, data);
                }
                else
                {
                    WriteBinaryBlock(timestamp, data);
                }

                _lastWriteTime = timestamp;
            }
        }

        private bool IsBinaryMode()
        {
            return !_settings.IsCsvMode;
        }

        /// <summary>
        /// 各ポイントデータが前回から変化したかチェックし、キャッシュを更新します。
        /// </summary>
        private bool CheckChangeAndCache(Dictionary<int, ushort[]> currentData)
        {
            bool anyChange = false;

            foreach (var point in _settings.Points)
            {
                if (!point.Enabled) continue;

                if (!currentData.TryGetValue(point.No, out ushort[] currentVal))
                {
                    continue;
                }

                if (!_lastDataCache.TryGetValue(point.No, out ushort[] lastVal))
                {
                    // 初回は「変化あり」とみなしてキャッシュ登録
                    _lastDataCache[point.No] = (ushort[])currentVal.Clone();
                    anyChange = true;
                    continue;
                }

                if (lastVal.Length != currentVal.Length)
                {
                    _lastDataCache[point.No] = (ushort[])currentVal.Clone();
                    anyChange = true;
                    continue;
                }

                // 差分（デッドバンド）チェック
                // デッドバンドは現在簡易的に±0(完全一致)としているが、必要に応じてpointごとにしきい値を設定可能
                bool pointChanged = false;
                for (int i = 0; i < currentVal.Length; i++)
                {
                    if (currentVal[i] != lastVal[i])
                    {
                        pointChanged = true;
                        break;
                    }
                }

                if (pointChanged)
                {
                    _lastDataCache[point.No] = (ushort[])currentVal.Clone();
                    anyChange = true;
                }
            }

            return anyChange;
        }

        /// <summary>
        /// ファイルサイズが100MBを超えていないか確認し、超えていれば新規ファイルを作成します。
        /// </summary>
        private void CheckAndPrepareFile(DateTime timestamp)
        {
            if (!Directory.Exists(_settings.OutputFolderPath))
            {
                Directory.CreateDirectory(_settings.OutputFolderPath);
            }

            bool needsNewFile = false;

            if (string.IsNullOrEmpty(_currentFilePath))
            {
                needsNewFile = true;
            }
            else if (File.Exists(_currentFilePath))
            {
                var fileInfo = new FileInfo(_currentFilePath);
                if (fileInfo.Length >= MaxFileSize)
                {
                    needsNewFile = true;
                    _fileSequence++;
                }
            }

            if (needsNewFile)
            {
                string ext = _settings.IsCsvMode ? "csv" : "dat";
                string timeStr = timestamp.ToString("yyyyMMdd_HHmmss");
                string projName = string.IsNullOrEmpty(_settings.ProjectName) ? "Project" : _settings.ProjectName;
                
                string fileName = $"{timeStr}_{projName}_{_fileSequence:D3}.{ext}";
                _currentFilePath = Path.Combine(_settings.OutputFolderPath, fileName);

                // CSVの場合は初回ヘッダーを書き込み
                if (_settings.IsCsvMode && !File.Exists(_currentFilePath))
                {
                    WriteHeader();
                }
            }
        }

        private void WriteHeader()
        {
            var sb = new StringBuilder();
            sb.Append("Timestamp");
            foreach (var point in _settings.Points)
            {
                if (!point.Enabled) continue;
                for (int i = 0; i < point.Size; i++)
                {
                    sb.Append($",Pt{point.No}_{point.DeviceType}{point.StartAddress}+{i}");
                }
            }
            File.WriteAllText(_currentFilePath, sb.ToString() + Environment.NewLine, Encoding.UTF8);
        }

        private void WriteCsvLine(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            var sb = new StringBuilder();
            sb.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            foreach (var point in _settings.Points)
            {
                if (!point.Enabled) continue;

                if (data.TryGetValue(point.No, out ushort[] values))
                {
                    foreach (var val in values)
                    {
                        sb.Append($",{val}");
                    }
                }
                else
                {
                    // データが取得できなかったポイントは空埋め
                    for (int i = 0; i < point.Size; i++)
                    {
                        sb.Append(",");
                    }
                }
            }

            File.AppendAllText(_currentFilePath, sb.ToString() + Environment.NewLine, Encoding.UTF8);
        }

        private void WriteBinaryBlock(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            // バイナリフォーマット設計:
            // [8バイト: DateTime(Ticks)] 
            // 続く各有効ポイント: 
            //   [2バイト: ポイントNo] 
            //   [2バイト: データサイズ(ワード数)] 
            //   [N*2バイト: ushortデータ]
            using (var fs = new FileStream(_currentFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(timestamp.Ticks);
                
                foreach (var point in _settings.Points)
                {
                    if (!point.Enabled) continue;
                    if (data.TryGetValue(point.No, out ushort[] values))
                    {
                        bw.Write((short)point.No);
                        bw.Write((short)values.Length);
                        foreach (var val in values)
                        {
                            bw.Write(val);
                        }
                    }
                }
            }
        }
    }
}
