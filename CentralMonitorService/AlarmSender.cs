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
        }
    }
}
