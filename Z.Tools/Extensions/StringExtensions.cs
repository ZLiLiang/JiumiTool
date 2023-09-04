using System.IO;

namespace Z.Tools.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 对字符串以','进行分割，分割后只有一个字符串则返回原字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic GetPath(this string value)
        {
            string[] values = value.Split(',');
            if (values.Length < 2)
            {
                return value;
            }
            else
            {
                return values;
            }
        }
    }
}
