using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ClassIsland.Shared;
using IslandCaller.Helpers;
using IslandCaller.Services.IslandCallerService;
using IslandCaller.Views;
using Microsoft.Extensions.Logging;

namespace IslandCaller.Controls;

public partial class HoverFluentControl : UserControl
{
    private IslandCallerService IslandCallerService { get; }
    private Window parentwindow { get; set; }
    private ILogger<HoverFluentControl> logger { get; set; }
    public PixelPoint lastWindowPosition { get; set; }
    private WindowDragHelper windowDragHelper { get; set; }
    private long _lastDragTime;
    public HoverFluentControl()
    {
        IslandCallerService = IAppHost.GetService<IslandCallerService>();
        logger = IAppHost.GetService<ILogger<HoverFluentControl>>();
        windowDragHelper = IAppHost.GetService<WindowDragHelper>();
        InitializeComponent();
        Button1.AddHandler(InputElement.PointerPressedEvent, Button1_PointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        Button2.AddHandler(InputElement.PointerPressedEvent, Button2_PointerPressed, RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
    }
    private async void Button1_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        e.Handled = true;
        if(Environment.TickCount64 - _lastDragTime < 50)
        {
            logger.LogDebug("捕获到重复的输入");
            _lastDragTime = Environment.TickCount64;
            return;
        }
        logger.LogDebug("Button1_PointerPressed: 触发窗口拖动");
        _lastDragTime = Environment.TickCount64;
        // 触发窗口拖动
        parentwindow = this.GetVisualRoot() as Window;
        lastWindowPosition = parentwindow.Position;
        await windowDragHelper.DragMoveAsync(parentwindow);
        logger.LogDebug("Button1_PointerPressed: 窗口拖动结束");
        if(parentwindow.Position == lastWindowPosition)
        {
            logger.LogDebug("Button1_PointerPressed: 窗口位置未变化，触发点击事件");
            IslandCallerService.ShowRandomStudent(1);
        }
    }
    private async void Button2_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        e.Handled = true;
        if (Environment.TickCount64 - _lastDragTime < 50)
        {
            logger.LogDebug("捕获到重复的输入");
            _lastDragTime = Environment.TickCount64;
            return;
        }
        logger.LogDebug("Button2_PointerPressed: 触发窗口拖动");
        _lastDragTime = Environment.TickCount64;
        // 触发窗口拖动
        parentwindow = this.GetVisualRoot() as Window;
        lastWindowPosition = parentwindow.Position;
        await windowDragHelper.DragMoveAsync(parentwindow);
        logger.LogDebug("Button2_PointerPressed: 窗口拖动结束");
        if (parentwindow.Position == lastWindowPosition)
        {
            logger.LogDebug("Button2_PointerPressed: 窗口位置未变化，触发点击事件");
            await new PersonalCall().ShowDialog(parentwindow);
        }
    }
}