using GlassesShop.Models.ViewModels;
using GlassesShop.Repositories.Interfaces;
using GlassesShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlassesShop.Controllers
{
    [ApiController]
    [Route("api/momo-ipn")]
    public class MoMoWebhookController : ControllerBase
    {
        private readonly MoMoService _moMoService;
        private readonly IOrderRepository _orderRepo;

        public MoMoWebhookController(MoMoService moMoService, IOrderRepository orderRepo)
        {
            _moMoService = moMoService;
            _orderRepo = orderRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] MoMoIpnPayload payload)
        {
            if (payload == null)
                return Ok(new { message = "Payload rỗng" });

            if (!_moMoService.VerifyIpnSignature(payload))
                return BadRequest(new { message = "Chữ ký không hợp lệ" });

            if (!int.TryParse(payload.ExtraData, out var orderId))
                return Ok(new { message = "Không xác định được đơn hàng" });

            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null || order.Payment == null)
                return Ok(new { message = "Không tìm thấy đơn hàng #" + orderId });

            if (order.Payment.PaymentStatus == "Đã thanh toán")
                return Ok(new { message = "Đơn hàng đã được xác nhận trước đó" });

            if (payload.ResultCode == 0)
            {
                await _orderRepo.UpdatePaymentStatusAsync(orderId, "Đã thanh toán", payload.TransId?.ToString());
            }
            else
            {
                await _orderRepo.UpdatePaymentStatusAsync(orderId, "Thất bại");
            }

            return Ok(new { message = "Đã xử lý" });
        }
    }
}