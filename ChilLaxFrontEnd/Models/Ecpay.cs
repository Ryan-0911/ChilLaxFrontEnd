namespace ChilLaxFrontEnd.Models
{
    public class Ecpay
    {
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ReturnURL { get; set; }
        public string OrderResultURL { get; set; }
        public string ClientRedirectURL { get; set; }
        public string MerchantID { get; set; }
        public string IgnorePayment { get; set;}
        public string PaymentType { get; set; }
        public string ChoosePayment { get; set; }
        public int EncryptType { get; set;}
        public string NeedExtraPaidInfo { get; set;}
        public string CheckMacValue { get; set;}
    }
}
