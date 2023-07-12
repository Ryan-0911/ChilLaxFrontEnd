using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;
using System.Security.Cryptography;


namespace ChilLaxFrontEnd.Controllers
{
    public class Checkout : Controller
    {
        public ActionResult Index()
        {
            var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            //需填入你的網址
            var website = $"https://localhost:44385/";
            var order = new Dictionary<string, string>
            {
                //綠界需要的參數

                //訂單編號
                { "MerchantTradeNo",  orderId},
                //交易時間
                { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                //交易金額
                { "TotalAmount",  "100"},
                //交易描述
                { "TradeDesc",  "無"},
                //商品名稱
                { "ItemName",  "測試商品"},
                //付款完成通知回傳網址
                { "ReturnURL",  $"{website}/api/Ecpay/AddPayInfo"},
                //Client端回傳付款結果網址
                { "OrderResultURL", $"{website}/Home/PayInfo/{orderId}"},
                //Client端返回特店的按鈕連結
                { "ClientRedirectURL",  $"{website}/Home/Index"},
                //特店編號
                { "MerchantID",  "2000132"},
                //付款方式
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
                //交易類型
                { "PaymentType",  "aio"},
                //預設付款方式
                { "ChoosePayment",  "ALL"},
                //CheckMacValue加密類型
                { "EncryptType",  "1"},
                //是否需要額外的付款資訊
                { "NeedExtraPaidInfo", "Y"}
            };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            var checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";
            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
