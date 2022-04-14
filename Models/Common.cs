using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YNB_DApp.Models
{
    public class Common
    {
        /// <summary>
        /// 通用返回1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class ResultModel<T>
        {
            public bool IsSuccess { get; set; }
            public int Code { get; set; }
            public T Data { get; set; }
            public string Msg { get; set; }
            public string Hash { get; set; }

        }
        /// <summary>
        /// 通用返回2
        /// </summary>
        public class ResultCode
        {
            public bool IsSuccess { get; set; }
            public int Code { get; set; }
            public string Msg { get; set; }
            public string Hash { get; set; }
            public string Data { get; set; }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        public class ResultToJson
        {
            public static HttpResponseMessage toJson(Object obj)
            {
                String str;
                if (obj is String || obj is Char)
                {
                    str = obj.ToString();
                }
                else
                {
                    str = JsonConvert.SerializeObject(obj);
                }
                HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
                return result;
            }
        }
        
        
    }


}
