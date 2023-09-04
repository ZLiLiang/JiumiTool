namespace Z.Tools.Modle
{
    public class Resource
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }
        public string FullName { get; set; }

        public void SetInfo(string name, string path, string directory, string fullName)
        {
            this.Name = name;
            this.Path = path;
            this.Directory = directory;
            this.FullName = fullName;
        }
    }
}
