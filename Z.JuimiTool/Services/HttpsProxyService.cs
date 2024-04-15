using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Titanium.Web.Proxy.Network;
using Z.JuimiTool.Common;
using Z.JuimiTool.Constants;
using Z.JuimiTool.Extensions;
using Z.JuimiTool.IServices;
using Z.JuimiTool.Models;

namespace Z.JuimiTool.Services
{
    public class HttpsProxyService : IHttpsProxyService, IDisposable
    {
        private bool isDisposed = false;
        private readonly ProxyServer proxyServer;
        private readonly ExplicitProxyEndPoint explicitProxyEndPoint;
        public event Action<VideoDownloadInfo> VideoAddedToList;

        public HttpsProxyService()
        {
            explicitProxyEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true);
            proxyServer = new ProxyServer();
            //设置系统代理
            proxyServer.SetAsSystemHttpsProxy(explicitProxyEndPoint);
            //使用Windows证书生成引擎
            proxyServer.CertificateManager.CertificateEngine = CertificateEngine.DefaultWindows;
            //启用证书
            proxyServer.CertificateManager.EnsureRootCertificate();
            proxyServer.AddEndPoint(explicitProxyEndPoint);
        }

        public void Start()
        {
            //绑定需要的方法
            explicitProxyEndPoint.BeforeTunnelConnectRequest += OnBeforeTunnelConnectRequest;
            proxyServer.BeforeRequest += OnBeforeRequest;
            proxyServer.BeforeResponse += OnBeforeResponse;

            //启动代理
            proxyServer.Start();
        }

        public void Stop()
        {
            //解绑需要的方法
            explicitProxyEndPoint.BeforeTunnelConnectRequest -= OnBeforeTunnelConnectRequest;
            proxyServer.BeforeRequest -= OnBeforeRequest;
            proxyServer.BeforeResponse -= OnBeforeResponse;

            //停止代理
            proxyServer.Stop();
        }

        #region 资源释放

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                }

                // 停止代理服务
                if (proxyServer != null && proxyServer.ProxyRunning == true)
                {
                    Stop();
                }

                isDisposed = true;
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

                    var model = JsonConvert.DeserializeObject<VideoDownloadInfo>(requestString);
                    model.DecryptionArray = new Rng(model.Decodekey).GetDecryptionArray();

                    VideoAddedToList.Invoke(model);
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
                    var injectionScript = await System.IO.File.ReadAllTextAsync(ScriptPath.InjectionScript);

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
