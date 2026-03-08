using Avalonia.Controls;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace IslandCaller.Helpers
{
    public class WindowTopmostHelper
    {
        private readonly ILogger<WindowTopmostHelper> logger = IAppHost.GetService<ILogger<WindowTopmostHelper>>();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new(-1);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        public void EnsureTopmost(Window window)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("当前操作系统不是 Windows，跳过置顶调用。");
                return;
            }

            var platformHandle = window.TryGetPlatformHandle();
            if (platformHandle == null)
            {
                logger.LogWarning("无法获取窗口句柄，置顶失败。");
                return;
            }

            var hwnd = platformHandle.Handle;
            var success = SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();
                logger.LogWarning("SetWindowPos 调用失败，错误码: {ErrorCode}", errorCode);
                return;
            }

            logger.LogTrace("已通过 Win32 API 置顶窗口，句柄: {Hwnd}", hwnd);
        }
    }
}
