using Avalonia.Platform.Storage;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using static IslandCaller.Services.ProfileService;

namespace IslandCaller.Helpers
{
    public class CsvParseHelper
    {
        public ILogger<CsvParseHelper> logger = IAppHost.GetService<ILogger<CsvParseHelper>>();
        public async Task<List<Person>> ParseCsvFileAsync(IStorageFile file, int nameRow, int genderRow, string? male, string? female) 
        { 
            string? path = file.Path.LocalPath;
            logger.LogInformation($"获取到CSV名单, 文件路径: {path}");
            string content;
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            content = await reader.ReadToEndAsync();
            var list = new List<Person>();
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                var columns = lines[i].Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (columns.Length > Math.Max(nameRow, genderRow))
                {
                    string name = columns[nameRow].Trim();
                    int gender;
                    if (genderRow == -1) gender = 0;
                    else
                    {
                        if (columns[genderRow] == male) gender = 0;
                        else if (columns[genderRow] == female) gender = 1;
                        else
                        {
                            logger.LogWarning($"第{i}行: 无法识别的性别值: {columns[genderRow]}");
                            continue;
                        }
                    }
                    list.Add(new Person
                    {
                        Id = i + 1,
                        Name = name,
                        Gender = gender,
                        ManualWeight = 1.0
                    });
                }
            }
            return list;
        }
    }
}