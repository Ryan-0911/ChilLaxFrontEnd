﻿using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ChilLaxFrontEnd.Models;
using System.Linq;
using ChilLaxFrontEnd.Models.DTO;

namespace ChilLaxFrontEnd.Controllers
{
    public class Checkout : Controller
    {
        private readonly ChilLaxContext _context;
        public Checkout(ChilLaxContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<string>> Index()
        {
            ChilLaxContext db = new ChilLaxContext();

            //產生隨機亂數
            string guid_num = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 13);
            string this_products = string.Empty;
            string orderId = "ChilLax" + $"{guid_num}";
            string msg = "備註欄";
            //需填入你的網址
            string website = $"https://localhost:7189";

            //取得最新一筆訂單
            string maxOrderId = await db.ProductOrders.MaxAsync(p => p.OrderId);
            //ProductOrder? this_order = db.ProductOrders.FirstOrDefault(p => p.OrderId == maxOrderId);
            List<ProductOrderDetailDTO> productOrderDetails = await db.ProductOrders
               .Where(o => o.OrderId == maxOrderId)
               .Join(db.OrderDetails, po => po.OrderId, od => od.OrderId, (po, od) => new { ProductOrder = po, OrderDetail = od })
               .Join(db.Products, od => od.OrderDetail.ProductId, p => p.ProductId, (od, p) => new ProductOrderDetailDTO
               {
                   ProductOrder = od.ProductOrder,
                   OrderDetail = od.OrderDetail,
                   Product = p
               }).ToListAsync();

            foreach (var productOrderDetail in productOrderDetails)
            {
                this_products += $"{productOrderDetail.Product?.ProductName}/";
            }

            

            var order = new Dictionary<string, string>
            {
                //綠界需要的參數

                //訂單編號，測試階段為避免重複以亂數產稱
                { "MerchantTradeNo",  orderId},
                //交易時間
                { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                //交易金額
                { "TotalAmount",  $"{productOrderDetails.FirstOrDefault() ?.ProductOrder?.OrderTotalPrice}"},
                //交易描述
                { "TradeDesc",  $"{msg}"},
                //商品名稱
                { "ItemName",  $"{this_products}"},
                //付款完成通知回傳網址
                { "ReturnURL",  $"{website}/api/Ecpay/AddPayInfo"},
                //Client端回傳付款結果網址(交易完成後須提供一隻API修改付款狀態，將未付款改成已付款)
                { "OrderResultURL", $"{website}/Checkout/UpdatePayment/{maxOrderId}"},
                //Client端返回特店的按鈕連結
                { "ClientRedirectURL",  $"{website}"},
                //特店編號(綠界提供測試商店編號)
                { "MerchantID",  "2000132"},
                //付款方式
                { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},
                //交易類型(固定填aio)
                { "PaymentType",  "aio"},
                //預設付款方式
                { "ChoosePayment",  "ALL"},
                //CheckMacValue加密類型(固定填1)
                { "EncryptType",  "1"},
                //是否需要額外的付款資訊(Y/N)
                { "NeedExtraPaidInfo", "Y"}
            };

            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }
        //Checkout/UpdatePayment/1
        [HttpGet]
        public async Task<string> UpdatePaymentAsync(int? id)
        {
            if (id == null || _context.ProductOrders == null)
                return "付款失敗";

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    ProductOrder productOrder = await _context.ProductOrders.FirstOrDefaultAsync(po => po.OrderId == id.ToString());

                    if (productOrder == null)
                        return "找不到該訂單";

                    //修改付款狀態
                    productOrder.OrderPayment = true;
                    _context.ProductOrders.Update(productOrder);
                    await _context.SaveChangesAsync();

                    //新增點數回饋
                    PointHistory pointHistory = new PointHistory();
                    pointHistory.ModifiedSource = "product";
                    pointHistory.MemberId = productOrder.MemberId;
                    pointHistory.PointDetailId = productOrder.OrderId;
                    pointHistory.ModifiedAmount =(int)Math.Floor(productOrder.OrderTotalPrice / 10.0);
                    _context.PointHistories.Add(pointHistory);
                    await _context.SaveChangesAsync();

                    // 執行其他非同步資料庫操作...

                    // 提交交易
                    await transaction.CommitAsync();

                    return "付款成功";
                }
                catch (Exception ex)
                {
                    // 發生例外時回滾交易
                    await transaction.RollbackAsync();
                    return "付款失敗：" + ex.Message;
                }
            }
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
