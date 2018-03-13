using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib.Helper;
using MessageAgent.Helper.Jobs;

namespace MessageAgent
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            JobLauncher.StartUp();
            Console.WriteLine("程式已啟動...");
            //Application.Run();
            while (true)
            {
                Console.WriteLine("請輸入=>r:立即執行; q:結束");
                var cmd = Console.ReadLine();
                if (cmd == "r")
                {
                    JobScheduler.LaunchImmediately();
                }
                else if(cmd=="q")
                {
                    break;
                }
                //Thread.Yield();
            }
        }
    }
}
