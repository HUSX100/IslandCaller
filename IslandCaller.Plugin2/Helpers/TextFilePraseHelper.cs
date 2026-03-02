using Avalonia.Platform.Storage;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using static IslandCaller.Services.ProfileService;

namespace IslandCaller.Helpers
{
    public class TextFilePraseHelper
    {
        public ILogger<TextFilePraseHelper> Logger = IAppHost.GetService<ILogger<TextFilePraseHelper>>();
        public async Task<List<Person>> ParseTextFileAsync(IStorageFile file)
        {
            string? path = file.Path.LocalPath;
            Logger.LogInformation($"获取到文本名单, 文件路径: {path}");
            string content;
            using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            content = await reader.ReadToEndAsync();
            var list = new List<Person>();
            var names = content.Split(new[] { ' ', ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < names.Length; i++)
            {
                list.Add(new Person
                {
                    Id = i + 1,
                    Name = names[i],
                    Gender = 0,
                    ManualWeight = 1.0
                });
            }
            return list;
        }
    }
}
