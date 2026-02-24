using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.Models
{
    public class History : INotifyPropertyChanged
    {
        private Dictionary<string, int> historyDict = new();
        private List<string> top20List = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string GetBasePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "IslandCaller",
                "History"
            );
        }

        private string GetFilePath(Guid guid)
        {
            return Path.Combine(GetBasePath(), $"{guid}.txt");
        }

        /// <summary>
        /// 异步载入历史（只加载 Dictionary）
        /// </summary>
        public void Load(Guid guid)
        {
            historyDict.Clear();

            string filePath = GetFilePath(guid);

            if (!File.Exists(filePath))
                return;

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                if (parts.Length != 2)
                    continue;

                string name = parts[0];
                if (int.TryParse(parts[1], out int count))
                {
                    historyDict[name] = count;
                }
            }

            OnPropertyChanged(nameof(historyDict));
        }

        /// <summary>
        /// 写入历史
        /// </summary>
        public void Add(Guid guid, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            // 1️⃣ 更新 Dictionary（统计次数）
            if (historyDict.ContainsKey(name))
                historyDict[name]++;
            else
                historyDict[name] = 1;

            OnPropertyChanged(nameof(historyDict));

            // 2️⃣ 更新 top20（允许重复）
            top20List.Insert(0, name);

            if (top20List.Count > 20)
                top20List.RemoveAt(top20List.Count - 1);

            OnPropertyChanged(nameof(top20List));

            // 3️⃣ 自动保存
            Save(guid);
        }

        /// <summary>
        /// 保存长期记录到本地
        /// </summary>
        private void Save(Guid guid)
        {
            string basePath = GetBasePath();
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            string filePath = GetFilePath(guid);

            StringBuilder sb = new();

            foreach (var pair in historyDict)
            {
                sb.AppendLine($"{pair.Key},{pair.Value}");
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// 获取长期记录中某个名字的出现次数
        /// </summary>
        public int GetCount(string name)
        {
            if (historyDict.TryGetValue(name, out int count))
                return count;

            return 0;
        }

        /// <summary>
        /// 获取短期记录中的序号（1-based）
        /// 不存在返回 -1
        /// </summary>
        public int GetIndex(string name)
        {
            int index = top20List.IndexOf(name);

            if (index == -1)
                return -1;

            return index + 1;
        }

        /// <summary>
        /// 清空长期记录
        /// </summary>
        public void ClearHistory(Guid guid)
        {
            historyDict.Clear();
            Save(guid);
            OnPropertyChanged(nameof(historyDict));
        }

        /// <summary>
        /// 清空短期记录
        /// </summary>
        public void ClearTop20()
        {
            top20List.Clear();
            OnPropertyChanged(nameof(top20List));
        }
    }
}