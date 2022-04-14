using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.responses;
using static YNB_DApp.Models.Common;
using static YNB_DApp.Models.ResponseAccount;

namespace YNB_DApp.Controllers.api
{
    //账户操作
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        /// <summary>
        /// 账户信息类
        /// </summary>
        public class AccountInfo
        {
            public string AccountID { get; set; }
            public string AccountPwd { get; set; }
            public string Type { get; set; }
            public string Code { get; set; }
            public string Issuer { get; set; }
            public string Balance { get; set; }
            public string Image { get; set; }
        }
        
        /// <summary>
        /// 创建新公钥密钥对(不注资不添加信任线)
        /// </summary>
        /// <param name="num">默认为1</param>
        [HttpGet]
        [Route("api/creataccount")]
        public ResultModel<List<AccountInfo>> CreatAccount(int num = 1)
        {
            var server = new Server("https://horizon.stellar.org");
            List<AccountInfo> list = new List<AccountInfo>();
            try
            {
                for (var i = 0; i < num; i++)
                {
                    Network.UsePublicNetwork();
                    var pair = KeyPair.Random();
                    list.Add(new AccountInfo
                    {
                        AccountID = pair.AccountId,
                        AccountPwd = pair.SecretSeed
                    });
                }
                return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
            }
            catch 
            {
                return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动"};
            }
        }

        /// <summary>
        /// 对以有密钥对进行注资并添加YNB信任线
        /// </summary>
        /// <param name="fromseed">注资方密钥</param>
        /// <param name="toid"></param>
        /// <param name="toseed"></param>
        /// <param name="startingBalance"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/setaccount")]
        public async Task<ResultCode> SetAccount(string fromseed, string toseed, string startingBalance = "1.51")
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            var sourceAccount = await server.Accounts.Account(source.AccountId);
            //KeyPair Toid = KeyPair.FromAccountId(toid);
            KeyPair Toseed = KeyPair.FromSecretSeed(toseed);

            try
            {
                Network.UsePublicNetwork();
                //HttpClient client = _clientFactory.CreateClient();
                var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new CreateAccountOperation(Toseed, startingBalance)
                         ).Build();
                transaction.Sign(source);
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != null && response.Hash != "")
                {
                    var user = await server.Accounts.Account(Toseed.AccountId);
                    var ChangeTrust = new TransactionBuilder(user)
                            .AddOperation(new ChangeTrustOperation.Builder(
                                new AssetTypeCreditAlphaNum4("YNB", "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3"), "888000000000")
                               .Build())
                            .Build();
                    ChangeTrust.Sign(Toseed);
                    var res = await server.SubmitTransaction(ChangeTrust);
                    if (res.Hash != null && res.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = res.Hash };
                    }
                    else
                    {
                        return new ResultCode { IsSuccess = false, Code = 300, Msg = "注资成功,信任线失败" };
                    }
                }
                else
                {
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "注资失败，请保留参数账号" };
                }
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "网络波动，请稍后再试" };
            }
        }

        /// <summary>
        /// ChangeNewTrust添加其他币种信任线
        /// </summary>
        /// <param name="userSeed">操作者密钥</param>
        /// <param name="code">资产代码</param>
        /// <param name="issuingPublicKeys">发行公钥</param>
        /// <param name="maxTrust">信任线额度(默认最大)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/changenewtrust")]
        public async Task<ResultCode> ChangeNewTrust(string userSeed, string code, string issuingPublicKeys, string maxTrust = "922337203685.477539")
        {
            try
            {
                Server server = new Server("https://horizon.stellar.org");
                KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
                var user = await server.Accounts.Account(userKeys.AccountId);
                Network.UsePublicNetwork();
                if (code.Length > 4)
                {
                    var ChangeTrust_12 = new TransactionBuilder(user)
                    .AddOperation(new ChangeTrustOperation.Builder(
                        new AssetTypeCreditAlphaNum12(code, issuingPublicKeys), maxTrust)
                       .Build())
                    .Build();
                    ChangeTrust_12.Sign(userKeys);
                    var response = await server.SubmitTransaction(ChangeTrust_12);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash };
                    }
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动" };
                }
                else
                {
                    var ChangeTrust_4 = new TransactionBuilder(user)
                    .AddOperation(new ChangeTrustOperation.Builder(
                        new AssetTypeCreditAlphaNum4(code, issuingPublicKeys), maxTrust)
                       .Build())
                    .Build();
                    ChangeTrust_4.Sign(userKeys);
                    var response = await server.SubmitTransaction(ChangeTrust_4);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash };
                    }
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动" };
                }
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 400, Msg = "账号有误" };
            }
        }

        /// <summary>
        /// fundaccount单纯注资
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="startingBalance"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/fundaccount")]
        public async Task<ResultCode> FundAccount(string fromseed, string toid, string startingBalance = "1.51")
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            var sourceAccount = await server.Accounts.Account(source.AccountId);
            KeyPair Toid = KeyPair.FromAccountId(toid);
            //KeyPair Toseed = KeyPair.FromSecretSeed(toseed);
            try
            {
                Network.UsePublicNetwork();
                //HttpClient client = _clientFactory.CreateClient();
                var transaction = new TransactionBuilder(sourceAccount)
                         .AddOperation(new CreateAccountOperation(Toid, startingBalance)
                         ).Build();
                transaction.Sign(source);
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != null && response.Hash != "")
                {
                    return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功" };
                }
                else
                {
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动，请稍后再试" };
                }
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "网络波动，请稍后再试" };
            }

        }

        /// <summary>
        /// setaccount批量创建账号注资创建YNB信任线
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="startingBalance"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ynbsetaccount")]
        public async Task<ResultModel<List<AccountInfo>>> YNBSetAccount(string fromseed, string startingBalance = "1.51", int num = 2)
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            var sourceAccount = await server.Accounts.Account(source.AccountId);
            //Network.UseTestNetwork();
            List<AccountInfo> list = new List<AccountInfo>();
            try
            {
                for (var i = 0; i < num; i++)
                {
                    Network.UsePublicNetwork();
                    //HttpClient client = _clientFactory.CreateClient();
                    var pair = KeyPair.Random();
                    var transaction = new TransactionBuilder(sourceAccount)
                        .AddOperation(new CreateAccountOperation(pair, startingBalance)
                        ).Build();
                    transaction.Sign(source);
                    var response = await server.SubmitTransaction(transaction);
                    AccountResponse account = await server.Accounts.Account(pair.AccountId);
                    list.Add(new AccountInfo
                    {
                        AccountID = pair.AccountId,
                        AccountPwd = pair.SecretSeed
                    });
                    try
                    {
                        KeyPair userKeys = KeyPair.FromSecretSeed(pair.SecretSeed);

                        var user = await server.Accounts.Account(pair.AccountId);
                        var ChangeTrust = new TransactionBuilder(user)
                            .AddOperation(new ChangeTrustOperation.Builder(
                                new AssetTypeCreditAlphaNum4("YNB", "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3"), "888000000000")
                               .Build())
                            .Build();
                        ChangeTrust.Sign(userKeys);
                        await server.SubmitTransaction(ChangeTrust);
                    }
                    catch
                    {
                        return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 300, Msg = "网络波动，请稍后再试" };
                    }
                }
                return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
            }
            catch
            {
                return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动，请稍后再试" };
            }
        }

        /// <summary>
        /// SendPayment转账(XLM)
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="amount"></param>
        /// <param name="Info"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/sendxlm")]
        public async Task<ResultCode> SendXLM(string fromseed, string toid, string amount, string Info = "",int fee = 100)
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            KeyPair destination = KeyPair.FromAccountId(toid);
            uint _fee = (uint)fee;
            //var result = await server.Accounts.Account(destination.AccountId);
            AccountResponse sourceAccount = await server.Accounts.Account(source.AccountId);
            Network.UsePublicNetwork();
            var transaction = new TransactionBuilder(sourceAccount).AddOperation(new PaymentOperation.Builder(destination, new AssetTypeNative(), amount).Build()).AddMemo(
                Memo.Text(Info)
                ).SetFee(_fee).Build();
            // Sign the transaction to prove you are actually the person sending it.
            //手续费方法
            //.SetFee(_fee)
            transaction.Sign(source);
            try
            {
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != null && response.Hash != "")
                {
                    return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash,Data = response.Result.FeeCharged};
                }
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "网络波动" };
            }
            catch (Exception ex)
            {
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "网络波动" };
            }
        }


        /// <summary>
        /// GetAccountBalance查询账号信息
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="fromseed"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getaccountbalance")]
        public async Task<ResultModel<List<AccountInfo>>> GetAccountBalance(string accountid = "",string fromseed = "")
        {
            try
            {
                Server server = new Server("https://horizon.stellar.org");
                Network.UsePublicNetwork();
                List<AccountInfo> list = new List<AccountInfo>();
                if(accountid == "" && fromseed == "")
                {
                    return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 300, Msg = "查询账号不能为空",Data =null };
                }
                else if (accountid != "")
                {
                    KeyPair keypair = KeyPair.FromAccountId(accountid);
                    AccountResponse account = await server.Accounts.Account(keypair.AccountId);
                    foreach (var m in account.Balances)
                    {
                        if(m.AssetType != "native")
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = m.AssetCode,
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }
                        else
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = "XLM",
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }
                        
                    }
                    return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
                }
                else if (accountid == "" && fromseed != "")
                {
                    KeyPair keypair = KeyPair.FromSecretSeed(fromseed);
                    AccountResponse account = await server.Accounts.Account(keypair.AccountId);
                    foreach (var m in account.Balances)
                    {
                        if (m.AssetType != "native")
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = m.AssetCode,
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }
                        else
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = "XLM",
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }
                    }
                    return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
                }
                else
                {
                    return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 300, Msg = "账号有误" };
                }
            }
            catch
            {
                return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };
            }
                
        }

        /// <summary>
        /// GetAccountBalances查询账号信息(带资产图标)
        /// </summary>
        /// <param name="accountid"></param>
        /// <param name="fromseed"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getaccountbalances")]
        public async Task<ResultModel<List<AccountInfo>>> GetAccountBalances(string accountid = "", string fromseed = "")
        {
            try
            {
                var ExplorerController = new ExplorerController();
                Server server = new Server("https://horizon.stellar.org");
                Network.UsePublicNetwork();
                List<AccountInfo> list = new List<AccountInfo>();
                if (accountid == "" && fromseed == "")
                {
                    return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 300, Msg = "查询账号不能为空", Data = null };
                }
                else if (accountid != "")
                {
                    KeyPair keypair = KeyPair.FromAccountId(accountid);
                    AccountResponse account = await server.Accounts.Account(keypair.AccountId);
                    foreach (var m in account.Balances)
                    {
                        if (m.AssetType != "native")
                        {
                            var imageinfo = ExplorerController.SearchAssetImage(m.AssetCode,m.AssetIssuer);
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = m.AssetCode,
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString,
                                Image = imageinfo.Result.Code == 200 ? imageinfo.Result.Data[0].Image.ToString(): ""
                            });
                        }
                        else
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = "XLM",
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }

                    }
                    return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
                }
                else if (accountid == "" && fromseed != "")
                {
                    KeyPair keypair = KeyPair.FromSecretSeed(fromseed);
                    AccountResponse account = await server.Accounts.Account(keypair.AccountId);
                    foreach (var m in account.Balances)
                    {
                        if (m.AssetType != "native")
                        {
                            var imageinfo = ExplorerController.SearchAssetImage(m.AssetCode, m.AssetIssuer);
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = m.AssetCode,
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString,
                                Image = imageinfo.Result.Code == 200 ? imageinfo.Result.Data[0].Image.ToString() : ""
                            });
                        }
                        else
                        {
                            list.Add(new AccountInfo
                            {
                                AccountID = keypair.AccountId,
                                Type = m.AssetType,
                                Code = "XLM",
                                Issuer = m.AssetIssuer,
                                Balance = m.BalanceString
                            });
                        }
                    }
                    return new ResultModel<List<AccountInfo>> { IsSuccess = true, Code = 200, Msg = "成功", Data = list };
                }
                else
                {
                    return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 300, Msg = "账号有误" };
                }
            }
            catch
            {
                return new ResultModel<List<AccountInfo>> { IsSuccess = false, Code = 500, Msg = "网络波动" };
            }

        }

        /// <summary>
        /// SendYNB转账(YNB)
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="amount"></param>
        /// <param name="Info"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/sendYNB")]
        public async Task<ResultCode> SendYNB(string fromseed, string toid, string amount, string Info = "")
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            KeyPair destination = KeyPair.FromAccountId(toid);
            try
            {
                var result = await server.Accounts.Account(destination.AccountId);
                AccountResponse sourceAccount = await server.Accounts.Account(source.AccountId);
                Network.UsePublicNetwork();
                var transaction = new TransactionBuilder(sourceAccount)
                    .AddOperation(new PaymentOperation.Builder(destination, new AssetTypeCreditAlphaNum4("YNB", "GDGWUSHVMXJ5OORRBOCTYJQR3RRT4EBMTE67JX7V3W6GDSJCVLBNDOY3"), amount)
                    .Build())
                    .AddMemo(
                        Memo.Text(Info)
                    ).Build();
                // Sign the transaction to prove you are actually the person sending it.
                transaction.Sign(source);
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != null && response.Hash != "")
                {
                    return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash};
                }
                return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动" };
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "账号错误" };
            }
        }

        /// <summary>
        /// SendPayment转账(其他币种)
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="code"></param>
        /// <param name="issuer"></param>
        /// <param name="amount"></param>
        /// <param name="Info"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/sendpayment")]
        public async Task<ResultCode> SendPayment(string fromseed, string toid,string code,string issuer, string amount, string Info = "")
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            KeyPair destination = KeyPair.FromAccountId(toid);
            try
            {
                var result = await server.Accounts.Account(destination.AccountId);
                AccountResponse sourceAccount = await server.Accounts.Account(source.AccountId);
                Network.UsePublicNetwork();
                if (code.Length > 4)
                {
                    var transaction = new TransactionBuilder(sourceAccount)
                        .AddOperation(new PaymentOperation.Builder(destination, new AssetTypeCreditAlphaNum12(code, issuer), amount)
                        .Build())
                        .AddMemo(
                            Memo.Text(Info)
                        ).Build();
                    // Sign the transaction to prove you are actually the person sending it.
                    transaction.Sign(source);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash };
                    }
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动" };
                }
                else
                {
                    var transaction = new TransactionBuilder(sourceAccount)
                        .AddOperation(new PaymentOperation.Builder(destination, new AssetTypeCreditAlphaNum4(code, issuer), amount)
                        .Build())
                        .AddMemo(
                            Memo.Text(Info)
                        ).Build();
                    // Sign the transaction to prove you are actually the person sending it.
                    transaction.Sign(source);
                    var response = await server.SubmitTransaction(transaction);
                    if (response.Hash != null && response.Hash != "")
                    {
                        return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash };
                    }
                    return new ResultCode { IsSuccess = false, Code = 400, Msg = "网络波动" };
                }
                
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "账号错误" };
            }
        }


        /// <summary>
        /// 单纯XLM账户回收
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/accountmerge")]
        public async Task<ResultCode> AccountMerge(string fromseed, string toid,int fee = 100)
        {
            var server = new Server("https://horizon.stellar.org");
            KeyPair source = KeyPair.FromSecretSeed(fromseed);
            KeyPair destination = KeyPair.FromAccountId(toid);
            uint _fee = (uint)fee;
            //var result = await server.Accounts.Account(destination.AccountId);
            AccountResponse sourceAccount = await server.Accounts.Account(source.AccountId);
            Network.UsePublicNetwork();
            var transaction = new TransactionBuilder(sourceAccount).AddOperation(new AccountMergeOperation.Builder(destination).Build()).SetFee(_fee).Build();
            // Sign the transaction to prove you are actually the person sending it.
            //手续费方法
            //.SetFee(_fee)
            transaction.Sign(source);
            try
            {
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != null && response.Hash != "")
                {
                    return new ResultCode { IsSuccess = true, Code = 200, Msg = "成功", Hash = response.Hash, Data = response.Result.FeeCharged };
                }
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "网络波动" };
            }
            catch (Exception ex)
            {
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "网络波动" };
            }
        }


        /// <summary>
        /// 账户回收(如果非XLM有余额或挂单则失败)
        /// </summary>
        /// <param name="fromseed"></param>
        /// <param name="toid"></param>
        /// <param name="fee"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/accountmergetest")]
        public ResultCode AccountMergetest(string fromseed, string toid, int fee = 100)
        {
            try
            {
                //查询账户资产信息
                var list = GetAccountBalance("",fromseed);
                if (list.Result.IsSuccess && list.Result.Data != null)
                {
                    var lists = list.Result.Data;
                    
                    foreach (var item in lists)
                    {
                        if(item.Code != "XLM")
                        {
                            //删除资产
                            var delres = ChangeNewTrust(fromseed, item.Code, item.Issuer, "0");
                            if (!delres.Result.IsSuccess && delres.Result.Code != 200)
                            {
                                return new ResultCode { IsSuccess = false, Code = 301, Msg = "资产删除出错", Data = "资产代码:" + item.Code + ";资产合约地址:" + item.Issuer };
                            }
                        }
                    }
                }
                else
                {
                    return new ResultCode { IsSuccess = false, Code = 302, Msg = "账户有误", Data = list.Result.Msg };
                }
                //账户回收
                var res = AccountMerge(fromseed, toid, fee);
                if (!res.Result.IsSuccess && res.Result.Code != 200)
                {
                    return new ResultCode { IsSuccess = false, Code = 303, Msg = "回收出错", Data = res.Result.Msg };
                }
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "账号回收成功" };
            }
            catch(Exception ex)
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "网络波动"+ ex };
            }
        }

    }
}
