namespace Z.Tools.Modle
{
    public class Folder : Resource
    {
        /// <summary>
        /// 目录
        /// </summary>
        public string Parent { get; set; }
        public void SetInfo(string name, string path, string fullName, string parent)
        {
            this.Name = name;
            this.Path = path;
            this.FullName = fullName;
            this.Parent = parent;
        }
    }
}
