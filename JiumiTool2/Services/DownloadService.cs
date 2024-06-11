using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using JiumiTool2.IServices;

namespace JiumiTool2.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;

        public DownloadService()
        {
            var handler = new HttpClientHandler { UseProxy = false };
            _httpClient = new(handler);
        }

        public async Task<byte[]> DownloadImage(string url)
        {
            // 构建请求信息
            var request = GetImageRequestMessage(url);
            // 发送请求
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            // 读取响应的数据
            var result = await response.Content.ReadAsByteArrayAsync();

            return result;
        }

        public async Task<string> DownloadVideo(string url, string outputPath, byte[] decryptionArray)
        {
            // 构建请求信息
            var firstRequest = GetVideoFirstRequestMessage(url);
            // 发送请求
            var fristResponse = await _httpClient.SendAsync(firstRequest);
            fristResponse.EnsureSuccessStatusCode();

            // 获取完整视频的总长度
            long totalSize = fristResponse.Content.Headers.ContentLength.GetValueOrDefault(-1L);
            if (totalSize <= 0)
            {
                throw new ArgumentException("无法获取视频总长度");
            }

            // 分段下载视频
            long downloadedSize = 0;
            long bytesProcessed = 0;

            while (downloadedSize < totalSize)
            {
                // 计算本次请求的范围
                long startPosition = downloadedSize;
                // 每次请求最大262144字节（最后一个块可能小于这个数）
                long endPosition = Math.Min(downloadedSize + 262143, totalSize - 1);

                // 构建请求信息
                var partRequest = GetVideoPartRequestMessage(url, startPosition, endPosition);
                // 获取视频部分数据
                var partResponse = await _httpClient.SendAsync(partRequest);
                partResponse.EnsureSuccessStatusCode();

                // 获取响应体的流
                using var contentStream = await partResponse.Content.ReadAsStreamAsync();

                // 创建用于保存完整视频的FileStream
                using var outputStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write, FileShare.None, 8 * 1024, true);

                // 将响应体内容写入输出文件
                var buffer = new byte[4096];
                // 字节读取数量
                int bytesRead = 0;

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
                return outputPath;
            else
                return "下载失败";
        }


        public async Task<string> DownloadVideo(string url, string outputPath, byte[] decryptionArray, IProgress<double> progress)
        {
            // 构建请求信息
            var firstRequest = GetVideoFirstRequestMessage(url);
            // 发送请求
            var fristResponse = await _httpClient.SendAsync(firstRequest);
            fristResponse.EnsureSuccessStatusCode();

            // 获取完整视频的总长度
            long totalSize = fristResponse.Content.Headers.ContentLength.GetValueOrDefault(-1L);
            if (totalSize <= 0)
            {
                throw new ArgumentException("无法获取视频总长度");
            }

            // 分段下载视频
            long downloadedSize = 0;
            long bytesProcessed = 0;

            while (downloadedSize < totalSize)
            {
                // 计算本次请求的范围
                long startPosition = downloadedSize;
                // 每次请求最大262144字节（最后一个块可能小于这个数）
                long endPosition = Math.Min(downloadedSize + 262143, totalSize - 1);

                // 构建请求信息
                var partRequest = GetVideoPartRequestMessage(url, startPosition, endPosition);
                // 获取视频部分数据
                var partResponse = await _httpClient.SendAsync(partRequest);
                partResponse.EnsureSuccessStatusCode();

                // 获取响应体的流
                using var contentStream = await partResponse.Content.ReadAsStreamAsync();

                // 创建用于保存完整视频的FileStream
                using var outputStream = new FileStream(outputPath, FileMode.Append, FileAccess.Write, FileShare.None, 8 * 1024, true);

                // 将响应体内容写入输出文件
                var buffer = new byte[4096];
                // 字节读取数量
                int bytesRead = 0;

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

                // 更新进度条
                progress.Report((((double)downloadedSize / (double)totalSize) * 100));
            }

            if (downloadedSize == totalSize)
                return outputPath;
            else
                return "下载失败";
        }

        #region 私有方法

        /// <summary>
        /// 获取图片请求头
        /// </summary>
        private HttpRequestMessage GetImageRequestMessage(string url)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            httpRequest.Headers.Add("Host", "finder.video.qq.com");
            httpRequest.Headers.Add("Connection", "keep-alive");
            httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            httpRequest.Headers.Add("Accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
            httpRequest.Headers.Add("Sec-Fetch-Site", "same-site");
            httpRequest.Headers.Add("Sec-Fetch-Mode", "no-cors");
            httpRequest.Headers.Add("Sec-Fetch-Dest", "image");
            httpRequest.Headers.Add("Referer", "https://channels.weixin.qq.com/");

            return httpRequest;
        }

        /// <summary>
        /// 获取视频第一次请求头
        /// </summary>
        private HttpRequestMessage GetVideoFirstRequestMessage(string url)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Head, url);

            httpRequest.Headers.Host = "finder.video.qq.com";
            httpRequest.Headers.Connection.Add("keep-alive");
            httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            httpRequest.Headers.Accept.ParseAdd("*/*");
            httpRequest.Headers.Add("Origin", "https://channels.weixin.qq.com");
            httpRequest.Headers.Add("Sec-Fetch-Site", "same-site");
            httpRequest.Headers.Add("Sec-Fetch-Mode", "no-cors");
            httpRequest.Headers.Add("Sec-Fetch-Dest", "empty");
            httpRequest.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");

            return httpRequest;
        }

        /// <summary>
        /// 获取视频后续请求头
        /// </summary>
        private HttpRequestMessage GetVideoPartRequestMessage(string url, long startPosition, long endPosition)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, url);

            httpRequest.Headers.Host = "finder.video.qq.com";
            httpRequest.Headers.Connection.Add("keep-alive");
            httpRequest.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            httpRequest.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            httpRequest.Headers.Add("Origin", "https://channels.weixin.qq.com");
            httpRequest.Headers.Add("Sec-Fetch-Site", "same-site");
            httpRequest.Headers.Add("Sec-Fetch-Mode", "no-cors");
            httpRequest.Headers.Add("Sec-Fetch-Dest", "empty");
            httpRequest.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");
            httpRequest.Headers.Range = new RangeHeaderValue(startPosition, endPosition);

            return httpRequest;
        }

        #endregion
    }
}
