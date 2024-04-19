using System.Windows.Media.Imaging;

namespace Z.JiumiTool.Models
{
    public class Video
    {
        /// <summary>
        /// 封面数据
        /// </summary>
        public BitmapImage ImageData { get; set; }

        /// <summary>
        /// 简略描述
        /// </summary>
        public string BriefTitle { get; set; }

        /// <summary>
        /// 完整描述
        /// </summary>
        public string WholeTitle { get; set; }

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
        /// 视频保存路径
        /// </summary>
        public string VideoSavePath { get; set; }
    }
}
