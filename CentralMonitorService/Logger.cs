using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;

namespace CentralMonitorService
{
    public class Logger
    {
        
        private static readonly ILog log;

        static Logger()
        {
            FileInfo logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Log.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            log = LogManager.GetLogger("MyLogger");
        }

        public static void Info(string message)
        {
            log.Info(message);
        }
    }
}
