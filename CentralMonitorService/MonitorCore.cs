using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Net;
using System.Data.SqlClient;

namespace CentralMonitorService
{
    class MonitorCore
    {
        // 记录需要监控的站点的xml文件
        private static string sitesFile = "";

        private static MonitorCore uniqueInstance;

        private MonitorCore()
        {

        }

        public static MonitorCore getInstance()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new MonitorCore();
            }
            return uniqueInstance;
        }

        /// <summary>
        /// Website 类，属性包括需要监控的站点的主机名，url，需要监控可用性的网页
        /// </summary>
        public class Website
        {
            public Website(string hostname, string url)
            {
                this.hostname = hostname;
                this.url = url;
            }
            public string hostname { get; set; }
            public string url { get; set; }
        }

       
        /// <summary>
        /// 获取需要监控的要素，包括监控的网页可用性，数据库连接，页面内容
        /// </summary>
        /// <param name="siteList"></param>
        /// <param name="dbList"></param>
        /// <param name="spSiteList"></param>
        public void getMonitorElements(out List<Website> siteList, out List<string> dbList, out List<string> spSiteList)
        {
            List<Website> sl = new List<Website>();
            List<string> dl = new List<string>();
            List<string> spl = new List<string>();

            siteList = sl;
            dbList = dl;
            spSiteList = spl;

            Console.WriteLine("asdfg");
        }


        /// <summary>
        /// 返回需要检测的站点列表
        /// </summary>
        /// <returns></returns>
        public List<Website> GetSiteList()
        {
            // 需要监控的站点列表
            List<Website> siteList = new List<Website>();

            Logger.Info("开始获取站点列表...");

            sitesFile = ConfigurationManager.AppSettings["SitesFile"];

            string filePath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, sitesFile);
            if (!File.Exists(filePath))
            {
                Logger.Error("找不到监控站点配置文件。");
                throw new FileNotFoundException("找不到监控站点配置文件。");
            }
            Logger.Info(filePath);

            // 加载站点配置文件
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlElement root = doc.DocumentElement;
            Logger.Info(root.Name);
            XmlNodeList nodeList = root.GetElementsByTagName("website");
            foreach (XmlNode node in nodeList)
            {
                string hostname = ((XmlElement)node).GetAttribute("hostname");
                string url = ((XmlElement)node).GetAttribute("url");

                Logger.Info(hostname + url);

                siteList.Add(new Website(hostname, url));
            }

            //Console.WriteLine("获取站点列表完毕。");
            Logger.Info("获取站点列表完毕。");
            
            return siteList;

        }


        /// <summary>
        /// 检查单个url 的返回码
        /// </summary>
        /// <param name="url"></param>
        public string checkResponseCode(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Console.WriteLine(response.StatusCode);
                return response.StatusCode.ToString();
            }
            catch (WebException ex)
            {
                //Console.WriteLine(ex.ToString());
                Logger.Error(string.Format("{0}. {1}", url, ex.ToString()));
                return "Request failed.";
            }
            

        }


        /// <summary>
        /// 检查站点返回码，返回请求失败的站点
        /// </summary>
        /// <param name="siteList">需要访问的所有站点</param>
        /// <returns>返回请求失败的站点</returns>
        public List<Website> checkSites(List<Website> siteList)
        {
            List<Website> failedList = new List<Website>();
            foreach (Website site in siteList)
            {
                string resCode = checkResponseCode(site.url);
                if (!resCode.Equals("OK"))
                {
                    failedList.Add(site);
                }
            }
            return failedList;

        }

        public bool connectDB()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "";
                conn.Open();
            }
                

            return true;
        }
        
    }
}
