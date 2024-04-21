namespace JiumiTool2.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(this Enum value)
        {
            var result = Enum.GetName(value.GetType(), value);

            return result;
        }
    }
}
