using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CentralMonitorService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            
            if (Environment.UserInteractive)
            {
                MonitorService s = new MonitorService();
                s.DebugStart();
                Console.Read();
                s.DebugStop();
            }

            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MonitorService()
                };
                ServiceBase.Run(ServicesToRun);
            }

            
        }
    }
}
