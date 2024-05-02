using System.ComponentModel;
using System.Reflection;

namespace JiumiTool2.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 枚举转化为字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(this Enum value)
        {
            var result = Enum.GetName(value.GetType(), value);

            return result;
        }

        /// <summary>
        /// 获取枚举的描述列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> ToDescriptionList<T>() where T : Enum
        {
            var enums = Enum.GetValues(typeof(T))
                .Cast<Enum>();
            var result = new List<string>();
            foreach (var item in enums)
            {
                result.Add(item.GetDescription());
            }

            return result;
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType()
                .GetField(value.ToString());
            var attribute = fieldInfo?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}
