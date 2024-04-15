using Newtonsoft.Json;

namespace Z.JuimiTool.Models
{
    public class VideoDownloadInfo
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 上传者
        /// </summary>
        public string Uploader { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 视频时长
        /// </summary>
        [JsonProperty(PropertyName = "videolen")]
        public int VideoPlayLength { get; set; }

        /// <summary>
        /// 封面链接
        /// </summary>
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 下载链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public ulong Decodekey { get; set; }

        /// <summary>
        /// 解密数组
        /// </summary>
        public byte[] DecryptionArray { get; set; }
    }
}
