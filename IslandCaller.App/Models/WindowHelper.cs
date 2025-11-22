using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
public static class WindowHelper
{
    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd,int dwAttribute,ref int pvAttribute,int cbAttribute);
    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
        ACCENT_ENABLE_HOSTBACKDROP = 5
    }
    public enum DwmWindowCornerPreference
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public int Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AccentPolicy
    {
        public int AccentState;
        public int Flags;
        public int GradientColor; // AABBGGRR
        public int AnimationId;
    }

    public static void EnableBlur(Window window, int color)
    {
        var windowHelper = new WindowInteropHelper(window);
        var accent = new AccentPolicy
        {
            AccentState = (int)AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
            GradientColor = color // AABBGGRR
        };

        var size = Marshal.SizeOf(accent);
        var pointer = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(accent, pointer, false);

        var data = new WindowCompositionAttributeData
        {
            Attribute = 19,
            Data = pointer,
            SizeOfData = size
        };

        SetWindowCompositionAttribute(windowHelper.Handle, ref data);
        Marshal.FreeHGlobal(pointer);
    }

    public static void SetWindowCorner(Window window, DwmWindowCornerPreference preference)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        int pref = (int)preference;
        DwmSetWindowAttribute(hwnd, 33, ref pref, sizeof(int)); // DWMWA_WINDOW_CORNER_PREFERENCE = 33
    }



    public static void EnableFluentWindow(Window window, int color)
    {
        EnableBlur(window, color);
        SetWindowCorner(window, DwmWindowCornerPreference.DWMWCP_ROUND);
    }
}