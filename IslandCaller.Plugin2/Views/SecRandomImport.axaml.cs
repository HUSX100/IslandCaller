using Avalonia.Controls;

namespace IslandCaller.Views;

public partial class SecRandomImport : Window
{
    public SecRandomImport()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var result = ((bool)isGender.IsChecked, male_input.Text?.ToString() ?? "", female_input.Text?.ToString() ?? "");
        Close(result);
    }
}