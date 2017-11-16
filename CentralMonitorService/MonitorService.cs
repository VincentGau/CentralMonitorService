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
using System.Timers;
using static CentralMonitorService.MonitorCore;


namespace CentralMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        private static object reentryLock = new object();

        MonitorCore wd = MonitorCore.getInstance();

        public MonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("OnStart.");

            // 定时器启动时立即执行一次
            timer1_Elapsed(null, null);

        }       

        protected override void OnStop()
        {
            Logger.Info("Service Stopped.");
            timer1.Stop();
            timer1.Dispose();
        }


        /// <summary>
        /// 用以调试service
        /// </summary>
        /// <param name="args"></param>
        internal void DebugMode(string[] args)
        {
            OnStart(args);
            Console.ReadLine();
            Console.WriteLine("Done.");
            OnStop();
        }
        

        /// <summary>
        /// 定时执行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            //lock (reentryLock)  // 重入锁
            //{

            //    List<Website> allSites = wd.GetSiteList();
            //    List<Website> failedList = wd.checkSites(allSites);

            //    if (allSites.Any() == false)
            //    {
            //        Logger.Error("监控配置文件为空。");
            //        return;
            //    }

            //    if (failedList.Any() == false)
            //    {
            //        Logger.Info("所有站点正常。");
            //    }

            //    else
            //    {
            //        Logger.Info("有站点请求失败。");
            //        StringBuilder sb = new StringBuilder();
            //        foreach (Website site in failedList)
            //        {
            //            sb.Append(site.url + ". ");
            //        }

            //        Logger.Info(string.Format("Failed sites: {0}", sb.ToString()));
            //    }
            //}
            wd.checkJSH();
        }

        public void test()
        {
            List<Website> sl = new List<Website>();
            List<string> dl = new List<string>();
            List<string> spl = new List<string>();

            wd.getMonitorElements(out sl, out dl, out spl);

        }
    }
}
