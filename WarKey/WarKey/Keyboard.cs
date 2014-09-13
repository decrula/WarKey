using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WarKey
{
    public class Keyboard
    {
        private const int WM_KEYDOWN = 0x100;   //按下消息  
        private const int WM_KEYUP = 0x101;     //松开消息  
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private IKeyDownEventHandler handler;

        public Keyboard(IKeyDownEventHandler handler)
        {
            this.handler = handler;
            Start();
        }

        private static IntPtr hKeyboardHook = IntPtr.Zero;

        // 键盘常量  
        public const int WH_KEYBOARD_LL = 13;

        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        // 声明键盘钩子事件类型  
        private HookProc KeyboardHookProcedure;

        /// <summary>  
        /// 声明键盘钩子的封送结构类型  
        /// </summary>  
        [StructLayout(LayoutKind.Sequential)]
        public class KeyboardHookStruct
        {
            public int vkCode;      // 表示一个1到254间的虚拟键盘码  
            public int scanCode;    // 表示硬件扫描码  
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        // 安装钩子  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        // 下一个钩子  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, Int32 wParam, IntPtr lParam);
        // 卸载钩子  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int UnhookWindowsHookEx(IntPtr hhk);

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

            KeyboardHookStruct MyKBHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

            if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN)
            {
                Keys keyData = (Keys)MyKBHookStruct.vkCode;
                KeyEventArgs e = new KeyEventArgs(keyData);

                if (handler.Handle(e) == false)
                    return 1;
            }

            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        public void Start()
        {
            if (hKeyboardHook == IntPtr.Zero)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);

                using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
                using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
                    hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(curModule.ModuleName), 0);

                if (hKeyboardHook == IntPtr.Zero)
                    Stop();
            }
        }

        public void Stop()
        {
            if (hKeyboardHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = IntPtr.Zero;
            }
        }

        // 析构函数中卸载钩子
        ~Keyboard()
        {
            Stop();
        }
    }
}