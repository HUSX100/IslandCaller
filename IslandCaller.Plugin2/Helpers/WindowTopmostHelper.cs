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

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static readonly IntPtr HWND_TOPMOST = new(-1);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const int GWL_EXSTYLE = -20;
        private const long WS_EX_NOACTIVATE = 0x08000000L;

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

        public void EnsureNoActivate(Window window)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("当前操作系统不是 Windows，跳过禁用激活调用。");
                return;
            }

            var platformHandle = window.TryGetPlatformHandle();
            if (platformHandle == null)
            {
                logger.LogWarning("无法获取窗口句柄，禁用激活失败。");
                return;
            }

            var hwnd = platformHandle.Handle;
            var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
            if (exStyle == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    logger.LogWarning("GetWindowLongPtr 调用失败，错误码: {ErrorCode}", errorCode);
                    return;
                }
            }

            var newStyleValue = new IntPtr(exStyle.ToInt64() | WS_EX_NOACTIVATE);
            var setResult = SetWindowLongPtr(hwnd, GWL_EXSTYLE, newStyleValue);
            if (setResult == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    logger.LogWarning("SetWindowLongPtr 调用失败，错误码: {ErrorCode}", errorCode);
                    return;
                }
            }

            logger.LogTrace("已通过 Win32 API 禁用窗口激活，句柄: {Hwnd}", hwnd);
        }

        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size == 8
                ? GetWindowLongPtr64(hWnd, nIndex)
                : new IntPtr(GetWindowLong32(hWnd, nIndex));
        }

        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr newValue)
        {
            return IntPtr.Size == 8
                ? SetWindowLongPtr64(hWnd, nIndex, newValue)
                : new IntPtr(SetWindowLong32(hWnd, nIndex, newValue.ToInt32()));
        }
    }
}
