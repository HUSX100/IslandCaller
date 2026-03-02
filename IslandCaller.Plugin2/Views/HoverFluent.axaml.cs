using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using IslandCaller.Services.IslandCallerService;
using IslandCaller.ViewModels;
using Microsoft.Extensions.Logging;

namespace IslandCaller.Views;
public partial class HoverFluent : Window
{
    private HoverFluentViewModel vm {  get; set; }
    private double scaling {  get; set; }
    private ILogger<HoverFluent> logger = ClassIsland.Shared.IAppHost.GetService<ILogger<HoverFluent>>();
    private IslandCallerService IslandCallerService = ClassIsland.Shared.IAppHost.GetService<IslandCallerService>();
    public HoverFluent()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        vm = this.DataContext as HoverFluentViewModel;
        scaling = this.RenderScaling;
        this.Position = new PixelPoint((int)Math.Round(vm.PositionX * scaling), (int)Math.Round(vm.PositionY * scaling));
        PositionChanged += OnPositionChanged;
        logger.LogDebug($"HoverFluent 坐标: PositionX={(int)Math.Round(vm.PositionX * scaling)}, PositionY={(int)Math.Round(vm.PositionY * scaling)}");
        logger.LogInformation("HoverFluent 悬浮窗初始化成功");

        Task.Run(async () =>
        {
            logger.LogInformation("HoverFluent 置顶任务启动");
            while (true)
            {
                await Task.Delay(5000);
                Dispatcher.UIThread.Invoke(() =>
                {
                    this.Topmost= true;
                    this.Focusable = false;
                });
            }
            ;
        });
    }

    private void OnPositionChanged(object? sender, PixelPointEventArgs e)
    {
        var screen = Screens.ScreenFromWindow(this)?.Bounds ?? Screens.Primary.Bounds;
        scaling = this.RenderScaling;
        var logger = ClassIsland.Shared.IAppHost.GetService<ILogger<HoverFluent>>();
        logger.LogDebug($"窗口位置改变: X={Position.X}, Y={Position.Y}");

        // 当前窗口位置和大小
        int x = Position.X;
        int y = Position.Y;
        int w = (int)Width;
        int h = (int)Height;

        // 修正坐标，保证窗口完全在屏幕内
        if (x < screen.X) x = screen.X;
        if (y < screen.Y) y = screen.Y;
        if (x + w > screen.X + screen.Width)
        {
            x = screen.X + screen.Width - w;
            logger.LogInformation("调整X坐标以适应屏幕");
        }
        if (y + h > screen.Y + screen.Height)
        {
            y = screen.Y + screen.Height - h;
            logger.LogInformation("调整Y坐标以适应屏幕");
        }

        // 如果有调整，更新位置
        if (x != Position.X || y != Position.Y)
        {
            Position = new PixelPoint(x, y);
        }

        vm.PositionX = x / scaling; 
        vm.PositionY = y / scaling;
    }

 

}