using IslandCaller.App.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
namespace IslandCaller.App.Views.Windows
{
    /// <summary>
    /// HoverFluent.xaml 的交互逻辑
    /// </summary>
    public partial class HoverFluent : System.Windows.Window
    {
        public HoverFluent()
        {
            InitializeComponent();
            this.Left = Models.Settings.Instance.Hover.Position.X;
            this.Top = Models.Settings.Instance.Hover.Position.Y;
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.EnableFluentWindow(this, 0x00FFFFFF); // 设置模糊 + 圆角
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                MoveWindow();
                Models.Settings.Instance.Hover.Position.X = this.Left;
                Models.Settings.Instance.Hover.Position.Y = this.Top;
            }
        }

        private void MainButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                MoveWindow();
                if (Models.Settings.Instance.Hover.Position.X == this.Left || Models.Settings.Instance.Hover.Position.Y == this.Top)
                {
                    MainButton_Click(sender, e);
                }
                else
                {
                    Models.Settings.Instance.Hover.Position.X = this.Left;
                    Models.Settings.Instance.Hover.Position.Y = this.Top;
                }
            }
        }

        private async void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainButton.IsEnabled = false;
            img.Opacity = 0.5;
            new IslandCallerNotificationProviderNew().RandomCall(1);
            await Task.Delay(3000);
            MainButton.IsEnabled = true;
            img.Opacity = 1.0;
        }
        private void SecondaryButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                MoveWindow();
                if (Models.Settings.Instance.Hover.Position.X == this.Left || Models.Settings.Instance.Hover.Position.Y == this.Top)
                {
                    SecondaryButton_Click(sender, e);
                }
                else
                {
                    Models.Settings.Instance.Hover.Position.X = this.Left;
                    Models.Settings.Instance.Hover.Position.Y = this.Top;
                }
            }
        }
        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {
            Secondary_Button.IsEnabled = false;
            new FluentCallerGUI().ShowDialog();
            Secondary_Button.IsEnabled = true;
        }

        private void MoveWindow()
        {
            DragMove();
            // 获取当前屏幕工作区
            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle);
            var workingArea = screen.WorkingArea;

            PresentationSource source = PresentationSource.FromVisual(this);
            Matrix transformToDevice = source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity;

            double scaledWidth = this.ActualWidth * transformToDevice.M11;
            double scaledHeight = this.ActualHeight * transformToDevice.M22;
            double scaledLeft = this.Left * transformToDevice.M11;
            double scaledTop = this.Top * transformToDevice.M22;
            double newLeft = this.Left;
            double newTop = this.Top;

            // 如果窗口左边界在屏幕左侧之外
            if (scaledLeft < workingArea.Left)
                newLeft = workingArea.Left;
            // 如果窗口右边界在屏幕右侧之外
            if (scaledLeft + scaledWidth > workingArea.Right)
                newLeft = (workingArea.Right - scaledWidth) / transformToDevice.M11;
            // 如果窗口上边界在屏幕上方之外
            if (scaledTop < workingArea.Top)
                newTop = workingArea.Top;
            // 如果窗口下边界在屏幕下方之外
            if (scaledTop + scaledHeight > workingArea.Bottom)
                newTop = (workingArea.Bottom - scaledHeight) / transformToDevice.M22;

            // 应用修正后的位置
            this.Left = newLeft;
            this.Top = newTop;
        }
    }

}
