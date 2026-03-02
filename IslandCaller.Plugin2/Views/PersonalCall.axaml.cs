using Avalonia.Controls;
using ClassIsland.Shared;
using IslandCaller.Services.IslandCallerService;
using System.ComponentModel;

namespace IslandCaller.Views;

public partial class PersonalCall : Window,INotifyPropertyChanged
{
    public double Num { get; set; }
    private IslandCallerService IslandCallerService { get; }
    public PersonalCall()
    {
        IslandCallerService = IAppHost.GetService<IslandCallerService>();   
        InitializeComponent();
        DataContext = this;
    }
    private void CancelButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
    private void SureButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IslandCallerService.ShowRandomStudent((int)Num);
        this.Close();
    }
}