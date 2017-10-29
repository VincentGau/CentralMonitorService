using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Net;

namespace CentralMonitorService
{
    class WebpageDetector
    {
        // 记录需要监控的站点的xml文件
        private static string sitesFile = "";

        /// <summary>
        /// Website 类，属性包括需要监控的站点的主机名，url
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
        /// 返回需要检测的站点列表
        /// </summary>
        /// <returns></returns>
        public List<Website> GetSiteList()
        {
            // 需要监控的站点列表
            List<Website> siteList = new List<Website>();
            sitesFile = ConfigurationManager.AppSettings["SitesFile"];

            if (!File.Exists(sitesFile))
            {
                throw new FileNotFoundException("找不到监控站点配置文件。");
            }

            // 加载站点配置文件
            XmlDocument doc = new XmlDocument();
            doc.Load(sitesFile);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodeList = root.GetElementsByTagName("website");
            foreach (XmlNode node in nodeList)
            {
                string hostname = ((XmlElement)node).GetAttribute("hostname");
                string url = ((XmlElement)node).GetAttribute("url");

                siteList.Add(new Website(hostname, url));
            }

            Console.WriteLine("获取站点列表完毕。");
            return siteList;
        }


        public void checkResponseCode(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Console.WriteLine(response.StatusCode);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void checkSites()
        {
            
            List<Website> siteList = GetSiteList();
            foreach (Website site in siteList)
            {
                checkResponseCode(site.url);
            }
        }
        
    }
}
