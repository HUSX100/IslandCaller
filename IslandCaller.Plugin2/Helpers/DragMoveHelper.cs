using Avalonia.Controls;
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

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 0x0002;

        /// <summary>
        /// 异步开始拖动窗口，并在拖动结束后返回
        /// </summary>
        public async Task DragMoveAsync(Window window)
        {
            // 1. 检查是否为 Windows 系统
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogInformation("当前操作系统不是 Windows，无法使用 DragMoveAsync 方法。");
                return;
            }

            // 2. 获取 Win32 句柄 (HWND)
            var platformHandle = window.TryGetPlatformHandle();
            if (platformHandle == null) return;

            IntPtr hwnd = platformHandle.Handle;
            logger.LogDebug($"获取窗口句柄: {hwnd}");

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
    }
}
