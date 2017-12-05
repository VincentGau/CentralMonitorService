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

        MonitorCore monitorCore = MonitorCore.getInstance();
        AlarmSender alarmSender = AlarmSender.getInstance();

        public MonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("OnStart.");

            monitorCore.MonitorDone += alarmSender.OnMonitorDone;

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
            lock (reentryLock)  // 重入锁
            {
                monitorCore.DoMonitor();
            }
            
        }
    }
}
