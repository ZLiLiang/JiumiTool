namespace Z.Tools.Modle
{
    public abstract class Resource
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 绝对路径，不包括目标名称
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 绝对路径
        /// </summary>
        public string FullName { get; set; }

    }
}
