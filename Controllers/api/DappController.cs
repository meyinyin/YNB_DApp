using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static YNB_DApp.Controllers.api.ExplorerController;
using static YNB_DApp.Models.Common;

namespace YNB_DApp.Controllers.api
{
    
    [ApiController]
    public class DappController : ControllerBase
    {
        /// <summary>
        /// dapp详情类
        /// </summary>
        public class dapplist
        {
            public string Title { get; set; }
            public string Src { get; set; }
            public string Color { get; set; }
            public string ImageSrc { get; set; }
        }
        /// <summary>
        /// 推荐DApp
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/dapplist")]
        public ResultModel<List<dapplist>> DappList()
        {
            var list = new List<dapplist>();
            var dapp1 = new dapplist();
            dapp1.Title = "StellarTerm";
            dapp1.Src = "https://stellarterm.com/exchange/YNB-GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3/XLM-native";
            dapp1.Color = "#68c86f";
            dapp1.ImageSrc = "";
            list.Add(dapp1);

            return new ResultModel<List<dapplist>> { IsSuccess = true,Msg = "",Code = 200 ,Data = list };
        }

        /// <summary>
        /// 推荐币种
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/tjassetlist")]
        public ResultModel<List<Explorer_AssetInfo>> TjAssetList()
        {
            var infolist = new List<Explorer_AssetInfo>();
            var info1 = new Explorer_AssetInfo();
            info1.Code = "YNB";info1.Issuer = "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3";info1.Type = "credit_alphanum4";
            info1.Image = "https://ynbs.xyz/logo/ynb.png";info1.Toml = "https://ynbs.xyz/.well-known/stellar.toml";
            infolist.Add(info1);
            var info2 = new Explorer_AssetInfo();
            info2.Code = "USDC"; info2.Issuer = "GDWWN7NIRXR3T63HPAOMZSCKINN56BL7HTDWYUGLLRGQVSL2FWLE2XWZ"; info2.Type = "credit_alphanum4";
            info2.Image = "https://www.centre.io/images/usdc/usdc-icon-86074d9d49.png";
            infolist.Add(info2);
            var info3 = new Explorer_AssetInfo();
            info3.Code = "BTC"; info3.Issuer = "GAUTUYY2THLF7SGITDFMXJVYH3LHDSMGEAKSBU267M2K7A3W543CKUEF"; info3.Type = "credit_alphanum4";
            info3.Image = "https://apay.io/public/logo/btc.svg";info3.Toml = "https://apay.io/.well-known/stellar.toml";
            infolist.Add(info3);
            var info4 = new Explorer_AssetInfo();
            info4.Code = "ETH"; info4.Issuer = "GBDEVU63Y6NTHJQQZIKVTC23NWLQVP3WJ2RI2OTSJTNYOIGICST6DUXR"; info4.Type = "credit_alphanum4";
            info4.Image = "https://apay.io/public/logo/eth.png";info4.Toml = "https://apay.io/.well-known/stellar.toml";
            infolist.Add(info4);
            var info5 = new Explorer_AssetInfo();
            info5.Code = "USDT"; info5.Issuer = "GCQTGZQQ5G4PTM2GL7CDIFKUBIPEC52BROAQIAPW53XBRJVN6ZJVTG6V"; info5.Type = "credit_alphanum4";
            info5.Image = "https://apay.io/public/logo/usdt.svg"; info5.Toml = "https://apay.io/.well-known/stellar.toml";
            infolist.Add(info5);
            return new ResultModel<List<Explorer_AssetInfo>> {IsSuccess = true,Code = 200,Msg = "成功",Data = infolist };
        }
       
        public class OfferAsset { 
            public string FCode { get; set; }
            public string FIssuer { get; set; }
            public string FType { get; set; }
            public string Code { get; set; }
            public string Issuer { get; set; }
            public string Type { get; set; }
            public string Sort { get; set; }
        }
        /// <summary>
        /// 推荐交易对
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/offerassetlist")]
        public ResultModel<List<OfferAsset>> OfferAssetList()
        {
            var infolist = new List<OfferAsset>();
            var info1 = new OfferAsset();
            info1.Sort = "XLM";
            info1.FCode = "XLM";
            info1.FIssuer = "";
            info1.FType = "native";
            info1.Code = "YNB";
            info1.Issuer = "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3";
            info1.Type = "credit_alphanum4";
            infolist.Add(info1);
            var info2 = new OfferAsset();
            info2.Sort = "XLM";
            info2.FCode = "XLM";
            info2.FIssuer = "";
            info2.FType = "native";
            info2.Code = "USDC";
            info2.Issuer = "GDWWN7NIRXR3T63HPAOMZSCKINN56BL7HTDWYUGLLRGQVSL2FWLE2XWZ";
            info2.Type = "credit_alphanum4";
            infolist.Add(info2);
            var info3 = new OfferAsset();
            info3.Sort = "XLM";
            info3.FCode = "XLM";
            info3.FIssuer = "";
            info3.FType = "native";
            info3.Code = "BTC";
            info3.Issuer = "GAUTUYY2THLF7SGITDFMXJVYH3LHDSMGEAKSBU267M2K7A3W543CKUEF";
            info3.Type = "credit_alphanum4";
            infolist.Add(info3);
            var info4 = new OfferAsset();
            info4.Sort = "BTC";
            info4.FCode = "XLM";
            info4.FIssuer = "";
            info4.FType = "native";
            info4.Code = "BTC";
            info4.Issuer = "GAUTUYY2THLF7SGITDFMXJVYH3LHDSMGEAKSBU267M2K7A3W543CKUEF";
            info4.Type = "credit_alphanum4";
            infolist.Add(info4);
            var info5 = new OfferAsset();
            info5.Sort = "YNB";
            info5.FCode = "XLM";
            info5.FIssuer = "";
            info5.FType = "native";
            info5.Code = "YNB";
            info5.Issuer = "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3";
            info5.Type = "credit_alphanum4";
            infolist.Add(info5);
            return new ResultModel<List<OfferAsset>> { IsSuccess = true, Code = 200, Msg = "成功", Data = infolist };

        }


    }
}
