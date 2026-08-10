using System.Text.Json.Serialization;

namespace GlassesShop.Models.ViewModels
{
    public class MoMoCreateResult
    {
        public bool Success { get; set; }
        public string? PayUrl { get; set; }
        public string? Message { get; set; }
    }

    public class MoMoIpnPayload
    {
        public string? PartnerCode { get; set; }
        public string? OrderId { get; set; }
        public string? RequestId { get; set; }
        public long Amount { get; set; }
        public string? OrderInfo { get; set; }
        public string? OrderType { get; set; }
        public long? TransId { get; set; }

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        public string? Message { get; set; }
        public string? PayType { get; set; }
        public long ResponseTime { get; set; }
        public string? ExtraData { get; set; }
        public string? Signature { get; set; }
    }
}