using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.NS_Utils
{
    class ConsoleHelper
    {
        [DllImport("kernel32.dll")] public static extern bool AttachConsole(int processID);
        [DllImport("kernel32.dll")] public static extern bool AllocConsole();
        [DllImport("kernel32.dll")] public static extern bool FreeConsole();
        [DllImport("kernel32.dll")] public static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll")] public static extern int GetConsoleOutputCP();
    }
}
