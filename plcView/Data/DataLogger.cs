using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using plcView.Config;

namespace plcView.Data
{
    public class DataLogger : IDisposable
    {
        private readonly ProjectSettings _settings;
        private string _currentFilePath = "";
        private readonly object _fileLock = new object();
        private const long MaxFileSize = 100 * 1024 * 1024; // 100MB
        private int _fileSequence = 1;

        // ストリーム保持用フィールド
        private FileStream _activeFileStream = null;
        private BinaryWriter _activeBinaryWriter = null;
        private StreamWriter _activeStreamWriter = null;

        // 差分保存用の前回データキャッシュ
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
        public void WriteData(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            if (string.IsNullOrEmpty(_settings.OutputFolderPath)) return;

            // 1. 差分判定
            bool hasChange = CheckChangeAndCache(data);
            bool forceHeartbeat = (timestamp - _lastWriteTime) >= _heartbeatInterval;

            if (!hasChange && !forceHeartbeat && _lastWriteTime != DateTime.MinValue)
            {
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

        private void CloseActiveStream()
        {
            if (_activeBinaryWriter != null)
            {
                try { _activeBinaryWriter.Flush(); } catch { }
                try { _activeBinaryWriter.Close(); } catch { }
                _activeBinaryWriter = null;
            }
            if (_activeStreamWriter != null)
            {
                try { _activeStreamWriter.Flush(); } catch { }
                try { _activeStreamWriter.Close(); } catch { }
                _activeStreamWriter = null;
            }
            if (_activeFileStream != null)
            {
                try { _activeFileStream.Flush(); } catch { }
                try { _activeFileStream.Close(); } catch { }
                _activeFileStream = null;
            }
        }

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
                CloseActiveStream();

                string ext = _settings.IsCsvMode ? "csv" : "dat";
                string timeStr = timestamp.ToString("yyyyMMdd_HHmmss");
                string projName = string.IsNullOrEmpty(_settings.ProjectName) ? "Project" : _settings.ProjectName;
                
                string fileName = $"{timeStr}_{projName}_{_fileSequence:D3}.{ext}";
                _currentFilePath = Path.Combine(_settings.OutputFolderPath, fileName);

                bool isNewFile = !File.Exists(_currentFilePath);

                // ストリームの新規オープン
                _activeFileStream = new FileStream(_currentFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                if (_settings.IsCsvMode)
                {
                    _activeStreamWriter = new StreamWriter(_activeFileStream, Encoding.UTF8);
                    if (isNewFile)
                    {
                        WriteHeader();
                    }
                }
                else
                {
                    _activeBinaryWriter = new BinaryWriter(_activeFileStream);
                }
            }
            else if (_activeFileStream == null)
            {
                // 既存ファイルがあり、かつストリームがまだ開かれていない場合
                bool isNewFile = !File.Exists(_currentFilePath);
                _activeFileStream = new FileStream(_currentFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                if (_settings.IsCsvMode)
                {
                    _activeStreamWriter = new StreamWriter(_activeFileStream, Encoding.UTF8);
                    if (isNewFile)
                    {
                        WriteHeader();
                    }
                }
                else
                {
                    _activeBinaryWriter = new BinaryWriter(_activeFileStream);
                }
            }
        }

        private void WriteHeader()
        {
            if (_activeStreamWriter == null) return;

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
            _activeStreamWriter.WriteLine(sb.ToString());
            _activeStreamWriter.Flush();
        }

        private void WriteCsvLine(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            if (_activeStreamWriter == null) return;

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
                    for (int i = 0; i < point.Size; i++)
                    {
                        sb.Append(",");
                    }
                }
            }

            _activeStreamWriter.WriteLine(sb.ToString());
            _activeStreamWriter.Flush();
        }

        private void WriteBinaryBlock(DateTime timestamp, Dictionary<int, ushort[]> data)
        {
            if (_activeBinaryWriter == null) return;

            // バイナリフォーマット設計 (改善版):
            // [8バイト: DateTime(Ticks)]
            // [2バイト: 有効ポイント数 (activePointCount)]  <- 指摘による境界破損バグ防止のため追加
            // 続く各有効ポイント: 
            //   [2バイト: ポイントNo] 
            //   [2バイト: データサイズ(ワード数)] 
            //   [N*2バイト: ushortデータ]
            
            _activeBinaryWriter.Write(timestamp.Ticks);

            // 有効なポイント数をカウントして書き込む
            short activePointCount = 0;
            foreach (var point in _settings.Points)
            {
                if (point.Enabled && data.ContainsKey(point.No))
                {
                    activePointCount++;
                }
            }
            _activeBinaryWriter.Write(activePointCount);

            foreach (var point in _settings.Points)
            {
                if (!point.Enabled) continue;
                if (data.TryGetValue(point.No, out ushort[] values))
                {
                    _activeBinaryWriter.Write((short)point.No);
                    _activeBinaryWriter.Write((short)values.Length);
                    foreach (var val in values)
                    {
                        _activeBinaryWriter.Write(val);
                    }
                }
            }
            _activeBinaryWriter.Flush();
        }

        public void Dispose()
        {
            lock (_fileLock)
            {
                CloseActiveStream();
            }
        }
    }
}
