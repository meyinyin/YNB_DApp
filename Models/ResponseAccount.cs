using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YNB_DApp.Models
{
    public class ResponseAccount
    {
        /// <summary>
        /// 返回值接受类1
        /// </summary>
        public class GetHttpClassModel
        {
            public object _embedded { get; set; }
            public object _links { get; set; }
            //public  List<Records> records { get; set; }
        }

        /// <summary>
        /// 挂单返回值接受类2
        /// </summary>
        public class Embedded
        {
            public List<Records> records { get; set; }
        }
        /// <summary>
        /// 币种返回之接受类
        /// </summary>
        public class AssetEmbedded
        {
            public List<AssetRecords> records { get; set; }
        }
        /// <summary>
        /// 转账返回值接收类
        /// </summary>
        public class PaymentEmbedded
        {
            public List<AccountPaymentRecords> records { get; set; }
        }
        /// <summary>
        /// 挂单返回值接受类3
        /// </summary>
        public class Records
        {
            public object _links { get; set; }
            public object self { get; set; }
            public string href { get; set; }
            public object offer_maker { get; set; }
            public string id { get; set; }
            public string paging_token { get; set; }
            public string seller { get; set; }
            public object selling { get; set; }
            public object buying { get; set; }
            public string amount { get; set; }
            public object price_r { get; set; }
            public long n { get; set; }
            public long d { get; set; }
            public string price { get; set; }
            public long last_modified_ledger { get; set; }
            public string last_modified_time { get; set; }




        }

        /// <summary>
        /// 自定义挂单类1
        /// </summary>
        public class BuyOrSell
        {
            public string asset_type { get; set; }
            public string asset_code { get; set; }
            public string asset_issuer { get; set; }
        }

        /// <summary>
        /// 自定义挂单类2
        /// </summary>
        public class GetHttpClassList
        {
            /// <summary>
            /// 交易数量
            /// </summary>
            public string _amount { get; set; }
            public decimal _famount { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string _buying_code { get; set; }
            public string _buying_issuer { get; set; }
            public string _buying_type { get; set; }
            public string _selling_code { get; set; }
            public string _selling_issuer { get; set; }
            public string _selling_type { get; set; }
            /// <summary>
            /// 挂单id
            /// </summary>
            public string _offerid { get; set; }
            public string _lastdate { get; set; }
            /// <summary>
            /// 价格
            /// </summary>
            public string _price { get; set; }
            public decimal _fprice { get; set; }
        }

        /// <summary>
        /// 币种信息查询类1
        /// </summary>
        public class AssetRecords
        {
            public object _links { get; set; }
            public object toml { get; set; }
            public string href { get; set; }
            public string asset_type { get; set; }
            public string asset_code { get; set; }
            public string asset_issuer { get; set; }
            public string paging_token { get; set; }
            public object accounts { get; set; }
            public int authorized { get; set; }
            public int authorized_to_maintain_liabilities { get; set; }
            public int unauthorized { get; set; }
            public int num_claimable_balances { get; set; }
            public string amount { get; set; }
            public object balances { get; set; }
            public string claimable_balances_amount { get; set; }
            public int num_accounts { get; set; }
            public object flags { get; set; }
            public bool auth_required { get; set; }
            public bool auth_revocable { get; set; }
            public bool auth_immutable { get; set; }
            public bool auth_clawback_enabled { get; set; }
        }
        /// <summary>
        /// 币种信息查询类2(toml)
        /// </summary>
        public class AssetRecordsToml {
            public object toml { get; set; }
        }
        /// <summary>
        /// 币种信息查询类3(toml.href)
        /// </summary>
        public class AssetRecordsTomlHref
        {
            public string href { get; set; }
        }
        /// <summary>
        /// 账户所有操作查询类(废弃)
        /// </summary>
        public class AccountRecords { 
            public object _links { get; set; }
            public object self { get; set; }
            public string href { get; set; }
            public string id { get; set; }
            public string paging_token { get; set; }
            public bool transaction_successful { get; set; }
            public string source_account { get; set; }
            public string type { get; set; }
            public int type_i { get; set; }
            public string created_at { get; set; }
            public string transaction_hash { get; set; }
            public string starting_balance { get; set; }
            public string funder { get; set; }
            public string account { get; set; }
            public string asset_type { get; set; }
            public string asset_code { get; set; }
            public string asset_issuer { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string limit { get; set; }
            public string trustee { get; set; }
            public string trustor { get; set; }
            public string price { get; set; }
            public object price_r { get; set; }
            public string buying_asset_type { get; set; }
            public string buying_asset_code { get; set; }
            public string buying_asset_issuer { get; set; }
            public string selling_asset_type { get; set; }
            public string selling_asset_code { get; set; }
            public string selling_asset_issuer { get; set; }
            public string offer_id { get; set; }
            public string sponsor { get; set; }
        }
    
        /// <summary>
        /// 账户转入转出查询类
        /// </summary>
        public class AccountPaymentRecords
        {
            public object _links { get; set; }
            public object id { get;}
            public string paging_token { get;}
            public bool transaction_successful { get; }
            public string source_account { get; }
            public string type { get; set; }
            public int type_i { get; }
            public string created_at { get; set; }
            public string transaction_hash { get; set; }
            public string asset_type { get; set; }
            public string asset_code { get; set; }
            public string asset_issuer { get; set; }
            public string from { get; set; }
            public string to { get; set; }
            public string amount { get; set; }

        }
        
        /// <summary>
        /// 区块哈希值查询接收类
        /// </summary>
        public class PaymentInfoRecords
        {
            public string memo { get; set; }
            public string memo_bytes { get; set; }
            public object _links { get; set; }
            public string id { get; set; }
            public string paging_token { get; set; }
            public string source_account { get; set; }
            public string hash { get; set; }
            public bool successful { get; set; }
            public string ledger { get; set; }
            public string created_at { get; set; }
            public string fee_charged { get; set; }
            public string source_account_sequence { get; set; }
            public string fee_account { get; set; }
            public string max_fee { get; set; }
            public int operation_count { get; set; }
            public string envelope_xdr { get; set; }
            public string result_xdr { get; set; }
            public string result_meta_xdr { get; set; }
            public string fee_meta_xdr { get; set; }
            public string memo_type { get; set; }
            public string[] signatures { get; set; }
        }
    }
}
