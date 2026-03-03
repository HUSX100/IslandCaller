using Avalonia.Platform.Storage;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static IslandCaller.Services.ProfileService;

namespace IslandCaller.Helpers
{
    public class SecRandomParseHelper
    {
        public ILogger<SecRandomParseHelper> logger = IAppHost.GetService<ILogger<SecRandomParseHelper>>();
        public async Task<List<Person>> ParseSecRandomProfileAsync(IStorageFile file, bool isGender, string? male, string? female)
        {
            string? path = file.Path.LocalPath;
            logger.LogInformation("开始解析 SecRandom 名单，文件路径: {Path}，是否读取性别: {IsGender}", path, isGender);
            using var stream = await file.OpenReadAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(stream);
            var list = new List<Person>();
            int i = 0;
            int rawCount = 0;
            foreach (var person in jsonDoc.RootElement.EnumerateObject())
            {
                rawCount++;
                i++;
                int gender = 0;
                if (isGender)
                {
                    string gender_text = person.Value.GetProperty("gender").ToString();
                    if (gender_text == male) gender = 0;
                    else if (gender_text == female) gender = 1;
                    else
                    {
                        logger.LogWarning($"第{i}行: 无法识别的性别值: {gender_text}");
                        continue;
                    }
                }
                list.Add(new Person
                {
                    Id = i,
                    Name = person.Name,
                    Gender = gender,
                    ManualWeight = 1.0
                });
            }
            logger.LogDebug("SecRandom 原始人数: {PersonCount}", rawCount);
            logger.LogInformation("SecRandom 名单解析完成，成功导入 {Count} 人", list.Count);
            return list;
        }
    }
}
