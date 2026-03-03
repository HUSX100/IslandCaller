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
            logger.LogInformation("开始解析 CSV 名单，文件路径: {Path}，姓名列: {NameRow}，性别列: {GenderRow}", path, nameRow, genderRow);
            string content;
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            content = await reader.ReadToEndAsync();
            var list = new List<Person>();
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            logger.LogDebug("CSV 原始行数: {LineCount}", lines.Length);
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
            logger.LogInformation("CSV 名单解析完成，成功导入 {Count} 人", list.Count);
            return list;
        }
    }
}
