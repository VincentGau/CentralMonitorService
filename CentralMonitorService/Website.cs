namespace CentralMonitorService
{

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
}
