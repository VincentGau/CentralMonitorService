using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using static CentralMonitorService.WebpageDetector;
using System.Timers;

namespace CentralMonitorService
{
    public partial class MonitorService : ServiceBase
    {

        private static Timer timer; // 计时器

        public MonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //ServiceStart();

            SetTimer();
            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            Console.ReadLine();
            timer.Stop();
            timer.Dispose();

            Console.WriteLine("Terminating the application...");
        }

       

        protected override void OnStop()
        {
            ServiceStop();
        }


        /// <summary>
        /// 用以调试service
        /// </summary>
        /// <param name="args"></param>
        internal void DebugMode(string[] args)
        {
            OnStart(args);
            //Console.ReadLine();
            Console.WriteLine("Done.");
            OnStop();
        }

        /// <summary>
        ///     
        /// </summary>
        private static void ServiceStart()
        {
            Logger.Info("Service Start.");


            WebpageDetector wd = new WebpageDetector();
            List<Website> allSites = wd.GetSiteList();
            List<Website> failedList = wd.checkSites(allSites);

            if (failedList.Any() == false)
            {
                Logger.Info("所有站点正常。");
            }

            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (Website site in failedList)
                {
                    sb.Append(site.url + ". ");
                }

                Logger.Info(string.Format("Failed sites: {0}",sb.ToString()));
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void ServiceStop()
        {
            FileStream fs = new FileStream(@"c:\Workspace\xxxx.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("WindowsService: Service Stopped" + DateTime.Now.ToString() + "\n");
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        private static void SetTimer()
        {
            // 每隔五分钟执行
            timer = new Timer(1000*60*1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            string str = string.Format("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
            Console.WriteLine(str);
            Logger.Info(str);
            ServiceStart();
        }


    }
}
