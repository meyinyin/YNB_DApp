using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YNB_DApp.Models
{
    public static class DecimalHelper
    {
        /// <summary>
        /// decimal保留指定位数小数
        /// </summary>
        /// <param name="num">原始数量</param>
        /// <param name="scale">保留小数位数</param>
        /// <returns>截取指定小数位数后的数量字符串</returns>
        public static string Todecimal(decimal num,int scale)
        {
            string numTodecimal = num.ToString();
            int index = numTodecimal.IndexOf(".");
            int length = numTodecimal.Length;
            if (index != -1) {
                return string.Format("{0}.{1}", numTodecimal.Substring(0, index), numTodecimal.Substring(index + 1, Math.Min(length - index - 1, scale)));
            }
            else
            {
                return num.ToString();
            }

        }

    }
}
