using System.IO;
using System.Text.RegularExpressions;
using JiumiTool2.Models;

namespace JiumiTool2.IServices
{
    public abstract class AResourceService
    {
        /// <summary>
        /// 重命名资源 <br/>
        /// action的第一个参数是srcFullName，第二个参数是destFullName
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="regex">正则匹配器</param>
        /// <param name="action">重命名函数</param>
        /// <returns>路径列表</returns>
        public List<string> RepeatName(IEnumerable<Resource> resources, Regex regex, Action<string, string> action)
        {
            int count = 1;
            List<string> result = new List<string>();

            foreach (Resource item in resources)
            {
                // 跳过包含备份字符串的路径
                if (item.Name.Contains("备份"))
                {
                    continue;
                }

                var srcFullName = item.FullName;
                // 对资源名进行重命名并编号
                //var destName = regex.Replace(item.Name, evaluator =>
                //{
                //    return count.ToString();
                //});
                var match = regex.Match(item.Name);
                if (match.Success)
                {
                    var destName = item.Name;
                    // 计算匹配项的起始位置和长度，以确定替换范围
                    int startIndex = match.Index;
                    int length = match.Length;

                    destName = destName.Substring(0, startIndex) + count + destName.Substring(startIndex + length);
                    var destFullName = Path.Combine(item.Path, destName);

                    // 重命名资源
                    action.Invoke(srcFullName, destFullName);
                    result.Add(string.Join(" -> ", item.Name, destName));

                    count++;
                }
                //var destFullName = Path.Combine(item.Path, destName);

                //// 重命名资源
                //action.Invoke(srcFullName, destFullName);
                //result.Add(string.Join(" -> ", item.Name, destName));

                //count++;
            }

            return result;
        }

        /// <summary>
        /// 备份资源 <br/>
        /// action的第一个参数是srcFullName，第二个参数是destFullName
        /// </summary>
        /// <param name="resources">资源列表</param>
        /// <param name="action">备份函数</param>
        /// <returns>路径</returns>
        public string Backup(IEnumerable<Resource> resources, Action<string, string> action)
        {
            var backUpPath = $"{resources.First().Path}\\备份-{DateTime.Now:yyyy-MM-dd}";
            // 创建目录
            Directory.CreateDirectory(backUpPath);

            foreach (var item in resources)
            {
                var srcFullName = item.FullName;
                var destFullName = Path.Combine(backUpPath, item.Name);
                // 备份操作
                action.Invoke(srcFullName, destFullName);
            }

            return $"备份路径:{backUpPath}";
        }
    }
}
