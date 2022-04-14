using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stellar_dotnet_sdk;
using static YNB_DApp.Models.Common;
using static YNB_DApp.Models.ResponseAccount;

namespace YNB_DApp.Controllers.api
{
    
    [ApiController]
    public class MangerOfferController : ControllerBase
    {
        /// <summary>
        /// 账户挂单查询
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/offerlist")]
        public async Task<ResultModel<List<GetHttpClassList>>> OfferList(string accountid, string num = "10")
        {
            string offerurl = $"https://horizon.stellar.org/accounts/{accountid}/offers??cursor=a&order=desc&limit={num}";
            //var _accountid = accountid;
            //var _num = num;
            List<GetHttpClassList> ghcllist = new List<GetHttpClassList>() { };
            var ret = await YNB_DApp.Models.RequestAccount<List<GetHttpClassList>>.GetHttpOffer(offerurl);
            if (ret.IsSuccess == true)
            {
                var responseText = ret.Msg;
                GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                Embedded embedded1 = JsonConvert.DeserializeObject<Embedded>(embedded);
                var listss = embedded1.records.ToList();
                foreach (Records item in listss)
                {
                    var sell_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_type;
                    var sell_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_code;
                    var sell_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_issuer;
                    var buy_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_type;
                    var buy_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_code;
                    var buy_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_issuer;
                    GetHttpClassList ghcl = new GetHttpClassList();
                    decimal price_ = (decimal)Convert.ToSingle(item.price);
                    decimal amount_ = decimal.Parse(item.amount);
                    decimal sumprice;
                    if (buy_asset_code == "YNB" || sell_asset_type == "native" || buy_asset_issuer == "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3" || sell_asset_code == "USDC")
                    {
                        ghcl._amount = item.amount;
                        ghcl._price = item.price;
                        ghcl._fprice = 1 / price_;
                        ghcl._famount = amount_ * price_;
                        ghcl._fprice = decimal.Round(ghcl._fprice, 7);
                        ghcl._offerid = item.id;
                        ghcl._lastdate = item.last_modified_time;

                    }
                    else
                    {
                        ghcl._amount = item.amount;
                        ghcl._price = item.price;
                        ghcl._offerid = item.id;
                        ghcl._lastdate = item.last_modified_time;
                        ghcl._fprice = price_;
                        ghcl._famount = amount_;
                    }
                    
                    if (sell_asset_type == "native")
                    {
                        ghcl._selling_code = "XLM";
                        ghcl._selling_issuer = "";
                        ghcl._selling_type = sell_asset_type;

                    }
                    else
                    {
                        ghcl._selling_code = sell_asset_code;
                        ghcl._selling_issuer = sell_asset_issuer;
                        ghcl._selling_type = sell_asset_type;
                    }
                    if (buy_asset_type == "native")
                    {
                        ghcl._buying_code = "XLM";
                        ghcl._buying_issuer = "";
                        ghcl._buying_type = buy_asset_type;
                    }
                    else
                    {
                        ghcl._buying_code = buy_asset_code;
                        ghcl._buying_issuer = buy_asset_issuer;
                        ghcl._buying_type = buy_asset_type;
                    }
                    ghcllist.Add(ghcl);
                }
                return new ResultModel<List<GetHttpClassList>> { IsSuccess = true, Code = 200, Msg = "成功", Data = ghcllist };
            }

            return new ResultModel<List<GetHttpClassList>> { IsSuccess = false, Code = 500 };
        }

        public class allOffer
        {
            public List<GetHttpClassList> selllist { get; set; }
            public List<GetHttpClassList> buylist { get; set; }

            public allOffer()
            {
                selllist = new List<GetHttpClassList>();
                buylist = new List<GetHttpClassList>();
            }
        }
        /// <summary>
        /// 币种挂单查询
        /// </summary>
        /// <param name="asset_type"></param>
        /// <param name="asset_issuer"></param>
        /// <param name="asset_code"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/allofferlist")]
        public async Task<ResultModel<allOffer>> AllOfferList(string asset_type, string asset_issuer, string asset_code,string num = "15")
        {
            List<allOffer> allOffer = new List<allOffer>();
            string sellurl = $"https://horizon.stellar.org/offers?selling_asset_type={asset_type}&selling_asset_issuer={asset_issuer}&selling_asset_code={asset_code}&order=desc&limit={num}";
            string buyurl = $"https://horizon.stellar.org/offers?buying_asset_type={asset_type}&buying_asset_issuer={asset_issuer}&buying_asset_code={asset_code}&order=desc&limit={num}";
            //List<GetHttpClassList> ghclselllist = new List<GetHttpClassList>();
            //allOffer ghclselllist = new allOffer();
            allOffer allofferlist = new allOffer();
            var sellret = await YNB_DApp.Models.RequestAccount<List<GetHttpClassList>>.GetHttpOffer(sellurl);
            var buyret = await YNB_DApp.Models.RequestAccount<List<GetHttpClassList>>.GetHttpOffer(buyurl);
            if (sellret.IsSuccess && buyret.IsSuccess)
            {
                if (sellret.Msg != "" && sellret.Msg != null)
                {
                    var responseText = sellret.Msg;
                    GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                    var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                    Embedded embedded1 = JsonConvert.DeserializeObject<Embedded>(embedded);
                    var listss = embedded1.records.ToList();
                    foreach (Records item in listss)
                    {
                        GetHttpClassList ghcl = new GetHttpClassList();
                        decimal price_ = (decimal)Convert.ToSingle(item.price);
                        decimal amount_ = decimal.Parse(item.amount);
                        ghcl._amount = item.amount;
                        ghcl._price = item.price;
                        ghcl._fprice = price_;
                        ghcl._famount = amount_;
                        ghcl._offerid = item.id;
                        ghcl._lastdate = item.last_modified_time;
                        var sell_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_type;
                        var sell_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_code;
                        var sell_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_issuer;
                        var buy_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_type;
                        var buy_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_code;
                        var buy_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_issuer;
                        if (sell_asset_type == "native")
                        {
                            ghcl._selling_code = "XLM";
                            ghcl._selling_issuer = "";
                            ghcl._selling_type = sell_asset_type;
                        }
                        else
                        {
                            ghcl._selling_code = sell_asset_code;
                            ghcl._selling_issuer = sell_asset_issuer;
                            ghcl._selling_type = sell_asset_type;
                        }
                        if (buy_asset_type == "native")
                        {
                            ghcl._buying_code = "XLM";
                            ghcl._buying_issuer = "";
                            ghcl._buying_type = buy_asset_type;
                        }
                        else
                        {
                            ghcl._buying_code = buy_asset_code;
                            ghcl._buying_issuer = buy_asset_issuer;
                            ghcl._buying_type = buy_asset_type;
                        }
                        //allofferlist.selllist("_price","desc");
                        
                        allofferlist.selllist.Add(ghcl);
                    }
                }
                if (buyret.Msg != "" && buyret.Msg != null)
                {
                    var responseText = buyret.Msg;
                    GetHttpClassModel ghcm = JsonConvert.DeserializeObject<GetHttpClassModel>(responseText);
                    var embedded = JsonConvert.SerializeObject(ghcm._embedded);
                    Embedded embedded1 = JsonConvert.DeserializeObject<Embedded>(embedded);
                    var listss = embedded1.records.ToList();
                    foreach (Records item in listss)
                    {
                        GetHttpClassList ghcl = new GetHttpClassList();
                        //decimal price_ = (decimal)Convert.ToSingle(item.price);
                        decimal price_ = decimal.Parse(item.price);
                        //price_ = decimal.Round(price_, 7);
                        //string price__= YNB_DApp.Models.DecimalHelper.Todecimal(price_, 7);
                        //price_ = decimal.Parse(price__);
                        decimal amount_ = decimal.Parse(item.amount);
                        ghcl._amount = item.amount;
                        ghcl._price = item.price;
                        ghcl._fprice = 1/price_;
                        ghcl._famount = amount_ * price_;
                        ghcl._fprice = decimal.Round(ghcl._fprice, 7);
                        ghcl._offerid = item.id;
                        ghcl._lastdate = item.last_modified_time;
                        var sell_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_type;
                        var sell_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_code;
                        var sell_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.selling)).asset_issuer;
                        var buy_asset_type = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_type;
                        var buy_asset_code = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_code;
                        var buy_asset_issuer = JsonConvert.DeserializeObject<BuyOrSell>(JsonConvert.SerializeObject(item.buying)).asset_issuer;
                        if (sell_asset_type == "native")
                        {
                            ghcl._selling_code = "XLM";
                            ghcl._selling_issuer = "";
                            ghcl._selling_type = sell_asset_type;
                        }
                        else
                        {
                            ghcl._selling_code = sell_asset_code;
                            ghcl._selling_issuer = sell_asset_issuer;
                            ghcl._selling_type = sell_asset_type;
                        }
                        if (buy_asset_type == "native")
                        {
                            ghcl._buying_code = "XLM";
                            ghcl._buying_issuer = "";
                            ghcl._buying_type = buy_asset_type;
                        }
                        else
                        {
                            ghcl._buying_code = buy_asset_code;
                            ghcl._buying_issuer = buy_asset_issuer;
                            ghcl._buying_type = buy_asset_type;
                        }
                        allofferlist.buylist.Add(ghcl);
                    }
                }
                allofferlist.selllist = allofferlist.selllist.OrderBy(c => c._fprice).ToList();
                allofferlist.buylist = allofferlist.buylist.OrderByDescending(c => c._fprice).ToList();
                return new ResultModel<allOffer> { IsSuccess = true, Code = 200, Msg = "成功",Data = allofferlist };
            }
            else
            {
                return new ResultModel<allOffer> {IsSuccess = false,Code = 300 };
            }
            
            
            
        }

        /// <summary>
        /// 挂单
        /// </summary>
        /// <param name="userSeed"></param>
        /// <param name="buycode"></param>
        /// <param name="sellissuingPublicKeys"></param>
        /// <param name="sellcode"></param>
        /// <param name="amount"></param>
        /// <param name="price"></param>
        /// <param name="buyissuingPublicKeys"></param>
        /// <param name="offerid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/buyorselloffer")]
        public async Task<ResultCode> BuyOrSellOffer(string userSeed, string buycode, string sellissuingPublicKeys, string sellcode, string amount, string price, string buyissuingPublicKeys, long offerid)
        {
            //stellar_dotnet_sdk.xdr.ManageSellOfferResult
            Server server = new Server("https://horizon.stellar.org");
            KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
            var sourceAccount = await server.Accounts.Account(userKeys.AccountId);
            //Network.UseTestNetwork();
            Network.UsePublicNetwork();
            try
            {
                //卖单
                if (buycode == "XLM")
                {
                    var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new ManageSellOfferOperation.Builder(new AssetTypeCreditAlphaNum4(sellcode, sellissuingPublicKeys), new AssetTypeNative(), amount, price
                         ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    transaction.Sign(userKeys);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok" + response.Hash };
                    }
                    else
                    {
                        return new ResultCode { IsSuccess = false, Code = 300, Msg = "error" + response.Hash };
                    }
                }
                //买单
                else if (sellcode == "XLM")
                {
                    var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new ManageSellOfferOperation.Builder(new AssetTypeNative(), new AssetTypeCreditAlphaNum4(buycode, buyissuingPublicKeys), amount, price
                         ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    transaction.Sign(userKeys);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok" + response.Hash };
                    }
                    else
                    {
                        return new ResultCode { IsSuccess = false, Code = 300, Msg = "error" + response.Hash };
                    }
                }
                else
                {
                    //var transaction = new TransactionBuilder(sourceAccount)
                    //     .AddOperation(new ManageSellOfferOperation.Builder(new AssetTypeCreditAlphaNum4(sellcode, sellissuingPublicKeys), new AssetTypeCreditAlphaNum4(buycode, buyissuingPublicKeys), amount, price
                    //     ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    //transaction.Sign(userKeys);
                    //var response = await server.SubmitTransaction(transaction);
                    //if (response.Hash != null && response.Hash != "")
                    //{
                    //    return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok" + response.Hash };
                    //}
                    //else
                    //{
                    //    return new ResultCode { IsSuccess = false, Code = 300, Msg = "error" + response.Hash };
                    //}
                    return new ResultCode { IsSuccess = false, Code = 300, Msg = "暂未开放其他交易对" };
                }
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 300, Msg = "error" };
            }
        }

        /// <summary>
        /// 撤单
        /// </summary>
        /// <param name="userSeed">密钥</param>
        /// <param name="buycode">买方资产代号</param>
        /// <param name="sellissuingPublicKeys">卖方资产发行账户</param>
        /// <param name="sellcode">卖方资产代号</param>
        /// <param name="offerid">订单id</param>
        /// <param name="buyissuingPublicKeys">买方资产发行账户</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/deleteoffer")]
        public async Task<ResultCode> DeleteOffer(string userSeed, string buycode, string sellissuingPublicKeys, string sellcode, long offerid, string buyissuingPublicKeys)
        {
            //逻辑:Is MangerSellOffer Or MangerBuyOffer then delete
            Server server = new Server("https://horizon.stellar.org");
            KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
            var sourceAccount = await server.Accounts.Account(userKeys.AccountId);
            //Network.UseTestNetwork();
            Network.UsePublicNetwork();
            try
            {
                string amount = "0";
                string price = "1";
                if (sellcode == "XLM")
                {
                    //await YNBModel.MangerBuyOffer(userSeed, buycode, sellissuingPublicKeys, sellcode, amount, price, offerid, buyissuingPublicKeys);
                    var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new ManageBuyOfferOperation.Builder(new AssetTypeNative(), new AssetTypeCreditAlphaNum4(buycode, buyissuingPublicKeys), amount, price
                         ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    transaction.Sign(userKeys);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash };
                    }
                    else
                    {
                        return new ResultCode { IsSuccess = false, Code = 300, Msg = "网络波动，请稍后再试" };
                    }
                }
                else if (buycode == "XLM")
                {
                    //await YNBModel.MangerSellOffer(userSeed, buycode, sellissuingPublicKeys, sellcode, amount, price, offerid, buyissuingPublicKeys);
                    var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new ManageSellOfferOperation.Builder(new AssetTypeCreditAlphaNum4(sellcode, sellissuingPublicKeys), new AssetTypeNative(), amount, price
                         ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    transaction.Sign(userKeys);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功" + response.Hash };
                    }
                    else
                    {
                        return new ResultCode { IsSuccess = false, Code = 300, Msg = "网络波动，请稍后再试" };
                    }
                }
                else
                {
                    //var transaction = new TransactionBuilder(sourceAccount)
                    //     .AddOperation(new ManageSellOfferOperation.Builder(new AssetTypeCreditAlphaNum4(sellcode, sellissuingPublicKeys), new AssetTypeCreditAlphaNum4(buycode, buyissuingPublicKeys), amount, price
                    //     ).SetOfferId(offerid).SetSourceAccount(userKeys).Build()).Build();
                    //transaction.Sign(userKeys);
                    //var response = await server.SubmitTransaction(transaction);
                    //if (response.Hash != null && response.Hash != "")
                    //{
                    //    return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功" + response.Hash };
                    //}
                    //else
                    //{
                    //    return new ResultCode { IsSuccess = false, Code = 300, Msg = "网络波动，请稍后再试" };
                    //}
                    return new ResultCode { IsSuccess = false, Code = 300, Msg = "暂未开放其他交易对" };
                }

            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 300, Msg = "参数或api地址有误,请核查后再试" };
            }
            //return new ResultCode { IsSuccess = true };
        }

        
    }
}
