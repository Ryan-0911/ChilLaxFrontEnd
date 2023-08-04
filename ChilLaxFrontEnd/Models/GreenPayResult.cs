namespace ChilLaxFrontEnd.Models
{
    public class GreenPayResult
    {
        public string? MerchantID { get; set; }
        public string? MerchantTradeNo { get; set; }
        public string? StoreID { get; set; }
        public int? RtnCode { get; set; }
        public string? RtnMsg { get; set; }
        public string? TradeNo { get; set; }
        public int? TradeAmt { get; set; }
    }
}
