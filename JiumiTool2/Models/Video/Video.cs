using System.Windows.Media.Imaging;

namespace JiumiTool2.Models
{
    public class Video
    {
        /// <summary>
        /// 封面数据
        /// </summary>
        public BitmapImage ImageData { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 视频时长
        /// </summary>
        public int VideoPlayLength { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Uploader { get; set; }

        /// <summary>
        /// 下载链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 解密数组
        /// </summary>
        public byte[] DecryptionArray { get; set; }
    }
}
