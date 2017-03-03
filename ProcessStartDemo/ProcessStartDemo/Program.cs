using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessStartDemo
{
    class Program
    {
        static void Main(string[] args)
        {
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

            if (!runFlag)   //如果程序没有启动
            {
                Process proc = new Process();
                proc.StartInfo.FileName = appName;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(appPath);
                proc.Start();


            }
            else if (runFlag)   //如果程序已经启动
            {
                Process[] myPro = Process.GetProcessesByName(appName);
                myPro[0].Kill();       //删除进程       

                Process proc = new Process();
                proc.StartInfo.FileName = appName;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(appPath);
                proc.Start();

            }
        }
    }
}
