using Shopping.Models.Momo;

namespace Shopping.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentMomoAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteMomoAsync(IQueryCollection collection);
    }
}
