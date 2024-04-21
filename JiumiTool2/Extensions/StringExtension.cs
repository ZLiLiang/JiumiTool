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
    }
}
