using System.ComponentModel;
using System.Reflection;

namespace JiumiTool2.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 字符串转换为枚举值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string value)
        {
            var result = (TEnum)Enum.Parse(typeof(TEnum), value, true);

            return result;
        }

        /// <summary>
        /// 描述转枚举值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum DescriptionToEnum<TEnum>(this string value)
        {
            var fieldInfos = typeof(TEnum).GetFields();
            foreach (var item in fieldInfos)
            {
                if (!item.FieldType.Equals(typeof(TEnum)))
                {
                    continue;
                }
                var attribute = item.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                var description = attribute.Description;
                if (value.Equals(description))
                {
                    return (TEnum)item.GetValue(null);
                }
            }

            return default;
        }
    }
}
