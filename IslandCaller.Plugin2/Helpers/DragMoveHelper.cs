using Avalonia.Controls;
using Avalonia.Input;
using ClassIsland.Shared;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace IslandCaller.Helpers
{
    public class WindowDragHelper
    {
        public ILogger<WindowDragHelper> logger = IAppHost.GetService<ILogger<WindowDragHelper>>();

        // --- Win32 API 导入 ---
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterTouchWindow(IntPtr hWnd);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 0x0002;
        private const int ERROR_INVALID_PARAMETER = 87;
        private static readonly object TouchDisableLock = new();
        private static readonly HashSet<IntPtr> TouchDisabledWindows = new();

        /// <summary>
        /// 异步开始拖动窗口，并在拖动结束后返回。
        /// </summary>
        public async Task DragMoveAsync(Window window, PointerType pointerType)
        {
            // 1. 检查是否为 Windows 系统
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("当前操作系统不是 Windows，无法使用 DragMoveAsync 方法。");
                return;
            }

            // 2. 获取 Win32 句柄 (HWND)
            var platformHandle = window.TryGetPlatformHandle();
            if (platformHandle == null)
            {
                return;
            }

            IntPtr hwnd = platformHandle.Handle;
            logger.LogDebug("获取窗口句柄: {Hwnd}", hwnd);

            if (pointerType == PointerType.Touch || pointerType == PointerType.Pen)
            {
                EnsureTouchInputDisabled(hwnd);
            }

            // 3. 释放鼠标捕获 (必须在 UI 线程)
            ReleaseCapture();

            // 4. 在后台线程调用阻塞的 SendMessage
            await Task.Run(() =>
            {
                // 此处会阻塞直到用户松开鼠标
                SendMessage(hwnd, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
            });

            logger.LogDebug("窗口拖动结束，SendMessage 已返回。");
        }

        public void EnsureTouchInputDisabled(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                return;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("当前操作系统不是 Windows，跳过触控输入取消注册。");
                return;
            }

            lock (TouchDisableLock)
            {
                if (TouchDisabledWindows.Contains(hwnd))
                {
                    return;
                }

                TouchDisabledWindows.Add(hwnd);
            }

            if (!UnregisterTouchWindow(hwnd))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == ERROR_INVALID_PARAMETER)
                {
                    logger.LogInformation("窗口未注册触控输入，无需取消注册。句柄: {Hwnd}", hwnd);
                    return;
                }

                logger.LogWarning("取消窗口触控输入失败，错误码: {ErrorCode}, 句柄: {Hwnd}", errorCode, hwnd);

                lock (TouchDisableLock)
                {
                    TouchDisabledWindows.Remove(hwnd);
                }

                return;
            }

            logger.LogInformation("已取消窗口触控输入注册，句柄: {Hwnd}", hwnd);
        }
    }
}
