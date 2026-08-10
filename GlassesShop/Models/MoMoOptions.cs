namespace GlassesShop.Models
{
    public class MoMoOptions
    {
        public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
        public string PartnerCode { get; set; } = "MOMO";
        public string AccessKey { get; set; } = "F8BBA842ECF85";
        public string SecretKey { get; set; } = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        public string PartnerName { get; set; } = "GlassesShop";
        public string RequestType { get; set; } = "captureWallet";
    }
}