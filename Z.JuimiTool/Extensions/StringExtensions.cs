using System.Collections.Generic;
using System.Linq;

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
    }
}
