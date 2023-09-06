namespace Z.Tools.Modle
{
    public class File : Resource
    {
        /// <summary>
        /// 目录
        /// </summary>
        public string Directory { get; set; }

        public void SetInfo(string name, string path, string fullName, string directory)
        {
            this.Name = name;
            this.Path = path;
            this.FullName = fullName;
            this.Directory = directory;
        }
    }
}
