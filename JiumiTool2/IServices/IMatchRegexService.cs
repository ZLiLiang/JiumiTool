using System.Text.RegularExpressions;
using JiumiTool2.Constants;

namespace JiumiTool2.IServices
{
    public interface IMatchRegexService
    {
        /// <summary>
        /// 生成正则表达式目标字符串
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string GeneratePattern(List<char> chars, string? pattern = null);

        /// <summary>
        /// 生成正则表达式的对象
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="modifySeat"></param>
        /// <returns></returns>
        public Regex GenerateRegex(string pattern, FileModifySeat modifySeat);
    }
}
