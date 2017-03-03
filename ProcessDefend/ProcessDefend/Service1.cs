using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Cjwdev.WindowsApi;
using System.Runtime.InteropServices;
using System.IO;

namespace ProcessDefend
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("WindowsService: Service Started" + DateTime.Now.ToString());
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(AppStartEvent);//到达时间执行事件
            timer.AutoReset = true;//设置一直执行，如果为false，就执行一次
            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        protected override void OnStop()
        {
            WriteLog("WindowsService: Service Stopped" + DateTime.Now.ToString());
        }

        /// <summary>
        /// 应用程序启动事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void AppStartEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer timer = (System.Timers.Timer)source;
            timer.Enabled = false;
       
            string appName = "QQ";//the path of the exe file
            string appPath = @"C:\Program Files (x86)\Tencent\QQ\Bin\QQ.exe";//the path of the exe file
            bool runFlag = false;


            Process[] myProcesses = Process.GetProcesses();
            foreach (Process myProcess in myProcesses)
            {
                if (myProcess.ProcessName.CompareTo(appName) == 0)
                {
                    runFlag = true;
                }
            }

            if (!runFlag)
            {              
                AppStart(appPath);
                WriteLog("启动QQ.exe" + DateTime.Now.ToString());
            }
            else if (runFlag)
            {
                Process[] myPro = Process.GetProcessesByName(appName);
                myPro[0].Kill();       //删除进程       
                AppStart(appPath);
                WriteLog("重新启动QQ.exe" + DateTime.Now.ToString());
            }
            timer.Enabled = true;
        }

        /// <summary>
        /// 启动制定路径的应用程序
        /// </summary>
        /// <param name="appPath">应用程序所在路径</param>
        public void AppStart(string appPath)
        {
            try
            {

                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    "",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="log"></param>
        private void WriteLog(string log)
        {
            if (!Directory.Exists(@"d:\ProcessDefend"))
            {
                Directory.CreateDirectory(@"d:\ProcessDefend");
            }
            FileStream fs = new FileStream(@"d:\ProcessDefend\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine(log + "\n");
            sw.Flush();
            sw.Close();
            fs.Close();
        }
    }
}
