using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;
using System.Configuration;

namespace CentralMonitorService
{
    public class Logger
    {
        private static ILog logger;

        static Logger()
        {
            Init();
        }

        /// <summary>
        /// 初始化日志配置
        /// </summary>
        private static void Init()
        {
            // 日志配置文件
            string logConfigFile = ConfigurationManager.AppSettings["LogConfigFile"];
            // logger名称
            string loggerName = ConfigurationManager.AppSettings["LoggerName"];

            if (string.IsNullOrEmpty(logConfigFile) || string.IsNullOrEmpty(loggerName))
            {
                //Console.WriteLine("没有设置日志配置文件或没有设置logger名称！");
                throw new ArgumentException("没有设置日志配置文件或没有设置logger名称！");
            }

            string filePath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, logConfigFile);
            if (!File.Exists(filePath))
            {
                //Console.WriteLine("找不到日志配置文件！");
                throw new FileNotFoundException("找不到日志配置文件！");
            }

            logger = LogManager.GetLogger(loggerName);

        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        public static void Warn(string msg)
        {
            logger.Warn(msg);
        }

        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        public static void Fatal(string msg)
        {
            logger.Fatal(msg);
        }

        public static void Info(string msg, Exception e)
        {
            logger.Info(msg, e);
        }

        public static void Error(string msg, Exception e)
        {
            logger.Error(msg, e);
        }

        public static void Warn(string msg, Exception e)
        {
            logger.Warn(msg, e);
        }

        public static void Debug(string msg, Exception e)
        {
            logger.Debug(msg, e);
        }

        public static void Fatal(string msg, Exception e)
        {
            logger.Fatal(msg, e);
        }
    }
}
