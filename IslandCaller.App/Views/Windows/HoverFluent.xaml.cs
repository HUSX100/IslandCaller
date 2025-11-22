using IslandCaller.App.Models;
using IslandCaller.App.Controls.HoverFluent;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace IslandCaller.App.Views.Windows
{
    /// <summary>
    /// HoverFluent.xaml 的交互逻辑
    /// </summary>
    public partial class HoverFluent : Window
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
            await new ShowName().showstudent(1);
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
            //new FluentCallerGUI().ShowDialog();
            Secondary_Button.IsEnabled = true;
        }

        private void MoveWindow()
        {
            DragMove();

            // 获取显示器工作区（替代已弃用的 System.Windows.Forms.Screen）
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            MONITORINFO mi = new MONITORINFO();
            mi.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
            bool gotInfo = GetMonitorInfo(monitor, ref mi);

            PresentationSource source = PresentationSource.FromVisual(this);
            Matrix transformToDevice = source?.CompositionTarget?.TransformToDevice ?? Matrix.Identity;

            // 得到工作区的像素值（如果 GetMonitorInfo 失败，使用 SystemParameters.WorkArea 作为回退）
            double waLeftPx, waTopPx, waRightPx, waBottomPx;
            if (gotInfo)
            {
                waLeftPx = mi.rcWork.left;
                waTopPx = mi.rcWork.top;
                waRightPx = mi.rcWork.right;
                waBottomPx = mi.rcWork.bottom;
            }
            else
            {
                // SystemParameters.WorkArea 是设备无关单位（DIPs），把它转换为像素
                var wa = SystemParameters.WorkArea;
                waLeftPx = wa.Left * transformToDevice.M11;
                waTopPx = wa.Top * transformToDevice.M22;
                waRightPx = (wa.Left + wa.Width) * transformToDevice.M11;
                waBottomPx = (wa.Top + wa.Height) * transformToDevice.M22;
            }

            double scaledWidth = this.ActualWidth * transformToDevice.M11;
            double scaledHeight = this.ActualHeight * transformToDevice.M22;
            double scaledLeft = this.Left * transformToDevice.M11;
            double scaledTop = this.Top * transformToDevice.M22;
            double newLeft = this.Left;
            double newTop = this.Top;

            // 如果窗口左边界在屏幕左侧之外（比较像素）
            if (scaledLeft < waLeftPx)
                newLeft = waLeftPx / transformToDevice.M11;
            // 如果窗口右边界在屏幕右侧之外
            if (scaledLeft + scaledWidth > waRightPx)
                newLeft = (waRightPx - scaledWidth) / transformToDevice.M11;
            // 如果窗口上边界在屏幕上方之外
            if (scaledTop < waTopPx)
                newTop = waTopPx / transformToDevice.M22;
            // 如果窗口下边界在屏幕下方之外
            if (scaledTop + scaledHeight > waBottomPx)
                newTop = (waBottomPx - scaledHeight) / transformToDevice.M22;

            // 应用修正后的位置
            this.Left = newLeft;
            this.Top = newTop;
        }

        // Win32 interop: Monitor / MonitorInfo
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);
    }
}
