using Shopping.Models.Vnpay;

namespace Shopping.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentVnPayUrl(PaymentInfoModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
