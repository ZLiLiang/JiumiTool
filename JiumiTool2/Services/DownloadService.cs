﻿using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using CommunityToolkit.Mvvm.Messaging.Messages;
using JiumiTool2.IServices;

namespace JiumiTool2.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestMessage _httpRequestMessage;

        public DownloadService()
        {
            _httpClient = new();
            _httpRequestMessage = new();
        }

        public async Task<byte[]> DownloadImage(string url)
        {
            // 构建请求信息
            SetImageRequestMessage(url);
            // 发送请求
            var response = await _httpClient.SendAsync(_httpRequestMessage);
            response.EnsureSuccessStatusCode();
            // 读取响应的数据
            var result = await response.Content.ReadAsByteArrayAsync();

            return result;
        }

        public async Task<string> DownloadVideo(string url, string outputPath, byte[] decryptionArray)
        {
            // 构建请求信息
            SetVideoFirstRequestMessage(url);
            // 发送请求
            var fristResponse = await _httpClient.SendAsync(_httpRequestMessage);
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
                SetVideoPartRequestMessage(url, startPosition, endPosition);
                // 获取视频部分数据
                var partResponse = await _httpClient.SendAsync(_httpRequestMessage);
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
            SetVideoFirstRequestMessage(url);
            // 发送请求
            var fristResponse = await _httpClient.SendAsync(_httpRequestMessage);
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
                SetVideoPartRequestMessage(url, startPosition, endPosition);
                // 获取视频部分数据
                var partResponse = await _httpClient.SendAsync(_httpRequestMessage);
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
                progress.Report((downloadedSize / totalSize) * 100);
            }

            if (downloadedSize == totalSize)
                return outputPath;
            else
                return "下载失败";
        }

        #region 私有方法

        /// <summary>
        /// 设置图片请求头
        /// </summary>
        private void SetImageRequestMessage(string url)
        {
            _httpRequestMessage.Method = HttpMethod.Get;
            _httpRequestMessage.RequestUri = new Uri(url);

            _httpRequestMessage.Headers.Clear();

            _httpRequestMessage.Headers.Add("Host", "finder.video.qq.com");
            _httpRequestMessage.Headers.Add("Connection", "keep-alive");
            _httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            _httpRequestMessage.Headers.Add("Accept", "image/avif,image/webp,image/apng,image/svg+xml,image/*,*/*;q=0.8");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "image");
            _httpRequestMessage.Headers.Add("Referer", "https://channels.weixin.qq.com/");
        }

        /// <summary>
        /// 设置视频第一次请求头
        /// </summary>
        private void SetVideoFirstRequestMessage(string url)
        {
            _httpRequestMessage.Method = HttpMethod.Head;
            _httpRequestMessage.RequestUri = new Uri(url);

            _httpRequestMessage.Headers.Clear();

            _httpRequestMessage.Headers.Host = "finder.video.qq.com";
            _httpRequestMessage.Headers.Connection.Add("keep-alive");
            _httpRequestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            _httpRequestMessage.Headers.Accept.ParseAdd("*/*");
            _httpRequestMessage.Headers.Add("Origin", "https://channels.weixin.qq.com");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
            _httpRequestMessage.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");
        }

        /// <summary>
        /// 设置视频后续请求头
        /// </summary>
        private void SetVideoPartRequestMessage(string url, long startPosition, long endPosition)
        {
            _httpRequestMessage.Method = HttpMethod.Get;
            _httpRequestMessage.RequestUri = new Uri(url);

            _httpRequestMessage.Headers.Clear();

            _httpRequestMessage.Headers.Host = "finder.video.qq.com";
            _httpRequestMessage.Headers.Connection.Add("keep-alive");
            _httpRequestMessage.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            _httpRequestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            _httpRequestMessage.Headers.Add("Origin", "https://channels.weixin.qq.com");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Site", "same-site");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "no-cors");
            _httpRequestMessage.Headers.Add("Sec-Fetch-Dest", "empty");
            _httpRequestMessage.Headers.Referrer = new Uri("https://channels.weixin.qq.com/");
            _httpRequestMessage.Headers.Range = new RangeHeaderValue(startPosition, endPosition);
        }

        #endregion
    }
}
