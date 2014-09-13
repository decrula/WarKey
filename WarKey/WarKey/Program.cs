using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WarKey
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isNotRunning = false;
            System.Threading.Mutex mu = new System.Threading.Mutex(true, "WarKeyRunning", out isNotRunning);

            if (isNotRunning)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmWarKey());
                mu.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("程序已在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
