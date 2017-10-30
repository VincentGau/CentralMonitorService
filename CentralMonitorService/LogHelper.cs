using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CentralMonitorService
{
    

    public static class LogHelper
    {
        private static readonly ILog logger = LogManager.GetLogger("MyLogger");

        public static void Info(string msg)
        {
            logger.Info(msg);
        }
    }
}
