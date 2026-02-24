using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IslandCaller.Models
{
    public class Profile
    {
        public class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Gender { get; set; }
        }
        // 名单存储
        public List<Person> Members { get; set; } = new List<Person>();

        private string GetBasePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "IslandCaller",
                "Profile"
            );
        }

        private string GetFilePath(Guid guid)
        {
            return Path.Combine(GetBasePath(), $"{guid}.csv");
        }

        // 读取名单
        public async Task Load(Guid guid)
        {
            var logger = IAppHost.GetService<ILogger<Profile>>();
            // 构建路径

            string filePath = Path.Combine(GetFilePath(guid));

            // 如果文件不存在
            if (!File.Exists(filePath))
            {
                logger.LogError($"找不到对应的名单文件: {filePath}");
                throw new FileNotFoundException($"找不到对应的名单文件: {filePath}");
            }

            string[] lines = await File.ReadAllLinesAsync(filePath);

            if (lines.Length == 0)
            {
                logger.LogError("CSV 文件为空");
                throw new Exception("CSV 文件为空");
            }

            // 检查标题
            string header = lines[0].Trim();
            if (header != "id,name,gender")
            {
                logger.LogError("CSV 标题格式错误，必须为: id,name,gender");
                throw new Exception("CSV 标题格式错误，必须为: id,name,gender");
            }

            Members.Clear();

            // 读取数据
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length != 3)
                {
                    logger.LogWarning($"第 {i + 1} 行格式错误: {line}");
                    continue;
                }

                Members.Add(new Person
                {
                    Id = Convert.ToInt32(parts[0]),
                    Name = parts[1],
                    Gender = Convert.ToInt32(parts[2])
                });
            }
        }

        // 获取名单
        public async Task<List<Person>> GetMembers(Guid guid)
        {
            var logger = IAppHost.GetService<ILogger<Profile>>();
            string filePath = GetFilePath(guid);

            if (!File.Exists(filePath))
            {
                logger.LogError($"找不到对应的名单文件: {filePath}");
                throw new FileNotFoundException($"找不到对应的名单文件: {filePath}");
            }

            string[] lines = await File.ReadAllLinesAsync(filePath);

            if (lines.Length == 0)
            {
                logger.LogError("CSV 文件为空");
                throw new Exception("CSV 文件为空");
            }

            if (lines[0].Trim() != "id,name,gender")
            {
                logger.LogError("CSV 标题格式错误，必须为: id,name,gender");
                throw new Exception("CSV 标题格式错误，必须为: id,name,gender");
            }

            List<Person> members = new List<Person>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');

                if (parts.Length != 3)
                {
                    logger.LogWarning($"第 {i + 1} 行格式错误: {line}");
                    continue;
                }

                members.Add(new Person
                {
                    Id = Convert.ToInt32(parts[0]),
                    Name = parts[1],
                    Gender = Convert.ToInt32(parts[2])
                });
            }

            return members;
        }

        // 写入名单（覆盖或创建）
        public async Task SaveMembersAsync(Guid guid, List<Person> members)
        {
            var logger = IAppHost.GetService<ILogger<Profile>>();
            string basePath = GetBasePath();

            // 如果目录不存在就创建
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            string filePath = GetFilePath(guid);

            StringBuilder sb = new StringBuilder();

            // 写标题
            sb.AppendLine("id,name,gender");

            foreach (var person in members)
            {
                sb.AppendLine($"{person.Id},{person.Name},{person.Gender}");
            }

            // 覆盖写入
            try
            {
                await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"写入名单文件失败: {filePath}");
                throw new Exception($"写入名单文件失败: {filePath}", ex);
            }
        }

        public void CreateDemoProfile(Guid guid)
        { 
            List<Person> members = new List<Person>();
            members.Add(new Person
            {
                Id = 1,
                Gender = 0,
                Name = "小明"
            });
            members.Add(new Person
            {
                Id = 1,
                Gender = 0,
                Name = "李明"
            });
            members.Add(new Person
            {
                Id = 1,
                Gender = 1,
                Name = "李华"
            });
        }
    }
}
