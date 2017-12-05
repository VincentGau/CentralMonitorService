using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralMonitorService
{
    class AlarmSender
    {
        private static AlarmSender uniqueInstance;
        public static AlarmSender getInstance()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new AlarmSender();
            }
            return uniqueInstance;
        }

        public void OnMonitorDone(object source, MonitorEventArgs args)
        {
            Console.WriteLine("Sending alarm...");
            List<Website> failedSites = args.argList;
            StringBuilder sb = new StringBuilder();
            foreach(Website site in failedSites)
            {
                sb.Append(site.url);
            }
            Console.WriteLine("Failed sites: ", sb.ToString());
            Logger.Info("ALARM! Failed sites: " + sb.ToString());
        }
    }
}
