using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Z.JiumiTool.Extensions
{
    public static class EnumExtension
    {
        public static List<string> ToList<T>() where T : Enum
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

        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType()
                .GetField(value.ToString());
            var attribute = fieldInfo?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}