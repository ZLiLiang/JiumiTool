namespace JiumiTool2.IServices
{
    public interface IDownloadService
    {
        /// <summary>
        /// 下载封面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Task<byte[]> DownloadImage(string url);

        /// <summary>
        /// 下载视频
        /// </summary>
        /// <param name="url"></param>
        /// <param name="outputPath">视频路径 例如xx/aa/bb.mp4</param>
        /// <param name="decryptionArray"></param>
        /// <returns></returns>
        public Task<string> DownloadVideo(string url, string outputPath, byte[] decryptionArray);

        /// <summary>
        /// 下载视频
        /// </summary>
        /// <param name="url"></param>
        /// <param name="outputPath">视频路径 例如xx/aa/bb.mp4</param>
        /// <param name="decryptionArray"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        public Task<string> DownloadVideo(string url, string outputPath, byte[] decryptionArray, IProgress<double> progress);
    }
}
