namespace JiumiTool2.IServices
{
    public interface IHttpsProxyService
    {
        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start();

        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop();
    }
}
