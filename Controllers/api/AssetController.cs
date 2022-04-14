using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.requests;
using static YNB_DApp.Models.Common;

namespace YNB_DApp.Controllers.api
{
    //币种操作(代发、空头、销毁...)
    [ApiController]
    public class AssetController : ControllerBase
    {
        /// <summary>
        /// 空投
        /// </summary>
        /// <param name="userSeed"></param>
        /// <param name="code"></param>
        /// <param name="issuingPublicKeys"></param>
        /// <param name="amount"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/createclaimablebalance")]
        public async Task<ResultCode> CreateClaimableBalance(string userSeed, string code, string issuingPublicKeys, string amount,string id)
        {
            try
            {
                Server server = new Server("https://horizon.stellar.org");
                KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
                var user = await server.Accounts.Account(userKeys.AccountId);
                Network.UsePublicNetwork();
                //索赔
                //var claimant = new Claimant {

                //};
                ////地址
                //claimant.Destination = KeyPair.FromAccountId(id);
                ////谓词
                //claimant.Predicate = ClaimPredicate.Unconditional();

                //var array = new Claimant[1];
                List<Claimant> array = new List<Claimant>();
               
                array.Add(new Claimant { Destination = KeyPair.FromAccountId(id), Predicate = ClaimPredicate.Unconditional() });
                
                //array[0].Destination = KeyPair.FromAccountId(id);
                //array[0].Predicate = ClaimPredicate.Unconditional();
                var transaction = new TransactionBuilder(user)
                            .AddOperation(new CreateClaimableBalanceOperation.Builder(
                                new AssetTypeCreditAlphaNum4(code, issuingPublicKeys), amount, array.ToArray())
                                .Build())
                            .Build();
                transaction.Sign(userKeys);
                var response = await server.SubmitTransaction(transaction);
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok", Hash = response.Hash };
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "错误"};
            }
            
        }


        /// <summary>
        /// 空头测试
        /// </summary>
        /// <param name="userSeed"></param>
        /// <param name="code"></param>
        /// <param name="issuingPublicKeys"></param>
        /// <param name="amount"></param>
        /// <param name="id"></param>
        /// <param name="seconds">时间限制/秒</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/createclaimablebalancetest")]
        public async Task<ResultCode> CreateClaimableBalanceTest(string userSeed, string code, string issuingPublicKeys, string amount, string id,long seconds)
        {
            try
            {
                Server server = new Server("https://horizon.stellar.org");
                KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
                var user = await server.Accounts.Account(userKeys.AccountId);
                Network.UsePublicNetwork();
                //索赔
                //var claimant = new Claimant {

                //};
                ////地址
                //claimant.Destination = KeyPair.FromAccountId(id);
                ////谓词
                //claimant.Predicate = ClaimPredicate.Unconditional();

                //var array = new Claimant[1];
                List<Claimant> array = new List<Claimant>();

                //array.Add(new Claimant { Destination = KeyPair.FromAccountId(id), Predicate = ClaimPredicate.Unconditional() });
                array.Add(new Claimant { Destination = KeyPair.FromAccountId(id), Predicate = ClaimPredicate.BeforeRelativeTime(seconds) });

                //array[0].Destination = KeyPair.FromAccountId(id);
                //array[0].Predicate = ClaimPredicate.Unconditional();
                var transaction = new TransactionBuilder(user)
                            .AddOperation(new CreateClaimableBalanceOperation.Builder(
                                new AssetTypeCreditAlphaNum4(code, issuingPublicKeys), amount, array.ToArray())
                                .Build())
                            .Build();
                transaction.Sign(userKeys);
                var response = await server.SubmitTransaction(transaction);
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok", Hash = response.Hash};
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "错误" };
            }

        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <param name="userSeed"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/createclaimablebalancetest1")]
        public async Task<ResultCode> CreateClaimableBalanceTest1(string userSeed,string id)
        {
            try
            {
                Server server = new Server("https://horizon.stellar.org");
                KeyPair userKeys = KeyPair.FromSecretSeed(userSeed);
                var user = await server.Accounts.Account(userKeys.AccountId);
                //var balances = await server.ClaimableBalances.ForClaimant(userKeys).Limit(1).Order(stellar_dotnet_sdk.requests.OrderDirection.DESC);

                Network.UsePublicNetwork();
                
                var transaction = new TransactionBuilder(user)
                            .AddOperation( new ClawbackClaimableBalanceOperation.Builder(id).Build())
                            .Build();
                transaction.Sign(userKeys);
                var response = await server.SubmitTransaction(transaction);
                if (response.Hash != "" || response.Hash != null)
                {
                    return new ResultCode { IsSuccess = false, Code = 300, Msg = "错误" };
                }
                return new ResultCode { IsSuccess = true, Code = 200, Msg = "ok", Hash = response.Hash};
            }
            catch
            {
                return new ResultCode { IsSuccess = false, Code = 500, Msg = "错误" };
            }

        }



        [HttpGet]
        [Route("api/test")]
        public string Test()
        {


            return "";
        }
    }
}
