using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static YNB_DApp.Models.Common;

namespace YNB_DApp.Models
{
    public class RequestAccount<T>
    {
        /// <summary>
        /// 公共方法GetHttpOffer
        /// </summary>
        /// <param name="_accountid"></param>
        /// <returns></returns>
        public static async Task<ResultModel<T>> GetHttpOffer(string offerurl)
        {
            try
            {
                //string offerurl = $"https://horizon.stellar.org/accounts/{_accountid}/offers??cursor=a&order=desc&limit={_num}";
                using (var client = new HttpClient())
                {
                    var jsonString = await client.GetStringAsync(offerurl);
                    if(jsonString != "" || jsonString != null)
                    {
                        return new ResultModel<T> { IsSuccess = true, Code = 200, Msg = jsonString };
                    }
                    return new ResultModel<T> { IsSuccess = false, Code = 300, Msg = "网络波动" };
                }
            }
            catch
            {
                return new ResultModel<T> { IsSuccess = false, Code = 500, Msg = "网络波动" };
            }
        }

        

        
    }
}
