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
                        // ファイル末尾に達するまで、または次のブロックの時刻を読む手前までループ
                        // ※有効ポイント数が可変なため、格納されているポイントNoを読んでシーク位置を進める
                        while (fs.Position < length)
                        {
                            // Peek用：次の8バイトがDateTimeのTick値かどうかを判定するための処理
                            // ただし今回はシンプルに「ポイントデータブロック」の終わりまで読み飛ばす
                            if (fs.Position + 4 > length)
                            {
                                fs.Position = length; // 破損データ対策で末尾へ
                                break;
                            }

                            // 次のデータブロックのヘッダを読み込み
                            long curPos = fs.Position;
                            short pointNo = br.ReadInt16();
                            short valLength = br.ReadInt16();

                            // 合理的な値のチェック (破損判定用。10ポイント以内、サイズ960点以内)
                            if (pointNo >= 1 && pointNo <= 10 && valLength >= 0 && valLength <= 960)
                            {
                                // 有効なポイントデータなので中身（valLength * 2バイト）をシーク
                                fs.Seek(valLength * 2, SeekOrigin.Current);
                            }
                            else
                            {
                                // ポイントデータではない（＝次のフレームのDateTime Ticksの開始）可能性があるため、
                                // 読み戻してインナーループを抜け、次のフレームの読み込みへ回す
                                fs.Position = curPos;
                                break;
                            }
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

                while (fs.Position < nextFrameOffset)
                {
                    if (fs.Position + 4 > nextFrameOffset)
                    {
                        break;
                    }

                    short pointNo = br.ReadInt16();
                    short valLength = br.ReadInt16();

                    if (pointNo >= 1 && pointNo <= 10 && valLength >= 0 && valLength <= 960)
                    {
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
