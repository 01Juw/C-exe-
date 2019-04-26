using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace 独立exe程序通信
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; // 任意值
            public int cbData;    // 指定lpData内存区域的字节数
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData; // 发送给目标窗口所在进程的数据

        }

        public const int WM_COPYDATA = 0x004A;

        //通过窗口的标题来查找窗口的句柄 
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        //在DLL库中的发送消息函数
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
            (
            int hWnd,                         // 目标窗口的句柄  
            int Msg,                          // 在这里是WM_COPYDATA
            int wParam,                       // 第一个消息参数
            ref  COPYDATASTRUCT lParam        // 第二个消息参数
           );
        //接收消息方法
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));
                MessageBox.Show(cds.lpData.ToString(),"winform弹框");
            }
            base.WndProc(ref m);
        }
        //发送消息方法
        private void MSend(string s, string _PNAME)
        {
            string Mproc = "";
            Process[] procs = System.Diagnostics.Process.GetProcesses();
            foreach (Process p in procs)
            {
                Mproc += p.ProcessName + "\r\n";
                if (p.ProcessName.Equals(_PNAME))
                {
                    // 获取目标进程句柄
                    IntPtr hWnd = p.MainWindowHandle;
                    // 封装消息
                    byte[] sarr = System.Text.Encoding.Default.GetBytes(s);
                    int len = sarr.Length;
                    COPYDATASTRUCT cds2;
                    cds2.dwData = (IntPtr)0;
                    cds2.cbData = len + 1;
                    cds2.lpData = s;
                    // 发送消息
                    SendMessage((int)hWnd, WM_COPYDATA, 0, ref cds2);

                }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            MSend(textBox1.Text, "通信wpf");
        }
    }
}
