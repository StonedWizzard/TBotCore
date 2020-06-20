using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore
{
    public static class StringExtensionsClass
    {
        /// <summary>
        /// Check size of the string and return bool value
        /// to indicate if string larger than required (false) or not (true)
        /// </summary>
        public static bool IsBloated(this string str, int length)
        {
            if (String.IsNullOrEmpty(str)) return false;
            return !(str.Length <= length);
        }
    }
}
