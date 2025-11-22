using IslandCaller.App.Views.Windows;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IslandCaller.App.Models
{
    internal class ShowName
    {
        public async Task showstudent(int num)
        {
            IntPtr ptr1 = Core.SimpleRandom(num);
            string name = Marshal.PtrToStringBSTR(ptr1);
            Marshal.FreeBSTR(ptr1); // 释放分配的 BSTR 内存
            Window shower = new FluentShower(name);
            shower.Show();
            await Task.Delay(3000 * num);
            shower.Close();
        }
    }
}
