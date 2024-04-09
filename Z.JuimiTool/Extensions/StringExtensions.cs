using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Z.JuimiTool.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 对路径以','进行分割
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> GetPaths(this string value)
        {
            return value.Split(',').ToList();
        }

        /// <summary>
        /// 删除文件名的括号以及括号的内容
        /// </summary>
        /// <param name="srcName">原文件名称</param>
        /// <param name="count">编号</param>
        /// <returns></returns>
        public static string RemoveParentheses(string srcName, int count)
        {
            string fileName = Regex.Replace(srcName, @"\([^)]*\)", ""); // 去除括号及内容

            fileName = Regex.Replace(fileName, @"(?:\s|\d)+(?=\.|\s|$)", math =>
            {
                return count.ToString();
            }); //对文件名进行编号

            return fileName;
        }
    }
}
