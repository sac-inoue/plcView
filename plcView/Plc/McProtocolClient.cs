using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace plcView.Plc
{
    public class McProtocolClient : IDisposable
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly int _timeoutMs;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly object _lockObject = new object();
        private bool _isDisposed = false;

        // デバッグログ出力用のイベント
        public event Action<string> OnLogMessage;

        public McProtocolClient(string ipAddress, int port, int timeoutMs)
        {
            _ipAddress = ipAddress;
            _port = port;
            _timeoutMs = timeoutMs;
        }

        /// <summary>
        /// PLCに接続します。接続エラー時は自動で再試行します。
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Log($"Connecting to PLC at {_ipAddress}:{_port}...");
                    _tcpClient = new TcpClient();
                    
                    // タイムアウト設定
                    _tcpClient.SendTimeout = _timeoutMs;
                    _tcpClient.ReceiveTimeout = _timeoutMs;

                    // 接続試行（非同期）
                    var connectTask = _tcpClient.ConnectAsync(_ipAddress, _port);
                    
                    // タイムアウト付きで待機
                    if (await Task.WhenAny(connectTask, Task.Delay(_timeoutMs, cancellationToken)) == connectTask)
                    {
                        await connectTask; // 例外が発生していればここでスローされる
                        _stream = _tcpClient.GetStream();
                        Log("Successfully connected to PLC.");
                        return; // 接続成功
                    }
                    else
                    {
                        throw new TimeoutException("PLC connection attempt timed out.");
                    }
                }
                catch (Exception ex)
                {
                    Log($"Connection failed: {ex.Message}. Retrying in 5 seconds...");
                    CloseConnection();
                    
                    // 5秒待機して再試行
                    try
                    {
                        await Task.Delay(5000, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// ワードデバイスを一括読出しします。
        /// </summary>
        public byte[] ReadWords(string deviceType, string addressStr, int size)
        {
            if (size <= 0 || size > 960)
            {
                throw new ArgumentException("Size must be between 1 and 960 words.");
            }

            if (!DeviceConverter.TryParse(deviceType, addressStr, out byte deviceCode, out int parsedAddress))
            {
                throw new ArgumentException($"Invalid device or address: {deviceType}{addressStr}");
            }

            lock (_lockObject)
            {
                if (_tcpClient == null || !_tcpClient.Connected || _stream == null)
                {
                    throw new InvalidOperationException("PLC is not connected.");
                }

                // 3Eフレーム 要求パケットの組み立て
                byte[] requestPacket = BuildReadRequest(deviceCode, parsedAddress, size);

                // 送信
                LogHex("TX", requestPacket);
                _stream.Write(requestPacket, 0, requestPacket.Length);

                // レスポンスヘッダ読み込み (11バイト)
                byte[] responseHeader = new byte[11];
                ReadExact(_stream, responseHeader);
                LogHex("RX Header", responseHeader);

                // 終了コードの確認 (9, 10バイト目)
                ushort endCode = BitConverter.ToUInt16(responseHeader, 9);
                if (endCode != 0)
                {
                    throw new IOException($"PLC returned error code: 0x{endCode:X4}");
                }

                // 応答データ長 (7, 8バイト目) から終了コードの2バイトを引いた残りがデータ部
                ushort dataLength = BitConverter.ToUInt16(responseHeader, 7);
                int expectedDataBytes = dataLength - 2;

                if (expectedDataBytes != size * 2)
                {
                    throw new IOException($"Unexpected response data size. Expected: {size * 2} bytes, got: {expectedDataBytes} bytes");
                }

                // データ部読み込み
                byte[] responseData = new byte[expectedDataBytes];
                ReadExact(_stream, responseData);
                LogHex("RX Data", responseData);

                return responseData;
            }
        }

        private byte[] BuildReadRequest(byte deviceCode, int address, int size)
        {
            byte[] packet = new byte[21];

            // 1. サブヘッダ: 3Eフレーム実行 (0x0050) -> リトルエンディアンで 0x50, 0x00
            packet[0] = 0x50;
            packet[1] = 0x00;

            // 2. ネットワーク番号: 0x00
            packet[2] = 0x00;

            // 3. PC番号: 0xFF
            packet[3] = 0xFF;

            // 4. 要求先ユニットI/O番号: 0x03FF -> リトルエンディアンで 0xFF, 0x03
            packet[4] = 0xFF;
            packet[5] = 0x03;

            // 5. 要求先局番号: 0x00
            packet[6] = 0x00;

            // 6. 要求データ長: 監視タイマ(2) + コマンド(2) + サブコマンド(2) + 先頭デバイス(3) + デバイスコード(1) + 点数(2) = 12バイト -> 0x000C -> 0x0C, 0x00
            packet[7] = 0x0C;
            packet[8] = 0x00;

            // 7. CPU監視タイマ: 2.5秒 (0x000A -> 10 * 250ms) -> 0x0A, 0x00
            packet[9] = 0x0A;
            packet[10] = 0x00;

            // 8. コマンド: 一括読出し (0x0401) -> リトルエンディアン 0x01, 0x04
            packet[11] = 0x01;
            packet[12] = 0x04;

            // 9. サブコマンド: ワード単位 (0x0000) -> 0x00, 0x00
            packet[13] = 0x00;
            packet[14] = 0x00;

            // 10. 先頭デバイス (3バイトアドレス): リトルエンディアンで格納
            packet[15] = (byte)(address & 0xFF);
            packet[16] = (byte)((address >> 8) & 0xFF);
            packet[17] = (byte)((address >> 16) & 0xFF);

            // 11. デバイスコード
            packet[18] = deviceCode;

            // 12. デバイス点数 (2バイト)
            packet[19] = (byte)(size & 0xFF);
            packet[20] = (byte)((size >> 8) & 0xFF);

            return packet;
        }

        private void ReadExact(NetworkStream stream, byte[] buffer)
        {
            int totalRead = 0;
            while (totalRead < buffer.Length)
            {
                int read = stream.Read(buffer, totalRead, buffer.Length - totalRead);
                if (read == 0)
                {
                    throw new IOException("Connection closed by PLC while reading data.");
                }
                totalRead += read;
            }
        }

        public void CloseConnection()
        {
            lock (_lockObject)
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }
            }
        }

        private void Log(string message)
        {
            OnLogMessage?.Invoke($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [INFO] {message}");
        }

        private void LogHex(string prefix, byte[] data)
        {
            if (OnLogMessage == null) return;
            string hexString = BitConverter.ToString(data).Replace("-", " ");
            OnLogMessage.Invoke($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [DEBUG] {prefix}: {hexString}");
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                CloseConnection();
                _isDisposed = true;
            }
        }
    }
}
