﻿using System.Runtime.InteropServices;

namespace IslandCaller.Models
{
    public static class Core
    {
        // Import the functions from the DLL
        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int RandomImport([MarshalAs(UnmanagedType.LPWStr)] string filename);

        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr SimpleRandom(int number);
        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool CreateHelloPasskey();
        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool VerifyHelloPasskey();

        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateTOTPUrl();
        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern bool VerifyTOTP([MarshalAs(UnmanagedType.LPWStr)] string user_code);
        [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern void ClearHistory();

        // Async wrapper
        public static Task<bool> CreateHelloPasskeyAsync()
        {
            return Task.Run(() => CreateHelloPasskey());
        }
        public static Task<bool> VerifyHelloPasskeyAsync()
        {
            return Task.Run(() => VerifyHelloPasskey());
        }
        public static Task<IntPtr> CreateTOTPUrlAsync()
        {
            Console.WriteLine("IslandCaller.Plugin | Info | Core.cs : Start to create TOTP url \n");
            return Task.Run(() => CreateTOTPUrl());
        }
        public static Task<bool> VerifyTOTPAsync(string user_code)
        {
            Console.WriteLine("IslandCaller.Plugin | Info | Core.cs : Start to verify TOTP usercode \n");
            Console.WriteLine("IslandCaller.Plugin | Info | Core.cs : Get usercode : " + user_code + " \n");
            return Task.Run(() => VerifyTOTP(user_code));
        }
    }
}
