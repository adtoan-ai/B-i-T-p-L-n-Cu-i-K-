using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GlassesShop.Models;
using Microsoft.Extensions.Options;

namespace GlassesShop.Services
{
    public class MoMoService
    {
        private readonly MoMoOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public MoMoService(IOptions<MoMoOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Models.ViewModels.MoMoCreateResult> CreatePaymentAsync(
            int orderDbId, decimal amount, string orderInfo, string redirectUrl, string ipnUrl)
        {
            var requestId = Guid.NewGuid().ToString();
            var orderId = $"DH{orderDbId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var amountText = ((long)amount).ToString();
            var extraData = orderDbId.ToString();

            var rawSignature =
                $"accessKey={_options.AccessKey}" +
                $"&amount={amountText}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={ipnUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_options.PartnerCode}" +
                $"&redirectUrl={redirectUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={_options.RequestType}";

            var signature = ComputeHmacSha256(rawSignature, _options.SecretKey);

            var requestBody = new
            {
                partnerCode = _options.PartnerCode,
                partnerName = _options.PartnerName,
                storeId = "GlassesShopStore",
                requestId,
                amount = amountText,
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = "vi",
                extraData,
                requestType = _options.RequestType,
                signature
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(_options.Endpoint, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                var resultCode = root.TryGetProperty("resultCode", out var rc) ? rc.GetInt32() : -1;

                if (resultCode == 0 && root.TryGetProperty("payUrl", out var payUrlEl))
                {
                    return new Models.ViewModels.MoMoCreateResult
                    {
                        Success = true,
                        PayUrl = payUrlEl.GetString()
                    };
                }

                var message = root.TryGetProperty("message", out var msgEl) ? msgEl.GetString() : "Lỗi không xác định từ MoMo";

                return new Models.ViewModels.MoMoCreateResult
                {
                    Success = false,
                    Message = message
                };
            }
            catch (Exception ex)
            {
                return new Models.ViewModels.MoMoCreateResult
                {
                    Success = false,
                    Message = "Không thể kết nối tới MoMo: " + ex.Message
                };
            }
        }

        public bool VerifyIpnSignature(Models.ViewModels.MoMoIpnPayload payload)
        {
            var rawSignature =
                $"accessKey={_options.AccessKey}" +
                $"&amount={payload.Amount}" +
                $"&extraData={payload.ExtraData}" +
                $"&message={payload.Message}" +
                $"&orderId={payload.OrderId}" +
                $"&orderInfo={payload.OrderInfo}" +
                $"&orderType={payload.OrderType}" +
                $"&partnerCode={payload.PartnerCode}" +
                $"&payType={payload.PayType}" +
                $"&requestId={payload.RequestId}" +
                $"&responseTime={payload.ResponseTime}" +
                $"&resultCode={payload.ResultCode}" +
                $"&transId={payload.TransId}";

            var expectedSignature = ComputeHmacSha256(rawSignature, _options.SecretKey);

            return string.Equals(expectedSignature, payload.Signature, StringComparison.OrdinalIgnoreCase);
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}