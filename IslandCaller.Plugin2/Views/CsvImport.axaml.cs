using Avalonia.Controls;

namespace IslandCaller.Views;

public partial class CsvImport : Window
{
    public CsvImport()
    {
        InitializeComponent();
    }

    private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var tb = (TextBox)sender;
        if (!string.IsNullOrEmpty(tb.Text) && !int.TryParse(tb.Text, out int value))
        {
            tb.Text = string.Empty; // 非数字清空
        }
        else if (tb.Text.StartsWith("0")) // 禁止前导零
        {
            tb.Text = tb.Text.TrimStart('0');
        }
    }


    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = (Convert.ToInt32(name_row?.Text ?? "1"), Convert.ToInt32(gender_row?.Text ?? "1"), (bool)isGender.IsChecked, male_input.Text?.ToString() ?? "", female_input.Text?.ToString() ?? "");
        Close(result);
    }
}