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
            logger.LogInformation($"获取到SecRandom Profile, 文件路径: {path}");
            using var stream = await file.OpenReadAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(stream);
            var list = new List<Person>();
            int i = 0;
            foreach(var person in jsonDoc.RootElement.EnumerateObject())
            {
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
            return list;
        }
    }
}
