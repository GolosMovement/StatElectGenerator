using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ElectionStatistics.Model
{
    public static class EnumsHelper
    {
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct
        {
            var fieldInfo = typeof(TEnum).GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}