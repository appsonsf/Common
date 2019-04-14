using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppsOnSF.Common.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举项上的<see cref="DescriptionAttribute"/>特性的文字描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo member = type.GetMember(value.ToString()).FirstOrDefault();
            var attr = member.GetCustomAttribute<DescriptionAttribute>();
            if (attr == null)
                return "";
            return attr.Description;
        }
    }
}
