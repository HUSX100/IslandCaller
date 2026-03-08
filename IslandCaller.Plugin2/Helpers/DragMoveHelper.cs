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
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [Out] TOUCHINPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern void CloseTouchInputHandle(IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_NULL = 0x0000;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_TOUCH = 0x0240;
        private const int SC_MOVE = 0xF010;
        private const int HTCAPTION = 0x0002;
        private const int WH_GETMESSAGE = 3;
        private const uint PM_REMOVE = 0x0001;
        private const uint TOUCHEVENTF_MOVE = 0x0001;
        private const uint TOUCHEVENTF_DOWN = 0x0002;
        private const uint TOUCHEVENTF_UP = 0x0004;
        private const int MK_LBUTTON = 0x0001;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static readonly HookProc TouchMessageHookProc = TouchToMouseHookCallback;
        private static IntPtr _touchHookHandle = IntPtr.Zero;
        private static IntPtr _dragTargetHwnd = IntPtr.Zero;

        /// <summary>
        /// 异步开始拖动窗口，并在拖动结束后返回。
        /// 触控输入时，会将 TOUCH 消息转发为鼠标消息以兼容 DragMove。
        /// </summary>
        public async Task DragMoveAsync(Window window, PointerType pointerType = PointerType.Mouse)
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

            // 触控输入时安装消息钩子，将 WM_TOUCH 转换为鼠标消息。
            bool useTouchHook = pointerType == PointerType.Touch;
            if (useTouchHook)
            {
                InstallTouchHook(hwnd);
            }

            // 3. 释放鼠标捕获 (必须在 UI 线程)
            ReleaseCapture();

            try
            {
                // 4. 在后台线程调用阻塞的 SendMessage
                await Task.Run(() =>
                {
                    // 此处会阻塞直到用户松开鼠标
                    SendMessage(hwnd, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
                });
            }
            finally
            {
                // 拖动结束后移除钩子，避免影响其它交互。
                if (useTouchHook)
                {
                    UninstallTouchHook();
                }
            }

            logger.LogDebug("窗口拖动结束，SendMessage 已返回。");
        }

        private void InstallTouchHook(IntPtr hwnd)
        {
            if (_touchHookHandle != IntPtr.Zero)
            {
                return;
            }

            _dragTargetHwnd = hwnd;
            _touchHookHandle = SetWindowsHookEx(WH_GETMESSAGE, TouchMessageHookProc, IntPtr.Zero, GetCurrentThreadId());
            if (_touchHookHandle == IntPtr.Zero)
            {
                logger.LogWarning("安装 TOUCH 消息钩子失败，错误码: {ErrorCode}", Marshal.GetLastWin32Error());
            }
        }

        private static void UninstallTouchHook()
        {
            if (_touchHookHandle == IntPtr.Zero)
            {
                return;
            }

            UnhookWindowsHookEx(_touchHookHandle);
            _touchHookHandle = IntPtr.Zero;
            _dragTargetHwnd = IntPtr.Zero;
        }

        private static IntPtr TouchToMouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)PM_REMOVE && lParam != IntPtr.Zero)
            {
                MSG msg = Marshal.PtrToStructure<MSG>(lParam);
                if (msg.message == WM_TOUCH && msg.hwnd == _dragTargetHwnd)
                {
                    ForwardTouchToMouse(msg.hwnd, msg.wParam, msg.lParam);

                    // 已手动转发成鼠标消息，原 TOUCH 消息置空防止重复处理。
                    msg.message = WM_NULL;
                    Marshal.StructureToPtr(msg, lParam, false);
                }
            }

            return CallNextHookEx(_touchHookHandle, nCode, wParam, lParam);
        }

        private static void ForwardTouchToMouse(IntPtr hwnd, IntPtr wParam, IntPtr lParam)
        {
            int inputCount = LOWORD(wParam);
            if (inputCount <= 0)
            {
                return;
            }

            TOUCHINPUT[] inputs = new TOUCHINPUT[inputCount];
            if (!GetTouchInputInfo(lParam, inputCount, inputs, Marshal.SizeOf<TOUCHINPUT>()))
            {
                return;
            }

            try
            {
                foreach (TOUCHINPUT touch in inputs)
                {
                    // TOUCHINPUT 的坐标单位是 1/100 像素，需要先还原再转为客户区坐标。
                    POINT point = new()
                    {
                        X = touch.x / 100,
                        Y = touch.y / 100
                    };

                    ScreenToClient(hwnd, ref point);
                    IntPtr mouseLParam = MakeLParam(point.X, point.Y);

                    if ((touch.dwFlags & TOUCHEVENTF_DOWN) != 0)
                    {
                        PostMessage(hwnd, WM_LBUTTONDOWN, (IntPtr)MK_LBUTTON, mouseLParam);
                    }
                    else if ((touch.dwFlags & TOUCHEVENTF_MOVE) != 0)
                    {
                        PostMessage(hwnd, WM_MOUSEMOVE, (IntPtr)MK_LBUTTON, mouseLParam);
                    }
                    else if ((touch.dwFlags & TOUCHEVENTF_UP) != 0)
                    {
                        PostMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, mouseLParam);
                    }
                }
            }
            finally
            {
                CloseTouchInputHandle(lParam);
            }
        }

        private static int LOWORD(IntPtr value) => (ushort)((ulong)value & 0xFFFF);

        private static IntPtr MakeLParam(int low, int high)
            => (IntPtr)((high << 16) | (low & 0xFFFF));

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public POINT pt;
            public int lPrivate;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int x;
            public int y;
            public IntPtr hSource;
            public uint dwID;
            public uint dwFlags;
            public uint dwMask;
            public uint dwTime;
            public IntPtr dwExtraInfo;
            public uint cxContact;
            public uint cyContact;
        }
    }
}
