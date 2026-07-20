using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using plcView.Config;
using plcView.Data;
using plcView.Plc;

namespace plcView
{
    public partial class MainForm : Form
    {
        private ProjectSettings _settings = new ProjectSettings();
        private CancellationTokenSource _collectCts;
        private Task _collectTask;
        
        // リアルタイム表示用の最新データ保持
        private readonly Dictionary<int, ushort[]> _latestMonitorData = new Dictionary<int, ushort[]>();
        private readonly object _monitorLock = new object();
        private DateTime _latestMonitorTime = DateTime.MinValue;

        // 履歴再生関連
        private HistoryReader _historyReader;
        private System.Windows.Forms.Timer _playTimer;
        private bool _isPlaying = false;

        public MainForm()
        {
            InitializeComponent();
            
            // 各種初期設定
            InitIntervalComboBox();
            InitPointsDataGridView();
            InitMonitorTab();
            InitHistoryTab();
            
            // DataGridViewのチラつき防止（ダブルバッファリング有効化）
            EnableDoubleBuffering(dgvPoints);
            EnableDoubleBuffering(dgvMonitor);
            EnableDoubleBuffering(dgvHistory);

            LoadSettingsToUI();
            UpdateStatusLabel("停止中", Color.LightGray);
        }

        private void EnableDoubleBuffering(DataGridView dgv)
        {
            var dgvType = dgv.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi?.SetValue(dgv, true, null);
        }

        #region 初期化

        private void InitIntervalComboBox()
        {
            cmbInterval.Items.Clear();
            cmbInterval.Items.Add(new KeyValuePair<int, string>(0, "0 (最速)"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(1000, "1秒"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(2000, "2秒"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(5000, "5秒"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(10000, "10秒"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(30000, "30秒"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(60000, "1分"));
            cmbInterval.Items.Add(new KeyValuePair<int, string>(300000, "5分"));

            cmbInterval.DisplayMember = "Value";
            cmbInterval.ValueMember = "Key";
            cmbInterval.SelectedIndex = 1; // 1秒デフォルト
        }

        private void InitPointsDataGridView()
        {
            dgvPoints.AutoGenerateColumns = false;
            dgvPoints.Columns.Clear();

            dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "No", HeaderText = "No", Width = 40, ReadOnly = true });
            dgvPoints.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Enabled", HeaderText = "有効", Width = 50 });
            dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "名称/メモ", Width = 150 });
            
            var cmbCol = new DataGridViewComboBoxColumn
            {
                DataPropertyName = "DeviceType",
                HeaderText = "デバイス",
                Width = 80
            };
            cmbCol.Items.AddRange("D", "W", "R", "ZR", "SD", "SW", "SM", "TN", "TS", "TC", "CN", "CS", "CC", "M", "X", "Y", "L", "F", "V", "B", "SB", "S", "DX", "DY");
            dgvPoints.Columns.Add(cmbCol);

            dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "StartAddress", HeaderText = "開始アドレス", Width = 100 });
            dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Size", HeaderText = "サイズ (ワード)", Width = 100 });

            // イベントバインド
            btnNewProject.Click += (s, e) => ActionSafe(NewProject);
            btnOpenProject.Click += (s, e) => ActionSafe(OpenProject);
            btnSaveProject.Click += (s, e) => ActionSafe(SaveProject);
            btnBrowseFolder.Click += (s, e) => ActionSafe(BrowseOutputFolder);
            btnStart.Click += (s, e) => ActionSafe(StartCollection);
            btnStop.Click += (s, e) => ActionSafe(StopCollection);
        }

        private void InitMonitorTab()
        {
            cmbMonitorPoint.Items.Clear();
            for (int i = 1; i <= 10; i++)
            {
                cmbMonitorPoint.Items.Add($"Point {i}");
            }
            cmbMonitorPoint.SelectedIndex = 0;

            rbUnitWord.CheckedChanged += (s, e) => UpdateMonitorView();
            rbUnitByte.CheckedChanged += (s, e) => UpdateMonitorView();
            cmbMonitorPoint.SelectedIndexChanged += (s, e) => UpdateMonitorView();

            InitMonitorGrid(dgvMonitor);
        }

        private void InitMonitorGrid(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "アドレス", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "値 (10進)", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "16進 (Hex)", Width = 100, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "2進 (Binary)", Width = 150, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "32bit解釈", Width = 120, ReadOnly = true });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ASCII (文字)", Width = 100, ReadOnly = true });
        }

        private void InitHistoryTab()
        {
            cmbHistoryPoint.Items.Clear();
            for (int i = 1; i <= 10; i++)
            {
                cmbHistoryPoint.Items.Add($"Point {i}");
            }
            cmbHistoryPoint.SelectedIndex = 0;

            cmbHistorySpeed.Items.Clear();
            cmbHistorySpeed.Items.Add("1倍速");
            cmbHistorySpeed.Items.Add("2倍速");
            cmbHistorySpeed.Items.Add("5倍速");
            cmbHistorySpeed.Items.Add("10倍速");
            cmbHistorySpeed.SelectedIndex = 0;

            btnOpenHistoryFile.Click += (s, e) => ActionSafe(OpenHistoryFile);
            trackHistory.Scroll += (s, e) => DisplayHistoryFrame(trackHistory.Value);

            btnPlayHistory.Click += (s, e) => StartHistoryPlayback();
            btnPauseHistory.Click += (s, e) => PauseHistoryPlayback();
            btnPrevFrame.Click += (s, e) => StepHistoryFrame(-1);
            btnNextFrame.Click += (s, e) => StepHistoryFrame(1);

            _playTimer = new System.Windows.Forms.Timer();
            _playTimer.Tick += PlayTimer_Tick;

            InitMonitorGrid(dgvHistory);
            cmbHistoryPoint.SelectedIndexChanged += (s, e) => DisplayHistoryFrame(trackHistory.Value);
        }

        #endregion

        #region 設定UIロード/保存

        private void LoadSettingsToUI()
        {
            txtIpAddress.Text = _settings.PlcSetting.IpAddress;
            numPort.Value = _settings.PlcSetting.Port;
            numTimeout.Value = _settings.PlcSetting.TimeoutMs;
            chkDebugMode.Checked = _settings.IsDebugMode;
            chkCsvMode.Checked = _settings.IsCsvMode;
            txtOutputFolder.Text = _settings.OutputFolderPath;

            // 収集間隔の選択
            for (int i = 0; i < cmbInterval.Items.Count; i++)
            {
                var kv = (KeyValuePair<int, string>)cmbInterval.Items[i];
                if (kv.Key == _settings.IntervalMs)
                {
                    cmbInterval.SelectedIndex = i;
                    break;
                }
            }

            // DataGridViewにバインド
            dgvPoints.DataSource = null;
            dgvPoints.DataSource = _settings.Points;
        }

        private void SaveUIToSettings()
        {
            _settings.PlcSetting.IpAddress = txtIpAddress.Text.Trim();
            _settings.PlcSetting.Port = (int)numPort.Value;
            _settings.PlcSetting.TimeoutMs = (int)numTimeout.Value;
            _settings.IsDebugMode = chkDebugMode.Checked;
            _settings.IsCsvMode = chkCsvMode.Checked;
            _settings.OutputFolderPath = txtOutputFolder.Text.Trim();
            
            if (cmbInterval.SelectedItem != null)
            {
                _settings.IntervalMs = ((KeyValuePair<int, string>)cmbInterval.SelectedItem).Key;
            }
        }

        #endregion

        #region コントロールアクション

        private void ActionSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewProject()
        {
            if (MessageBox.Show("現在の設定をクリアして新規プロジェクトを作成しますか？", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _settings = new ProjectSettings();
                LoadSettingsToUI();
                AppendLog("新規プロジェクト設定を作成しました。");
            }
        }

        private void OpenProject()
        {
            using (var ofd = new OpenFileDialog { Filter = "JSONファイル|*.json|すべてのファイル|*.*" })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    _settings = ProjectSettings.Load(ofd.FileName);
                    LoadSettingsToUI();
                    AppendLog($"プロジェクトを読み込みました: {ofd.FileName}");
                }
            }
        }

        private void SaveProject()
        {
            SaveUIToSettings();
            using (var sfd = new SaveFileDialog { Filter = "JSONファイル|*.json", FileName = "project.json" })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    _settings.Save(sfd.FileName);
                    AppendLog($"プロジェクトを保存しました: {sfd.FileName}");
                }
            }
        }

        private void BrowseOutputFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    txtOutputFolder.Text = fbd.SelectedPath;
                }
            }
        }

        #endregion

        #region データ収集処理（スレッド）

        private void StartCollection()
        {
            SaveUIToSettings();

            if (string.IsNullOrEmpty(_settings.PlcSetting.IpAddress))
            {
                throw new InvalidOperationException("PLCのIPアドレスを設定してください。");
            }
            if (string.IsNullOrEmpty(_settings.OutputFolderPath))
            {
                throw new InvalidOperationException("データ保存先フォルダを設定してください。");
            }

            _collectCts = new CancellationTokenSource();
            _collectTask = Task.Run(() => CollectLoopAsync(_collectCts.Token));

            UpdateStatusLabel("収集開始中", Color.Yellow);
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            SetSettingsInputsEnabled(false);
        }

        private void StopCollection()
        {
            if (_collectCts != null)
            {
                _collectCts.Cancel();
                UpdateStatusLabel("停止処理中", Color.Orange);
            }
        }

        private void SetSettingsInputsEnabled(bool enabled)
        {
            groupBoxPlc.Enabled = enabled;
            groupBoxProject.Enabled = enabled;
            dgvPoints.ReadOnly = !enabled;
        }

        private void UpdateStatusLabel(string text, Color color)
        {
            if (InvokeRequired)
            {
                SafeInvoke(new Action(() => UpdateStatusLabel(text, color)));
                return;
            }
            lblStatus.Text = text;
            lblStatus.BackColor = color;
        }

        private async Task CollectLoopAsync(CancellationToken token)
        {
            // 収集実行中のスレッド競合を防ぐため設定のスナップショットを使用
            var snapshot = _settings.Clone();

            var client = new McProtocolClient(
                snapshot.PlcSetting.IpAddress, 
                snapshot.PlcSetting.Port, 
                snapshot.PlcSetting.TimeoutMs
            );

            client.OnLogMessage += (msg) =>
            {
                // デバッグログが有効、または通常の接続情報などのみUIにログ出力
                if (snapshot.IsDebugMode || msg.Contains("[INFO]"))
                {
                    AppendLog(msg);
                }
                
                // デバッグログ書き出し
                if (snapshot.IsDebugMode)
                {
                    WriteDebugLog(msg);
                }
            };

            DataLogger logger = null;
            try
            {
                logger = new DataLogger(snapshot);

                // 接続確立（自動リトライ付き）
                await client.ConnectAsync(token);
                if (token.IsCancellationRequested) return;

                UpdateStatusLabel("収集実行中", Color.LightGreen);

                while (!token.IsCancellationRequested)
                {
                    var loopStartTime = DateTime.Now;
                    var sampleData = new Dictionary<int, ushort[]>();

                    foreach (var point in snapshot.Points)
                    {
                        if (token.IsCancellationRequested) break;
                        if (!point.Enabled) continue;

                        try
                        {
                            byte[] rawBytes = client.ReadWords(point.DeviceType, point.StartAddress, point.Size);
                            ushort[] words = new ushort[point.Size];
                            for (int i = 0; i < point.Size; i++)
                            {
                                words[i] = BitConverter.ToUInt16(rawBytes, i * 2);
                            }

                            sampleData[point.No] = words;
                        }
                        catch (Exception ex)
                        {
                            client.CloseConnection(); // ソケット破損対策で一旦閉じる
                            AppendLog($"[ERR] Point {point.No} 取得失敗: {ex.Message}");
                            
                            // 再接続を試みる
                            await client.ConnectAsync(token);
                            break; // この回のループはスキップして仕切り直し
                        }
                    }

                    if (sampleData.Count > 0 && !token.IsCancellationRequested)
                    {
                        // ログファイルへの記録
                        logger.WriteData(loopStartTime, sampleData);

                        // リアルタイムモニタキャッシュ更新
                        lock (_monitorLock)
                        {
                            _latestMonitorTime = loopStartTime;
                            foreach (var kv in sampleData)
                            {
                                _latestMonitorData[kv.Key] = kv.Value;
                            }
                        }

                        // UIのグリッド更新（非同期で促す）
                        SafeBeginInvoke(new Action(UpdateMonitorView));
                    }

                    // 待機時間計算
                    int waitMs = snapshot.IntervalMs;
                    if (waitMs <= 0)
                    {
                        // 0ms設定時は、CPU張り付き防止に1msウェイトを入れる
                        waitMs = 1;
                    }

                    var elapsed = (int)(DateTime.Now - loopStartTime).TotalMilliseconds;
                    var remainingWait = waitMs - elapsed;

                    if (remainingWait > 0)
                    {
                        await Task.Delay(remainingWait, token);
                    }
                    else
                    {
                        // 処理時間がサンプリング周期を超えている場合も、最低1msの息継ぎを入れる
                        await Task.Delay(1, token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // キャンセルによる正常終了
            }
            catch (Exception ex)
            {
                AppendLog($"[FATAL] 収集ループで致命的なエラーが発生しました: {ex.Message}");
            }
            finally
            {
                if (logger != null)
                {
                    logger.Dispose();
                }
                client.Dispose();
                UpdateStatusLabel("停止中", Color.LightGray);
                
                SafeBeginInvoke(new Action(() =>
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    SetSettingsInputsEnabled(true);
                }));
            }
        }

        private void AppendLog(string message)
        {
            if (InvokeRequired)
            {
                SafeBeginInvoke(new Action(() => AppendLog(message)));
                return;
            }

            txtLog.AppendText(message + Environment.NewLine);
            // ログが長くなりすぎたら切り詰める
            if (txtLog.TextLength > 50000)
            {
                txtLog.Text = txtLog.Text.Substring(10000);
            }
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.ScrollToCaret();
        }

        private void WriteDebugLog(string message)
        {
            try
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                string logPath = Path.Combine(logDir, $"debug_{DateTime.Today:yyyyMMdd}.log");
                File.AppendAllText(logPath, message + Environment.NewLine, Encoding.UTF8);
            }
            catch
            {
                // ログ保存失敗時はサイレントに無視
            }
        }

        #endregion

        #region メモリモニター表示ロジック

        private void UpdateMonitorView()
        {
            int selectedPointIndex = cmbMonitorPoint.SelectedIndex + 1;
            PointSetting point = _settings.Points.FirstOrDefault(p => p.No == selectedPointIndex);

            if (point == null || !point.Enabled)
            {
                dgvMonitor.Rows.Clear();
                return;
            }

            ushort[] data = null;
            lock (_monitorLock)
            {
                if (_latestMonitorData.TryGetValue(point.No, out ushort[] cache))
                {
                    data = (ushort[])cache.Clone();
                }
            }

            if (data == null)
            {
                dgvMonitor.Rows.Clear();
                return;
            }

            RenderGrid(dgvMonitor, point, data, rbUnitByte.Checked);
        }

        private void RenderGrid(DataGridView dgv, PointSetting point, ushort[] data, bool isByteUnit)
        {
            int scrollRow = dgv.FirstDisplayedScrollingRowIndex;
            
            int targetRowCount = isByteUnit ? data.Length * 2 : data.Length;
            if (dgv.RowCount != targetRowCount)
            {
                dgv.Rows.Clear();
                if (targetRowCount > 0)
                {
                    dgv.Rows.Add(targetRowCount);
                }
            }

            if (!DeviceConverter.TryParse(point.DeviceType, point.StartAddress, out _, out int startAddressNum))
            {
                dgv.Rows.Clear();
                dgv.Rows.Add("Error: 不正なアドレス設定", "", "", "", "", "");
                return;
            }

            if (isByteUnit)
            {
                // バイト単位 (8ビット) 表示
                for (int i = 0; i < data.Length; i++)
                {
                    ushort wordVal = data[i];
                    byte lowByte = (byte)(wordVal & 0xFF);
                    byte highByte = (byte)((wordVal >> 8) & 0xFF);

                    // Low Byte
                    UpdateByteRow(dgv, point.DeviceType, startAddressNum, i, "L", lowByte, i * 2);
                    // High Byte
                    UpdateByteRow(dgv, point.DeviceType, startAddressNum, i, "H", highByte, i * 2 + 1);
                }
            }
            else
            {
                // ワード単位 (16ビット) 表示
                for (int i = 0; i < data.Length; i++)
                {
                    ushort val = data[i];
                    int addr = startAddressNum + i;
                    string addressStr = $"{point.DeviceType}{GetAddressDisplayString(point.DeviceType, addr)}";

                    string val10Str = ((short)val).ToString(); // 符号あり10進数
                    string valHexStr = $"0x{val:X4}";
                    string valBinStr = Convert.ToString(val, 2).PadLeft(16, '0');

                    string val32Str = "";
                    if (i + 1 < data.Length)
                    {
                        uint dwordVal = (uint)((data[i + 1] << 16) | val);
                        float floatVal = BitConverter.ToSingle(BitConverter.GetBytes(dwordVal), 0);
                        val32Str = $"{dwordVal} / {floatVal:F3}";
                    }

                    string asciiStr = GetAsciiString(val);

                    DataGridViewRow row = dgv.Rows[i];
                    row.Cells[0].Value = addressStr;
                    row.Cells[1].Value = val10Str;
                    row.Cells[2].Value = valHexStr;
                    row.Cells[3].Value = valBinStr;
                    row.Cells[4].Value = val32Str;
                    row.Cells[5].Value = asciiStr;
                }
            }

            if (scrollRow >= 0 && scrollRow < dgv.RowCount)
            {
                dgv.FirstDisplayedScrollingRowIndex = scrollRow;
            }
        }

        private void UpdateByteRow(DataGridView dgv, string deviceType, int startAddr, int offset, string suffix, byte val, int rowIndex)
        {
            int addr = startAddr + offset;
            string addressStr = $"{deviceType}{GetAddressDisplayString(deviceType, addr)}-{suffix}";
            string val10Str = val.ToString();
            string valHexStr = $"0x{val:X2}";
            string valBinStr = Convert.ToString(val, 2).PadLeft(8, '0');
            string val32Str = "-";
            string asciiStr = (val >= 32 && val <= 126) ? ((char)val).ToString() : ".";

            DataGridViewRow row = dgv.Rows[rowIndex];
            row.Cells[0].Value = addressStr;
            row.Cells[1].Value = val10Str;
            row.Cells[2].Value = valHexStr;
            row.Cells[3].Value = valBinStr;
            row.Cells[4].Value = val32Str;
            row.Cells[5].Value = asciiStr;
        }

        private string GetAddressDisplayString(string deviceType, int address)
        {
            if (DeviceConverter.IsHexDevice(deviceType))
            {
                return address.ToString("X");
            }
            return address.ToString();
        }

        private string GetAsciiString(ushort val)
        {
            byte b1 = (byte)(val & 0xFF);
            byte b2 = (byte)((val >> 8) & 0xFF);

            char c1 = (b1 >= 32 && b1 <= 126) ? (char)b1 : '.';
            char c2 = (b2 >= 32 && b2 <= 126) ? (char)b2 : '.';

            return $"{c1}{c2}";
        }

        #endregion

        #region 履歴再生ロジック

        private void OpenHistoryFile()
        {
            using (var ofd = new OpenFileDialog { Filter = "バイナリ履歴データ|*.dat|すべてのファイル|*.*" })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    txtHistoryFile.Text = ofd.FileName;
                    _historyReader = new HistoryReader(ofd.FileName);

                    if (_historyReader.FrameCount > 0)
                    {
                        trackHistory.Minimum = 0;
                        trackHistory.Maximum = _historyReader.FrameCount - 1;
                        trackHistory.Value = 0;
                        trackHistory.Enabled = true;

                        DisplayHistoryFrame(0);
                        AppendLog($"履歴ファイルをロードしました。フレーム数: {_historyReader.FrameCount}");
                    }
                    else
                    {
                        trackHistory.Enabled = false;
                        MessageBox.Show(this, "履歴ファイルに有効なデータが含まれていません。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void DisplayHistoryFrame(int index)
        {
            if (_historyReader == null || index < 0 || index >= _historyReader.FrameCount) return;

            HistoryFrame frame = _historyReader.ReadFrame(index);
            if (frame == null) return;

            lblHistoryTime.Text = $"再生日時: {frame.Timestamp:yyyy-MM-dd HH:mm:ss.fff}";

            int selectedPointIndex = cmbHistoryPoint.SelectedIndex + 1;
            PointSetting point = _settings.Points.FirstOrDefault(p => p.No == selectedPointIndex);

            if (point == null || !point.Enabled)
            {
                dgvHistory.Rows.Clear();
                return;
            }

            if (frame.Data.TryGetValue(point.No, out ushort[] data))
            {
                RenderGrid(dgvHistory, point, data, false); // 履歴はワード固定表示
            }
            else
            {
                dgvHistory.Rows.Clear();
            }
        }

        private void StartHistoryPlayback()
        {
            if (_historyReader == null || _historyReader.FrameCount == 0) return;

            _isPlaying = true;
            btnPlayHistory.Enabled = false;
            btnPauseHistory.Enabled = true;

            // 再生速度の取得
            int speedMultiplier = 1;
            string speedText = cmbHistorySpeed.SelectedItem?.ToString() ?? "1倍速";
            if (speedText.Contains("2")) speedMultiplier = 2;
            else if (speedText.Contains("5")) speedMultiplier = 5;
            else if (speedText.Contains("10")) speedMultiplier = 10;

            // 元々の収集周期（例: 1000ms）を倍率で割る。最速0msの場合は100ms周期とする
            int baseInterval = _settings.IntervalMs > 0 ? _settings.IntervalMs : 1000;
            int playInterval = Math.Max(50, baseInterval / speedMultiplier);

            _playTimer.Interval = playInterval;
            _playTimer.Start();
        }

        private void PauseHistoryPlayback()
        {
            _isPlaying = false;
            _playTimer.Stop();
            btnPlayHistory.Enabled = true;
            btnPauseHistory.Enabled = false;
        }

        private void StepHistoryFrame(int step)
        {
            if (_historyReader == null || _historyReader.FrameCount == 0) return;

            int nextIndex = trackHistory.Value + step;
            if (nextIndex >= 0 && nextIndex < _historyReader.FrameCount)
            {
                trackHistory.Value = nextIndex;
                DisplayHistoryFrame(nextIndex);
            }
            else if (nextIndex >= _historyReader.FrameCount && _isPlaying)
            {
                // ループ再生か停止
                PauseHistoryPlayback();
            }
        }

        private void PlayTimer_Tick(object sender, EventArgs e)
        {
            if (trackHistory.Value < trackHistory.Maximum)
            {
                StepHistoryFrame(1);
            }
            else
            {
                // 末尾に達したらループか一時停止
                PauseHistoryPlayback();
                trackHistory.Value = 0;
                DisplayHistoryFrame(0);
            }
        }

        private void SafeInvoke(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                Invoke(action);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        private void SafeBeginInvoke(Action action)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                BeginInvoke(action);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        #endregion
    }
}
