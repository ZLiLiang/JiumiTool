using System.Threading.Tasks;

namespace Z.JuimiTool.IServices
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
        /// <param name="outputVideoPath">视频路径 例如xx/aa/bb.mp4</param>
        /// <param name="url"></param>
        /// <param name="decryptionArray"></param>
        /// <returns></returns>
        public Task<string> DownloadVideo(string outputVideoPath, string url, byte[] decryptionArray);
    }
}
