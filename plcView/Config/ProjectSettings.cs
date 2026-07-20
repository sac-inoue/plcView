using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace plcView.Config
{
    public class PointSetting
    {
        public int No { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DeviceType { get; set; }      // D, W, M, X, Y, B, etc.
        public string StartAddress { get; set; }    // 文字列で保持（16進数入力対応用、例: "100", "1A0"）
        public int Size { get; set; }               // ワード数
    }

    public class PlcSetting
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int TimeoutMs { get; set; }

        public PlcSetting()
        {
            IpAddress = "192.168.3.39";
            Port = 5007;
            TimeoutMs = 2000;
        }
    }

    public class ProjectSettings
    {
        public string ProjectName { get; set; }
        public PlcSetting PlcSetting { get; set; }
        public int IntervalMs { get; set; }         // 0:最速, 1000:1秒, etc.
        public bool IsDebugMode { get; set; }
        public bool IsCsvMode { get; set; }         // true: CSVテキスト, false: バイナリ(dat)
        public string OutputFolderPath { get; set; }
        public List<PointSetting> Points { get; set; }

        public ProjectSettings()
        {
            ProjectName = "NewProject";
            PlcSetting = new PlcSetting();
            IntervalMs = 1000;
            IsDebugMode = false;
            IsCsvMode = true; // デフォルトはCSV
            OutputFolderPath = "";
            Points = new List<PointSetting>();
            for (int i = 1; i <= 10; i++)
            {
                Points.Add(new PointSetting
                {
                    No = i,
                    Enabled = false,
                    Name = $"Point {i}",
                    DeviceType = "D",
                    StartAddress = "0",
                    Size = 10
                });
            }
        }

        /// <summary>
        /// 設定をディープコピーします。
        /// </summary>
        public ProjectSettings Clone()
        {
            var clone = new ProjectSettings
            {
                ProjectName = this.ProjectName,
                IntervalMs = this.IntervalMs,
                IsDebugMode = this.IsDebugMode,
                IsCsvMode = this.IsCsvMode,
                OutputFolderPath = this.OutputFolderPath,
                PlcSetting = new PlcSetting
                {
                    IpAddress = this.PlcSetting.IpAddress,
                    Port = this.PlcSetting.Port,
                    TimeoutMs = this.PlcSetting.TimeoutMs
                },
                Points = new List<PointSetting>()
            };

            foreach (var pt in this.Points)
            {
                clone.Points.Add(new PointSetting
                {
                    No = pt.No,
                    Enabled = pt.Enabled,
                    Name = pt.Name,
                    DeviceType = pt.DeviceType,
                    StartAddress = pt.StartAddress,
                    Size = pt.Size
                });
            }

            return clone;
        }

        /// <summary>
        /// JSONファイルから設定をロードします。
        /// </summary>
        public static ProjectSettings Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Project file not found.", filePath);
            }

            string json = File.ReadAllText(filePath);
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<ProjectSettings>(json);
        }

        /// <summary>
        /// 設定をJSONファイルとして保存します。
        /// </summary>
        public void Save(string filePath)
        {
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(this);
            
            // JSONを見やすくフォーマットする簡易インデント処理 (JavaScriptSerializerは標準でフォーマット出力が非対応のため)
            json = FormatJson(json);

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }

        private string FormatJson(string json)
        {
            int indent = 0;
            var quoted = false;
            var sb = new System.Text.StringBuilder();
            for (var i = 0; i < json.Length; i++)
            {
                var ch = json[i];
                switch (ch)
                {
                    case '"':
                        quoted = !quoted;
                        sb.Append(ch);
                        break;
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            indent++;
                            sb.Append(new string(' ', indent * 2));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            indent--;
                            sb.Append(new string(' ', indent * 2));
                        }
                        sb.Append(ch);
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            sb.Append(new string(' ', indent * 2));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.Append(" ");
                        }
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
