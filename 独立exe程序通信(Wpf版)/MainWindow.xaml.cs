using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace 独立exe程序通信_Wpf版_
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        const int WM_COPYDATA = 0x004A; // 固定数值，不可更改
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; // 任意值
            public int cbData;    // 指定lpData内存区域的字节数
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData; // 发送给目标窗口所在进程的数据

        }
        //在DLL库中的发送消息函数
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
            (
            int hWnd,                         // 目标窗口的句柄  
            int Msg,                          // 在这里是WM_COPYDATA
            int wParam,                       // 第一个消息参数
            ref  COPYDATASTRUCT lParam        // 第二个消息参数
           );
        /// <summary>
        /// 发送方法
        /// </summary>
        /// <param name="s">内容</param>
        /// <param name="_PNAME">exe进程名称</param>
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
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.win_SourceInitialized(this, e);

        }
        void win_SourceInitialized(object sender, EventArgs e)
        {

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_COPYDATA)
            {

                COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT)); // 接收封装的消息

                string rece = cds.lpData; // 获取消息内容
                GetSendMess(rece);

            }
            return hwnd;
        }
        private void GetSendMess(string _str) {
            MessageBox.Show(_str,"wpf弹框");
            switch (_str)
            {
                case "值1":
                    break;
                default:
                    break;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MSend(textBox1.Text, "通信winform");
        }
    }
}
