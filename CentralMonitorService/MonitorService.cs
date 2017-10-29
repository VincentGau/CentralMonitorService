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

namespace CentralMonitorService
{
    public partial class MonitorService : ServiceBase
    {
        public MonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ServiceStart();
        }

       

        protected override void OnStop()
        {
            ServiceStop();
        }


        /// <summary>
        /// 调试service用
        /// </summary>
        internal void DebugStart()
        {
            ServiceStart();
        }

        /// <summary>
        /// 调试service用
        /// </summary>
        internal void DebugStop()
        {
            ServiceStop();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ServiceStart()
        {
            Logger.Info("Lalala");


            WebpageDetector wd = new WebpageDetector();
            wd.checkSites();
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


    }
}
