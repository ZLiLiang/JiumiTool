using JiumiTool2.IServices;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;
using Titanium.Web.Proxy;
using System.Net;
using Titanium.Web.Proxy.EventArguments;
using Newtonsoft.Json;
using System.Text;
using JiumiTool2.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace JiumiTool2.Services
{
    public class HttpsProxyService : IHttpsProxyService, IDisposable
    {
        private bool _isDisposed = false;
        private readonly ProxyServer _proxyServer;
        private readonly ExplicitProxyEndPoint _explicitProxyEndPoint;
        private readonly string _injectionScript = $"{Environment.CurrentDirectory}/Script/InjectionScript.js";

        public HttpsProxyService()
        {
            _explicitProxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);
            _proxyServer = new ProxyServer();
            //使用Windows证书生成引擎
            _proxyServer.CertificateManager.CertificateEngine = CertificateEngine.DefaultWindows;
            _proxyServer.CertificateManager.RootCertificateName = "Jiumi";
            //启用证书
            _proxyServer.CertificateManager.EnsureRootCertificate();
        }

        public void Start()
        {
            //绑定需要的方法
            _explicitProxyEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;
            _proxyServer.BeforeRequest += OnBeforeRequest;
            _proxyServer.BeforeResponse += OnBeforeResponse;

            //启动代理
            _proxyServer.Start();
            _proxyServer.AddEndPoint(_explicitProxyEndPoint);
            //设置系统代理
            _proxyServer.SetAsSystemHttpsProxy(_explicitProxyEndPoint);
        }

        public void Stop()
        {
            //解绑需要的方法
            _explicitProxyEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            _proxyServer.BeforeRequest -= OnBeforeRequest;
            _proxyServer.BeforeResponse -= OnBeforeResponse;

            //停止代理
            _proxyServer.Stop();
            //禁止系统代理
            _proxyServer.DisableSystemHttpsProxy();
        }

        #region 资源释放

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                }

                // 停止代理服务
                if (_proxyServer != null && _proxyServer.ProxyRunning == true)
                {
                    Stop();
                }

                _isDisposed = true;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 拦截本地向代理服务器发送请求信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnBeforeRequest(object sender, SessionEventArgs e)
        {
            await Task.Run(async () =>
            {
                //请求链接
                var requestUri = e.HttpClient.Request.RequestUri.AbsoluteUri;
                //请求类型
                var requestType = e.HttpClient.Request.Method;
                //判断请求地址，获取注入拿到的结果
                if (requestUri.Contains("http://127.0.0.1:8000/") && requestType.Equals("POST"))
                {
                    var requestBytes = await e.GetRequestBody();
                    var requestString = Encoding.UTF8.GetString(requestBytes);
                    var injectionResult = JsonConvert.DeserializeObject<InjectionResult>(requestString);
                    WeakReferenceMessenger.Default.Send<InjectionResult, string>(injectionResult, "injectionResult");
                }
            });
        }

        /// <summary>
        /// 拦截代理服务器向本地的响应信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnBeforeResponse(object sender, SessionEventArgs e)
        {
            await Task.Run(async () =>
            {
                //请求链接
                var requestUri = e.HttpClient.Request.RequestUri.AbsoluteUri;
                //响应状态
                var statusCode = e.HttpClient.Response.StatusCode;

                if (requestUri.Contains("channels.weixin.qq.com") && statusCode == 200)
                {
                    // 获取响应正文
                    var responseBytes = await e.GetResponseBody();
                    var responseString = Encoding.UTF8.GetString(responseBytes);

                    // 读取本地JS文件内容
                    var injectionScript = await System.IO.File.ReadAllTextAsync(_injectionScript);

                    // 注入JS脚本内容
                    var scriptTag = $"<script>{injectionScript}</script>\n</body>";
                    responseString = responseString.Replace("</body>", scriptTag, StringComparison.OrdinalIgnoreCase);

                    // 设置修改后的响应正文
                    e.SetResponseBody(Encoding.UTF8.GetBytes(responseString));
                }
            });
        }

        /// <summary>
        /// 隧道链接请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnBeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            await Task.Run(() =>
            {
                //判断主机名字，如果不是则不解析，因为构造函数是默认解析的
                var hostname = e.HttpClient.Request.RequestUri.Host;
                if (!(hostname.Contains("finder.video.qq.com") || hostname.Contains("channels.weixin.qq.com")))
                {
                    //是否要解析SSL，不解析就直接转发
                    e.DecryptSsl = false;
                }
            });
        }

        #endregion
    }
}
