using System.Text.RegularExpressions;
using JiumiTool2.Constants;
using JiumiTool2.IServices;

namespace JiumiTool2.Services
{
    public class MatchRegexService : IMatchRegexService
    {
        public string GeneratePattern(List<char> chars, string? pattern = null)
        {
            pattern ??= ".*?";
            var startChar = chars.First();
            var endChar = chars.Last();
            var result = @$"\{startChar}{pattern}\{endChar}";

            return result;
        }

        public Regex GenerateRegex(string pattern, FileModifySeat modifySeat)
        {
            if (modifySeat == FileModifySeat.Prefix)
            {
                return new Regex(pattern, RegexOptions.None);
            }
            else
            {
                return new Regex(pattern, RegexOptions.RightToLeft);
            }
        }
    }
}
