using System;
using System.Globalization;

namespace plcView.Plc
{
    public static class DeviceConverter
    {
        /// <summary>
        /// デバイス種別とアドレス文字列から、MCプロトコル用のデバイスコードと開始アドレス（数値）を取得します。
        /// </summary>
        public static bool TryParse(string deviceType, string addressStr, out byte deviceCode, out int parsedAddress)
        {
            deviceCode = 0;
            parsedAddress = 0;

            if (string.IsNullOrEmpty(deviceType) || string.IsNullOrEmpty(addressStr))
                return false;

            deviceType = deviceType.Trim().ToUpper();
            addressStr = addressStr.Trim().ToUpper();

            // 1. デバイスコードのマッピング
            switch (deviceType)
            {
                case "D":  deviceCode = 0xA8; break; // データレジスタ
                case "W":  deviceCode = 0xB4; break; // リンクレジスタ
                case "R":  deviceCode = 0xAF; break; // ファイルレジスタ
                case "ZR": deviceCode = 0xB0; break; // ファイルレジスタ (連番)
                case "SD": deviceCode = 0xA9; break; // 特殊レジスタ
                case "SW": deviceCode = 0xB5; break; // リンク特殊レジスタ
                case "TN": deviceCode = 0xC2; break; // タイマ現在値
                case "CN": deviceCode = 0xC5; break; // カウンタ現在値
                case "M":  deviceCode = 0x90; break; // 内部リレー
                case "X":  deviceCode = 0x9C; break; // 入力
                case "Y":  deviceCode = 0x9D; break; // 出力
                case "L":  deviceCode = 0x92; break; // ラッチリレー
                case "F":  deviceCode = 0x93; break; // アラーム
                case "B":  deviceCode = 0xA0; break; // リンクリレー
                case "SB": deviceCode = 0xA1; break; // リンク特殊リレー
                default:
                    return false; // 未対応デバイス
            }

            // 2. アドレスの進数判定とパース
            bool isHex = IsHexDevice(deviceType);

            try
            {
                if (isHex)
                {
                    // "0X" で始まっている場合は除去
                    if (addressStr.StartsWith("0X"))
                    {
                        addressStr = addressStr.Substring(2);
                    }
                    parsedAddress = int.Parse(addressStr, NumberStyles.HexNumber);
                }
                else
                {
                    parsedAddress = int.Parse(addressStr, CultureInfo.InvariantCulture);
                }
                return true;
            }
            catch
            {
                // パース失敗時、ZRなどハイブリッドな進数デバイスに対するフォールバック
                if (deviceType == "ZR")
                {
                    // 16進数で失敗した場合は10進数で試す
                    if (int.TryParse(addressStr, out parsedAddress))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// デバイス種別が16進数アドレス表記であるかを判定します。
        /// </summary>
        public static bool IsHexDevice(string deviceType)
        {
            if (string.IsNullOrEmpty(deviceType)) return false;
            deviceType = deviceType.Trim().ToUpper();

            switch (deviceType)
            {
                case "W":  // リンクレジスタ
                case "B":  // リンクリレー
                case "X":  // 入力
                case "Y":  // 出力
                case "SB": // リンク特殊リレー
                case "SW": // リンク特殊レジスタ
                case "ZR": // ファイルレジスタ連番 (通常16進表記されるケースが多い)
                    return true;
                default:
                    return false;
            }
        }
    }
}
