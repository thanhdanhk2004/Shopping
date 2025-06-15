using Microsoft.AspNetCore.Mvc;
using Shopping.Models.Momo;
using Shopping.Models.Vnpay;
using Shopping.Services.Momo;
using Shopping.Services.VnPay;

namespace Shopping.Controllers
{
    public class PaymentController : Controller
    {
        private IMomoService _momoService;
        private IVnPayService _vnPayService;
        public PaymentController(IMomoService momoService, IVnPayService vnPayService)
        {
            _momoService = momoService;
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
        {
            var response = await _momoService.CreatePaymentMomoAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpPost]
        public IActionResult CreatePaymentVNPay(PaymentInfoModel model)
        {
            var url = _vnPayService.CreatePaymentVnPayUrl(model, HttpContext);
            return Redirect(url);
        }
    }
}
