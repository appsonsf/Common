using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AppsOnSF.Common.Utilities
{
    public static class StringHelper
    {
        /// <summary>
        /// 判断字符串是否是合法的Guid
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsGuid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Guid.TryParse(input, out Guid result);
        }

        public static bool IsChineseMobile(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var regex = new Regex("^((13[0-9])|(14[5,7])|(15[0-3,5-9])|(17[0,3,5-8])|(18[0-9])|166|198|199|(147))\\d{8}$");
            return regex.IsMatch(input);
        }

        public static string GenerateMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();
                foreach (byte bytes in hashBytes)
                {
                    sb.Append(bytes.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
