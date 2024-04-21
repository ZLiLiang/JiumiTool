using System.Reflection;

namespace JiumiTool2.Commons
{
    public class NameToPageTypeConverter
    {
        private static readonly Type[] PageTypes = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.Namespace?.StartsWith("JiumiTool2.Views") ?? false)
        .ToArray();

        public static Type? Convert(string pageName)
        {
            return PageTypes.FirstOrDefault(singlePageType =>
                singlePageType.Name.Equals(pageName, StringComparison.CurrentCultureIgnoreCase)
            );
        }
    }
}
