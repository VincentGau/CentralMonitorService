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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading;

namespace CentralMonitorService
{
    public class MonitorEventArgs : EventArgs
    {
        public List<Website> argList { get; set; }
    }

    partial class MonitorCore
    {
        // 记录需要监控的站点的xml文件
        private static string sitesFile = "";

        // MonitorCore 实例
        private static MonitorCore uniqueInstance;

        // 事件委托
        public event EventHandler<MonitorEventArgs> MonitorDone;

        // 异常站点
        private List<Website> failedSiteList = new List<Website>();

        protected virtual void OnMonitorDone(List<Website> siteList)
        {
            MonitorDone?.Invoke(this, new MonitorEventArgs() { argList = siteList });
        }

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

        

        public void DoMonitor()
        {

            //List<Website> failedSites = checkSites(GetSiteList());

            List<Website> siteList = new List<Website>();
            List<string> dbList = new List<string>();
            List<string> reservedList = new List<string>();

            getMonitorElements(out siteList, out dbList, out reservedList);

            Parallel.ForEach(siteList, (i) =>
            {
                checkResponseCodeParallel(i);
            });

            OnMonitorDone(failedSiteList);
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
            string retString = null;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    retString = response.StatusCode.ToString();
                }
                //Console.WriteLine(response.StatusCode);
                return retString;
            }
            catch (WebException ex)
            {
                //Console.WriteLine(ex.ToString());
                Logger.Error(string.Format("请求站点异常：{0}. {1}", url, ex.ToString()));
                retString = "Request failed.";
            }
            return retString;
        }

        public void checkResponseCodeParallel(Website site)
        {
            
            string retString = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(site.url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    retString = response.StatusCode.ToString();
                    if(!retString.Equals("OK"))
                    {
                        failedSiteList.Add(site);
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.Error(string.Format("请求站点异常：{0}. {1}", site.url, ex.ToString()));
                failedSiteList.Add(site);
            }
            //catch(Exception ex)
            //{
            //    Logger.Error(string.Format("请求站点异常：{0}. {1}", site.url, ex.ToString()));
            //}
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

        /// <summary>
        /// 异步方式发送HTTP 请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> getResponseAsync(string url)
        {
            string result = null;
            try
            {
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                result = response.StatusCode.ToString();
            }
            catch(Exception ex)
            {
                Logger.Error("站点请求异常。", ex);
                result = "Request failed.";
            }
            return result;
            
        }

        /// <summary>
        /// 异步方式完成大量HTTP 请求
        /// </summary>
        /// <param name="siteList"></param>
        /// <returns></returns>
        public async Task<List<Website>> checkSitesAsync(List<Website> siteList)
        {
            List<Website> failedList = new List<Website>();
            foreach (Website site in siteList)
            {
                string resCode = await getResponseAsync(site.url);
                if (!resCode.Equals("OK"))
                {
                    failedList.Add(site);
                }
            }
            return failedList;
        }



        public bool connectDB(string ip, string dbName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection())
                {
                    conn.ConnectionString = "Data Source=localhost;Initial Catalog=HakuTest;Integrated Security=True";
                    conn.Open();
                    return true;
                }                
            }

            catch (Exception ex)
            {
                Logger.Error(string.Format("数据库连接异常: {0}. {1}，错误信息：{2}", ip, dbName, ex.ToString()));
                Logger.Error("BLABLABLA...", ex);
            }

            return false;
            
        }

        public string checkJSH()
        {
            string url = "http://ewealth.abchina.com/app/data/api/DataService/ExchangeRateV2";
            string result = "Failed";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream respStream = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream, Encoding.GetEncoding("UTF-8")))
                    {
                        string htmlContent = sr.ReadToEnd();
                        //Console.WriteLine(htmlContent);
                        JObject jo = (JObject)JsonConvert.DeserializeObject(htmlContent);
                        string jshTime = jo["Data"]["Table"][0]["PublishTime"].ToString();
                        //Console.WriteLine(jshTime);

                        DateTime dt = Convert.ToDateTime(jshTime);
                        DateTime t1 = DateTime.Now;
                        TimeSpan ts = t1 - dt;
                        Console.WriteLine(ts.TotalHours);
                        if(ts.TotalHours < 2)
                        {
                            result = "Normal";
                            Logger.Info("结售汇牌价更新及时");
                        }
                        else
                        {
                            result = "Not in time";
                            Logger.Warn("结售汇牌价更新不及时.");
                        }
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine("返回的Json数据里找不到结售汇牌价更新时间.");
                Console.WriteLine(ex.ToString());
                Logger.Error("返回的Json数据里找不到结售汇牌价更新时间.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Logger.Error("获取结售汇更新时间失败。", ex);
            }


            return result;
        }

        /// <summary>
        /// DEPRECATED
        /// 检查结售汇页面更新时间，成功返回Success字符串，否则返回错误信息字符串(页面弃用)
        /// </summary>
        /// <returns></returns>
        public string checkRatePage()
        {
            string url = "http://app.abchina.com/rateinfo/ratesearch.aspx?id=1";
            string result = "";
            string dateString = "";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            try
            {
                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // html片段：<label id="Label2">每百单位外币兑换人民币   更新日期:2017年11月02日 09时18分</label></div>
                    Stream respStream = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream, Encoding.GetEncoding("UTF-8")))
                    {
                        string htmlContent = sr.ReadToEnd();
                        //Console.WriteLine(htmlContent);
                        string expr = @"更新日期:(.*?)</label>";
                        MatchCollection mc = Regex.Matches(htmlContent, expr);
                        if (mc is null)
                        {
                            Logger.Error("结售汇页面找不到更新时间或页面格式已改变。");
                            result = "结售汇页面找不到更新时间或页面格式已改变。";
                            return result;
                        }
                        foreach(Match m in mc)
                        {
                            GroupCollection group = m.Groups;
                            dateString = group[1].Value;
                            Console.WriteLine(dateString);
                        }
                    }
                }

                DateTime dt = DateTime.ParseExact(dateString, "yyyy年MM月dd日 hh时mm分", null);
                DateTime t1 = DateTime.Now;
                TimeSpan ts = t1 - dt;

                Console.WriteLine(dt);
                Console.WriteLine(ts.TotalHours);

                return "Success";

            }
            catch (WebException ex)
            {
                Logger.Error(string.Format("请求结售汇站点异常：{0}.", ex.ToString()));
                return "请求结售汇站点异常。";
            }

            catch(FormatException ex)
            {
                Logger.Error("结售汇更新时间转换异常。", ex);
                return "结售汇更新时间转换异常。";
            }

            catch(Exception ex)
            {
                Logger.Error("获取结售汇更新时间异常。", ex);
                return "获取结售汇更新时间异常。";
            }
        }
        
    }
}
