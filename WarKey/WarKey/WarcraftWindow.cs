using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WarKey
{
    public static class WarcraftWindow
    {
        private static readonly uint WM_KEYDOWN = 0x100;
        private static readonly uint WM_KEYUP = 0x101;
        private static readonly int KEY_QUOTLEFT = 219;   // [ 
        private static readonly int KEY_QUOTRIGHT = 221;  // ] 

        // 用来在访问令牌中启用和禁用一个权限
        private static readonly int TOKEN_ADJUST_PRIVILEGES = 0x20;
        // 用来查询一个访问令牌
        private static readonly int TOKEN_QUERY = 0x8;
        // 权限启用标志
        private static readonly int SE_PRIVILEGE_ENABLED = 0x2;

        //private static readonly int PROCESS_ALL_ACCESS = 0x1F0FFF;
        private static readonly int PROCESS_VM_READ = 0x0010;
        private static readonly int PROCESS_VM_WRITE = 0x0020;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        //[DllImport("user32.dll")]
        //public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);

        //提升权限所需的api
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValueA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);
        [DllImport("advapi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, ref TOKEN_PRIVILEGES PreviousState, ref int ReturnLength);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        //读取内存
        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, IntPtr lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern void CloseHandle(IntPtr hObject);

        #region 提升权限所需的结构体
        public struct LUID
        {
            // 本地唯一标志的低32位
            public int LowPart;
            // 本地唯一标志的高32位
            public int HighPart;
        }

        public struct LUID_AND_ATTRIBUTES
        {
            public LUID pLuid;
            // 指定了LUID的属性，其值可以是一个32位大小的bit 标志
            public int Attributes;
        }

        public struct TOKEN_PRIVILEGES
        {
            // 包含了一个访问令牌的一组权限信息：即该访问令牌具备的权限
            // 指定了权限数组的容量
            public int PrivilegeCount;
            // 指定一组的LUID_AND_ATTRIBUTES 结构，每个结构包含了LUID和权限的属性
            public LUID_AND_ATTRIBUTES Privileges;
        }
        #endregion


        /// <summary>
        /// 魔兽窗口是否存在 
        /// </summary>
        public static bool Exisits
        {
            get { return FindWindow(null, "Warcraft III") != IntPtr.Zero; }
        }
        
        /// <summary>
        /// 魔兽窗口是否前焦
        /// </summary>
        public static bool IsForeground
        {
            get
            {
                IntPtr war3 = FindWindow(null, "Warcraft III");
                return war3 == GetForegroundWindow();
            }
        }

        /// <summary>
        /// 魔兽窗口是否聊天状态
        /// </summary>
        public static bool IsChating
        {
            get
            {
                // 读取聊天状态时所改的内存地址里的值，开启聊天为1，关闭为0，预设0
                int isChating = 0;
                IntPtr tokenHandle = IntPtr.Zero;
                LUID privilegeLUID = new LUID();
                TOKEN_PRIVILEGES newPrivileges = new TOKEN_PRIVILEGES();
                TOKEN_PRIVILEGES tokenPrivileges = default(TOKEN_PRIVILEGES);
                OpenProcessToken(Process.GetCurrentProcess().Handle, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref tokenHandle);
                LookupPrivilegeValue("", "SeDebugPrivilege", ref privilegeLUID);

                tokenPrivileges.PrivilegeCount = 1;
                tokenPrivileges.Privileges.Attributes = SE_PRIVILEGE_ENABLED;
                tokenPrivileges.Privileges.pLuid = privilegeLUID;
                int Size = 4;
                IntPtr dota = FindWindow(null, "Warcraft III");
                if ((AdjustTokenPrivileges(tokenHandle, 0, ref tokenPrivileges, 4 + (12 * tokenPrivileges.PrivilegeCount), ref newPrivileges, ref Size)) != 0)
                {
                    int dotaID;
                    IntPtr dotaProcess;
                    GetWindowThreadProcessId(dota, out dotaID);
                    dotaProcess = OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE, false, dotaID);
                    byte[] buffer = new byte[4];
                    IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);     //获取缓冲区地址
                    ReadProcessMemory(dotaProcess, 0x6FAE8450, byteAddress, 4, IntPtr.Zero);    //将指定内存中的值读入缓冲区
                    CloseHandle(dotaProcess);
                    isChating = Marshal.ReadInt32(byteAddress);
                }
                return isChating == 1;
            }
        }

        /// <summary>
        /// 发送键值（聊天屏蔽）
        /// </summary>
        /// <param name="keyValue"></param>
        public static void Send(int keyValue)
        {
            IntPtr war3 = FindWindow(null, "Warcraft III");

            if (war3 == IntPtr.Zero || IsChating)
                return;

            SendMessage(war3, WM_KEYDOWN, keyValue, 0);
            SendMessage(war3, WM_KEYUP, keyValue, 0);
        }

        /// <summary>
        /// 显示敌方血量
        /// </summary>
        public static void DisplayEnemysHP()
        {
            IntPtr war3 = FindWindow(null, "Warcraft III");

            if (war3 == IntPtr.Zero)
                return;

            SendMessage(war3, WM_KEYDOWN, KEY_QUOTRIGHT, 0);
        }

        /// <summary>
        /// 显示友方血量
        /// </summary>
        public static void DisplayAlliesHP()
        {
            IntPtr war3 = FindWindow(null, "Warcraft III");

            if (war3 == IntPtr.Zero)
                return;

            SendMessage(war3, WM_KEYDOWN, KEY_QUOTLEFT, 0);
        }
    }
}
