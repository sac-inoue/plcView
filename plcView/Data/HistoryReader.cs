using System;
using System.Collections.Generic;
using System.IO;

namespace plcView.Data
{
    public class HistoryFrame
    {
        public DateTime Timestamp { get; set; }
        // キー: ポイントNo、値: ushortデータ配列
        public Dictionary<int, ushort[]> Data { get; set; }

        public HistoryFrame()
        {
            Data = new Dictionary<int, ushort[]>();
        }
    }

    public class HistoryReader
    {
        private readonly string _filePath;
        private readonly List<long> _blockOffsets = new List<long>();
        private readonly List<DateTime> _timestamps = new List<DateTime>();

        public int FrameCount => _timestamps.Count;
        public List<DateTime> Timestamps => _timestamps;

        public HistoryReader(string filePath)
        {
            _filePath = filePath;
            if (File.Exists(_filePath))
            {
                IndexFile();
            }
        }

        /// <summary>
        /// バイナリファイルをスキャンし、各データフレームのファイルオフセットと時刻をインデックス化します。
        /// </summary>
        private void IndexFile()
        {
            _blockOffsets.Clear();
            _timestamps.Clear();

            using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
            {
                long length = fs.Length;
                while (fs.Position < length)
                {
                    long offset = fs.Position;
                    
                    try
                    {
                        // 1. 時刻の読み込み (8バイト)
                        long ticks = br.ReadInt64();
                        var time = new DateTime(ticks);

                        _blockOffsets.Add(offset);
                        _timestamps.Add(time);

                        // 2. 各ポイントデータのスキップ
                        // 保存されたポイント数（2バイト）を読み込み、その個数分だけ確実にループしてシークする
                        if (fs.Position + 2 > length)
                        {
                            break;
                        }
                        short pointCount = br.ReadInt16();

                        for (int p = 0; p < pointCount; p++)
                        {
                            if (fs.Position + 4 > length)
                            {
                                break;
                            }
                            short pointNo = br.ReadInt16();
                            short valLength = br.ReadInt16();
                            fs.Seek(valLength * 2, SeekOrigin.Current);
                        }
                    }
                    catch
                    {
                        // 途中でパースエラーが起きた場合はそこまでのインデックスで切り上げる
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 指定インデックス（フレーム番号）のデータをファイルから直接シークして取得します。
        /// </summary>
        public HistoryFrame ReadFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= _blockOffsets.Count)
            {
                return null;
            }

            long offset = _blockOffsets[frameIndex];
            var frame = new HistoryFrame();

            using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var br = new BinaryReader(fs))
            {
                fs.Position = offset;
                
                // 時刻読み込み
                long ticks = br.ReadInt64();
                frame.Timestamp = new DateTime(ticks);

                // フレームサイズ境界チェック用の限界オフセットを計算
                long nextFrameOffset = (frameIndex + 1 < _blockOffsets.Count) ? _blockOffsets[frameIndex + 1] : fs.Length;

                // 保存されたポイント数を読み込む
                if (fs.Position + 2 > nextFrameOffset)
                {
                    return frame;
                }
                short pointCount = br.ReadInt16();

                for (int p = 0; p < pointCount; p++)
                {
                    if (fs.Position + 4 > nextFrameOffset)
                    {
                        break;
                    }

                    short pointNo = br.ReadInt16();
                    short valLength = br.ReadInt16();

                    if (pointNo >= 1 && pointNo <= 10 && valLength >= 0 && valLength <= 960)
                    {
                        if (fs.Position + valLength * 2 > nextFrameOffset)
                        {
                            break;
                        }

                        ushort[] values = new ushort[valLength];
                        for (int i = 0; i < valLength; i++)
                        {
                            values[i] = br.ReadUInt16();
                        }
                        frame.Data[pointNo] = values;
                    }
                    else
                    {
                        // 異常値の場合はフレーム終了
                        break;
                    }
                }
            }

            return frame;
        }
    }
}
