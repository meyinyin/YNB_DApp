using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stellar_dotnet_sdk;
using static YNB_DApp.Models.Common;
using static YNB_DApp.Models.ResponseAccount;

namespace YNB_DApp.Controllers.api
{
    //区块信息查询
    [ApiController]
    public class ExplorerController : ControllerBase
    {
        string BaseUrl = "https://horizon.stellar.org";
        /// <summary>
        /// 币种查询信息类
        /// </summary>
        public class Explorer_AssetInfo
        {
            public string Type { get; set; }
            public string Code { get; set; }
            public string Issuer { get; set; }
            public string Amount { get; set; }
            /// <summary>
            /// 币种图标
            /// </summary>
            public string Image { get; set; }
            /// <summary>
            /// 主域地址
            /// </summary>
            public string Toml { get; set; }
        }
        /// <summary>
        /// 币种查询
        /// </summary>
        /// <param name="code"></param>
        /// <param name="issuer">默认为空</param>
        /// <param name="num">默认为10</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/searchAsset")]
        public async Task<ResultModel<List<Explorer_AssetInfo>>> SearchAsset(string code, string issuer = "", string num = "10")
        {
            try
            {
                string asseturl = BaseUrl+$"/assets?asset_code={code}&limit={num}";
                if (code == "YNB")
                {
                    asseturl = BaseUrl+"/assets?asset_code=YNB&asset_issuer=GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3";
                }else if(issuer != "")
                {
                    asseturl = BaseUrl+$"/assets?asset_code={code}&asset_issuer={issuer}";
                }
                List<Explorer_AssetInfo> Explorer_AssetInfo = new List<Explorer_AssetInfo>();

                var assetinfo = await YNB_DApp.Models.RequestAccount<List<Explorer_AssetInfo>>.GetHttpOffer(asseturl);
                if (assetinfo.IsSuccess)
                {
                    if (assetinfo.Msg != "" && assetinfo.Msg != null)
                    {
                        var responseText = assetinfo.Msg;
                        GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                        var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                        AssetEmbedded embedded1 = JsonConvert.DeserializeObject<AssetEmbedded>(embedded);
                        var listss = embedded1.records.ToList();
                        
                        //AssetEmbedded embedded1 = JsonConvert.DeserializeObject<AssetEmbedded>(embedded);
                        //var listss = embedded1.assetRecords.ToList();
                        foreach (AssetRecords item in listss)
                        {
                            Explorer_AssetInfo ghcl = new Explorer_AssetInfo();
                            ghcl.Code = item.asset_code;
                            ghcl.Type = item.asset_type;
                            ghcl.Issuer = item.asset_issuer;
                            ghcl.Amount = item.amount;
                            Explorer_AssetInfo.Add(ghcl);
                        }
                        return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = true, Msg = "成功", Code = 200, Data = Explorer_AssetInfo };
                    }
                    return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = true, Code = 300, Msg = "查无此币", Data = null };
                }
                else
                {
                    return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };
                }
            }
            catch
            {
                return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };

            }


            
        }

        /// <summary>
        /// 币种查询(带图标)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="issuer">默认为空</param>
        /// <param name="num">默认为10</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/searchAssetimage")]
        public async Task<ResultModel<List<Explorer_AssetInfo>>> SearchAssetImage(string code,string issuer = "", string num = "10")
        {
            try
            {
                string asseturl = BaseUrl + $"/assets?asset_code={code}&limit={num}";
                if (code == "YNB")
                {
                    asseturl = BaseUrl + "/assets?asset_code=YNB&asset_issuer=GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3";
                }
                else if (issuer != "")
                {
                    asseturl = BaseUrl + $"/assets?asset_code={code}&asset_issuer={issuer}";
                }
                List<Explorer_AssetInfo> Explorer_AssetInfo = new List<Explorer_AssetInfo>();

                var assetinfo = await YNB_DApp.Models.RequestAccount<List<Explorer_AssetInfo>>.GetHttpOffer(asseturl);
                if (assetinfo.IsSuccess)
                {
                    if (assetinfo.Msg != "" && assetinfo.Msg != null)
                    {
                        var responseText = assetinfo.Msg;
                        GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                        var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                        AssetEmbedded embedded1 = JsonConvert.DeserializeObject<AssetEmbedded>(embedded);
                        var listss = embedded1.records.ToList();
                        //AssetEmbedded embedded1 = JsonConvert.DeserializeObject<AssetEmbedded>(embedded);
                        //var listss = embedded1.assetRecords.ToList();
                        foreach (AssetRecords item in listss)
                        {
                            
                            var embedded2 = JsonConvert.SerializeObject(item._links);
                            string startTxt = "href':'".Replace("\'","\"");
                            string endTxt = "'}".Replace("\'", "\"");
                            Regex rg = new Regex("(?<=(" + startTxt + "))[.\\s\\S]*?(?=(" + endTxt + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                            //解析币种主域
                            //AssetRecordsToml embedded3 = JsonConvert.DeserializeObject<AssetRecordsToml>(embedded2);
                            //var embedded4 = JsonConvert.SerializeObject(embedded3.toml);
                            //AssetRecordsTomlHref embedded5 = JsonConvert.DeserializeObject<AssetRecordsTomlHref>(embedded4);
                            Explorer_AssetInfo ghcl = new Explorer_AssetInfo();
                            ghcl.Code = item.asset_code;
                            ghcl.Type = item.asset_type;
                            ghcl.Issuer = item.asset_issuer;
                            ghcl.Amount = item.amount;
                            ghcl.Toml = rg.Match(embedded2).Value;//正则方案
                            //ghcl.Toml = embedded5.href;//解析方案
                            ghcl.Image = "";
                            if (ghcl.Toml != null && ghcl.Toml != "")
                            {
                                var imageReq = TomlForImage(ghcl.Toml, ghcl.Code);
                                if (imageReq.Result.IsSuccess)
                                {
                                    ghcl.Image = imageReq.Result.Data.ImageUrl;
                                }
                            }
                            Explorer_AssetInfo.Add(ghcl);
                        }
                        return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = true, Msg = "成功", Code = 200, Data = Explorer_AssetInfo };
                    }
                    return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = true, Code = 300, Msg = "查无此币", Data = null };
                }
                else
                {
                    return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };
                }
            }
            catch
            {
                return new ResultModel<List<Explorer_AssetInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };
            }
        }

        /// <summary>
        /// 资产图标类
        /// </summary>
        public class Explorer_AssetImage
        {
            public string ImageUrl { get; set; }
        }
        /// <summary>
        /// 读取资产图标
        /// </summary>
        /// <param name="toml"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/tomlforimage")]
        public async Task<ResultModel<Explorer_AssetImage>> TomlForImage(string toml,string code)
        {
            try
            {
                var Explorer_AssetImage = new Explorer_AssetImage();
                var assettomlinfo = await YNB_DApp.Models.RequestAccount<Explorer_AssetImage>.GetHttpOffer(toml);
                if (assettomlinfo.IsSuccess)
                {
                    if (assettomlinfo.Msg != "" && assettomlinfo.Msg != null)
                    {
                        //string CURRENCIES = "CURRENCIES";
                        var codeTxt = $"code='{code}'".Replace("\'", "\"");
                        string startTxt = "image='".Replace("\'", "\"");
                        string endTxt = "'".Replace("\'", "\"");
                        var imageurltxt = assettomlinfo.Msg.Substring(assettomlinfo.Msg.IndexOf(codeTxt));
                        Regex rg = new Regex("(?<=(" + startTxt + "))[.\\s\\S]*?(?=(" + endTxt + "))", RegexOptions.Multiline | RegexOptions.Singleline);
                        var imageurl = rg.Match(imageurltxt).Value;
                        if (imageurl != "" && imageurl != null)
                        {
                            Explorer_AssetImage.ImageUrl = imageurl;
                            return new ResultModel<Explorer_AssetImage> { IsSuccess = true, Code = 200, Msg = "成功", Data = Explorer_AssetImage };
                        }
                        return new ResultModel<Explorer_AssetImage> { IsSuccess = false, Code = 300, Msg = "资产图标解析出错" };
                    }
                    return new ResultModel<Explorer_AssetImage> { IsSuccess = false, Code = 300, Msg = "资产没有设置图标" };
                }
                return new ResultModel<Explorer_AssetImage> { IsSuccess = false, Code = 500, Msg = "资产图标无权读取" };
            }
            catch
            {
                return new ResultModel<Explorer_AssetImage> { IsSuccess = false, Code = 500, Msg = "网络波动" };
            }
        }

        /// <summary>
        /// 正则测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/txt")]
        public string Txt()
        {
            string txt = "{'toml':{'href':'https://gwallet.tech/.well-known/stellar.toml'}}";
            var s = "href':'";
            string e = "'}";
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(txt).Value;
        }

        /// <summary>
        /// 转账记录信息类
        /// </summary>
        public class Explorer_PaymentInfo
        {
            public string Type { get; set; }
            public string Code { get; set; }
            public string Issuer { get; set; }
            public string Amount { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string DateTime { get; set; }
            public string Hash { get; set; }

        }
        /// <summary>
        /// 查询转账记录
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/searchaccountpayment")]
        public async Task<ResultModel<List<Explorer_PaymentInfo>>> SearchAccountPayment(string accountid,int num = 10)
        {
            try
            {
                string accounturl = BaseUrl+$"/accounts/{accountid}/payments?order=desc&limit={num}";
                //string accounturl = $"https://horizon.stellar.org/accounts/{accountid}/payments?order=desc&limit={num}";
                List<Explorer_PaymentInfo> Explorer_PaymentInfo = new List<Explorer_PaymentInfo>();
                var accountinfo = await YNB_DApp.Models.RequestAccount<List<Explorer_PaymentInfo>>.GetHttpOffer(accounturl);
                if (accountinfo.IsSuccess)
                {
                    if (accountinfo.Msg != "" && accountinfo.Msg != null)
                    {
                        var responseText = accountinfo.Msg;
                        GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                        var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                        PaymentEmbedded embedded1 = JsonConvert.DeserializeObject<PaymentEmbedded>(embedded);
                        var listss = embedded1.records.ToList();

                        foreach (AccountPaymentRecords item in listss)
                        {
                            Explorer_PaymentInfo ghcl = new Explorer_PaymentInfo();
                            if (item.asset_type == "native")
                            {
                                ghcl.Code = "XLM";
                                ghcl.Type = item.asset_type;
                                ghcl.Issuer = "";
                            }
                            else
                            {
                                ghcl.Code = item.asset_code;
                                ghcl.Type = item.asset_type;
                                ghcl.Issuer = item.asset_issuer;
                            }
                            ghcl.Amount = item.amount;
                            ghcl.From = item.from;
                            ghcl.To = item.to;
                            ghcl.DateTime = item.created_at;
                            ghcl.Hash = item.transaction_hash;
                            Explorer_PaymentInfo.Add(ghcl);
                        }
                        return new ResultModel<List<Explorer_PaymentInfo>> { IsSuccess = true, Msg = "成功", Code = 200, Data = Explorer_PaymentInfo };
                    }
                    return new ResultModel<List<Explorer_PaymentInfo>> { IsSuccess = true, Msg = "暂无记录", Code = 200,Data = null };
                }
                else
                {
                    return new ResultModel<List<Explorer_PaymentInfo>> { IsSuccess = false, Msg = "网络波动", Code = 500 };
                }
            }
            catch
            {
                return new ResultModel<List<Explorer_PaymentInfo>> { IsSuccess = false, Msg = "网络波动", Code = 500 };
            }
            
        }

        /// <summary>
        /// 转账记录详细信息类(手续费、信封地址、备注...)
        /// </summary>
        public class Explorer_PaymentHashInfo
        {
            public string Hash { get; set; }
            public string DateTime { get; set; }
            public string Fee { get; set; }
            public string Max_Fee{get;set;}
            public string Ledger { get; set; }
            public string Envelope_Xdr { get; set; }
            public string Fee_Xdr { get; set; }
            public string Memo_Type { get; set; }
            public string Memo { get; set; }
        }

        /// <summary>
        /// 查询转账记录详细信息
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/searchpaymenthash")]
        public async Task<ResultModel<Explorer_PaymentHashInfo>> SearchPaymentHash(string hash)
        {
            try
            {
                string hashurl = BaseUrl+$"/transactions/{hash}";
                var hashinfo = await YNB_DApp.Models.RequestAccount<Explorer_PaymentHashInfo>.GetHttpOffer(hashurl);
                if (hashinfo.IsSuccess)
                {
                    if (hashinfo.Msg != "" && hashinfo.Msg != null)
                    {
                        var item = new Explorer_PaymentHashInfo();
                        var responseText = hashinfo.Msg;
                        PaymentInfoRecords ghcm = JsonConvert.DeserializeObject<PaymentInfoRecords>(responseText);
                        item.Hash = ghcm.hash;
                        item.DateTime = ghcm.created_at;
                        item.Fee = ghcm.fee_charged;
                        item.Max_Fee = ghcm.max_fee;
                        item.Ledger = ghcm.ledger;
                        item.Envelope_Xdr = ghcm.envelope_xdr;
                        item.Fee_Xdr = ghcm.fee_meta_xdr;
                        item.Memo_Type = ghcm.memo_type;
                        item.Memo = ghcm.memo;
                        return new ResultModel<Explorer_PaymentHashInfo> { IsSuccess = true, Msg = "成功", Code = 200, Data = item };
                    }
                    return new ResultModel<Explorer_PaymentHashInfo> { IsSuccess = true, Msg = "未查询到区块哈希值信息", Code = 300 };
                }
                else
                {
                    return new ResultModel<Explorer_PaymentHashInfo> { IsSuccess = false, Msg = "网络波动", Code = 500 };
                }
            }
            catch
            {
                return new ResultModel<Explorer_PaymentHashInfo> { IsSuccess = false, Msg = "网络波动", Code = 500 };
            }
            
        }


        /// <summary>
        /// 价格(未启用)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/assetprice")]
        public async Task<ResultCode> AssetPrice()
        {
            string buycode = "";
            string buyissuingPublicKeys = "";
            Server server = new Server("https://horizon.stellar.org");
            var es = server.Trades.BaseAsset(new AssetTypeCreditAlphaNum4(buycode, buyissuingPublicKeys));
            return new ResultCode { };
        }
    }
}
