using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Cmp;
using Z.JiumiTool.IServices;

namespace Z.JiumiTool.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient httpClient;

        public DownloadService()
        {
            httpClient = new HttpClient();
        }

        public async Task<byte[]> DownloadImage(string url)
        {
            //构建请求信息
            var requestMessage = GetImageRequestMessage(url);
            //发送请求
            var response = await httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            //读取响应的数据
            var result = await response.Content.ReadAsByteArrayAsync();

            return result;
        }

        public async Task<string> DownloadVideo(string outputVideoPath, string url, byte[] decryptionArray)
        {
            //构建请求信息
            var requestMessageHead = GetVideoRequestMessage(url);
            //发送请求
            var responseHead = await httpClient.SendAsync(requestMessageHead);
            responseHead.EnsureSuccessStatusCode();

            //获取完整视频的总长度
            long totalSize = responseHead.Content.Headers.ContentLength.GetValueOrDefault(-1L);
            if (totalSize <= 0)
            {
                throw new ArgumentException("无法获取视频总长度");
            }

            //分段下载视频
            long downloadedSize = 0;
            long bytesProcessed = 0;

            while (downloadedSize < totalSize)
            {
                //计算本次请求的范围
                long startPosition = downloadedSize;
                //每次请求最大262144字节（最后一个块可能小于这个数）
                long endPosition = Math.Min(downloadedSize + 262143, totalSize - 1);

                //构建请求信息
                var requestMessageGet = GetVideoRequestMessage(url, startPosition, endPosition);

                //发送GET请求并获取响应
                var responseGet = await httpClient.SendAsync(requestMessageGet);
                responseGet.EnsureSuccessStatusCode();

                //获取响应体的流
                using var contentStream = await responseGet.Content.ReadAsStreamAsync();

                //创建用于保存完整视频的FileStream
                using var outputStream = new FileStream(outputVideoPath, FileMode.Append, FileAccess.Write, FileShare.None, 8 * 1024, true);

                //将响应体内容写入输出文件
                var buffer = new byte[4096];
                //字节读取数量
                int bytesRead;
                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    var remaining = Math.Min(decryptionArray.Length - bytesProcessed, bytesRead);
                    for (int i = 0; i < remaining; i++)
                    {
                        buffer[i] = (byte)(buffer[i] ^ decryptionArray[bytesProcessed + i]);
                    }
                    await outputStream.WriteAsync(buffer.AsMemory(0, bytesRead));

                    bytesProcessed += remaining;
                    downloadedSize += bytesRead;
                }
            }

            if (downloadedSize == totalSize)
                return outputVideoPath;
            else
                return "下载失败";
        }

        #region 私有方法

        /// <summary>
        /// 设置图片请求头
        /// </summary>
        private HttpRequestMessage GetImageRequestMessage(string url)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("Host", "finder.video.qq.com");
            requestMessage.Headers.Add("Connection", "keep-alive");
            requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            requestMessage.Headers.Add("Accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "image");
            requestMessage.Headers.Add("Referer", "https://channels.weixin.qq.com/");

            return requestMessage;
        }

        /// <summary>
        /// 设置视频第一次请求头
        /// </summary>
        private HttpRequestMessage GetVideoRequestMessage(string url)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Head, url);
            requestMessage.Headers.Host = "finder.video.qq.com";
            requestMessage.Headers.Connection.Add("keep-alive");
            requestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            requestMessage.Headers.Accept.ParseAdd("*/*");
            requestMessage.Headers.Add("Origin", "https://channels.weixin.qq.com");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
            requestMessage.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");

            return requestMessage;
        }

        /// <summary>
        /// 设置视频后续请求头
        /// </summary>
        private HttpRequestMessage GetVideoRequestMessage(string url, long startPosition, long endPosition)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Host = "finder.video.qq.com";
            requestMessage.Headers.Connection.Add("keep-alive");
            requestMessage.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            requestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            requestMessage.Headers.Add("Origin", "https://channels.weixin.qq.com");
            requestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            requestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            requestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
            requestMessage.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");
            requestMessage.Headers.Range = new RangeHeaderValue(startPosition, endPosition);

            return requestMessage;
        }

        #endregion
    }
}
