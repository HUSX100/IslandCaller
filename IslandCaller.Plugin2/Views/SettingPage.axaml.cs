using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Controls;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;
using IslandCaller.Helpers;
using IslandCaller.Services;
using IslandCaller.ViewModels;
using System.Collections.ObjectModel;
using static IslandCaller.Services.ProfileService;
using static IslandCaller.ViewModels.SettingPageViewModel;

namespace IslandCaller.Views;

[SettingsPageInfo("plugins.IslandCaller", "IslandCaller 设置", "\uECF9", "\uECF8", SettingsPageCategory.External)]
public partial class SettingPage : SettingsPageBase
{
    private SettingPageViewModel vm;
    private HistoryService HistoryService;
    public SettingPage()
    {
        InitializeComponent();
        vm = this.DataContext as SettingPageViewModel;
        HistoryService = IAppHost.GetService<HistoryService>();
    }

    private void AddButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int nextId = vm.ProfileList.Any() ? vm.ProfileList.Max(s => s.ID) + 1 : 1;
        vm.ProfileList.Add(new SettingPageViewModel.StudentModel
        {
            ID = nextId,
            Name = "",
            Gender = 0,
            ManualWeight = 1.0
        });
    }
    private async void ImportButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        List<Person> newlist = new List<Person>();
        await CommonTaskDialogs.ShowDialog("导入提示", "导入的名单仅支持下列格式: \n\n" +
            "文本名单 (*.txt): 名单仅包含姓名，使用空格，逗号，或换行分隔\n\n" +
            "SecRandom 名单 (\\list\\rool_call_list\\*.json)\n\n" +
            "CSV 名单 (*.csv): 名单包含姓名,性别可选，不能含有标题");
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择要导入的名单",
            AllowMultiple = false, // 是否允许多选
            FileTypeFilter = new[]
            {
                new FilePickerFileType("文本名单")
                {
                    Patterns = new[] { "*.txt" }
                },
                new FilePickerFileType("SecRandom 名单")
                {
                    Patterns = new[] { "*.json" }
                },
                new FilePickerFileType("CSV 名单")
                {
                    Patterns = new[] { "*.csv" }
                }
            }
        });
        if (files.Count > 0)
        {
            IStorageFile file = files[0];
            switch (Path.GetExtension(file.Name))
            {
                case ".txt":
                    newlist = await new TextFilePraseHelper().ParseTextFileAsync(file);
                    break;
                case ".json":
                    var result_json = await new SecRandomImport().ShowDialog<(bool isGender, string male, string female)>(this.GetVisualRoot() as Window);
                    newlist = await new SecRandomParseHelper().ParseSecRandomProfileAsync(file,result_json.isGender,result_json.male,result_json.female);
                    break;
                case ".csv":
                    var result_csv = await new CsvImport().ShowDialog<(int nameRow, int genderRow, bool isGender, string male, string female)>(this.GetVisualRoot() as Window);
                    result_csv.nameRow -= 1;
                    result_csv.genderRow -= 1;
                    if (!result_csv.isGender) result_csv.genderRow = -1;
                    newlist = await new CsvParseHelper().ParseCsvFileAsync(file, result_csv.nameRow, result_csv.genderRow, result_csv.male, result_csv.female);
                    break;
            }
            var ordered_profile = newlist
            .OrderBy(m => m.Id)
            .Select(m => new StudentModel
            {
                ID = m.Id,
                Name = m.Name,
                Gender = m.Gender,
                ManualWeight = m.ManualWeight
            });
            vm.ProfileList = new ObservableCollection<StudentModel> (ordered_profile);
        }

    }

    private void ClearButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        HistoryService.ClearThisLessonHistory();
        HistoryService.ClearLongTermHistory();
    }
}