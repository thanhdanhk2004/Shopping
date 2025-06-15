using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shopping.Areas.Admin.Reponsitory;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Reponitory;
using Shopping.Services.Momo;
using Shopping.Services.VnPay;
using System.Security.Claims;

namespace Shopping.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly Context _context;
        private readonly IEmailSender _emailSender;
        private IMomoService _momoService;
        private IVnPayService _vpnPayService;
        public CheckoutController(Context context, IEmailSender emailSender, IMomoService momoService, IVnPayService vnPayService)
        {
            _context = context;
            _emailSender = emailSender;
            _momoService = momoService;
            _vpnPayService = vnPayService;
        }
        
        public async Task<IActionResult> Checkout(string payment_method)
        {
            var user_email = User.FindFirstValue(ClaimTypes.Email);
            if(user_email == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var odercode = Guid.NewGuid().ToString();
            //Add shipping tu cookies
            var shipping_price_cookies = Request.Cookies["shipping_price"];
            decimal shipping_price = 0;
            if (shipping_price_cookies != null)
            {
                var shipping_price_json = shipping_price_cookies;
                shipping_price = JsonConvert.DeserializeObject<decimal>(shipping_price_json);
            }

            var coupon_cookies = Request.Cookies["CouponTitle"];
            var oder_item = new OrderModel
            {
                Username = user_email,
                OrderCode = odercode,
                CreatedDate = DateTime.Now,
                Status = 1,
                PriceShipping = shipping_price,
                CouponCode = coupon_cookies,
                
            };
            if (payment_method != null)
            {
                oder_item.PaymentMethod = payment_method;
            }
            else
                oder_item.PaymentMethod = "COD";

            _context.Orders.Add(oder_item);
            _context.SaveChanges();

            List<CartItemModel> carts = HttpContext.Session.get_Json<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            foreach (var cart in carts)
            {
                OrderDetailModel order_detail = new OrderDetailModel
                {
                    Username = user_email,
                    OrderCode = odercode,
                    ProductId = cart.ProductId,
                    Price = cart.Price,
                    Quantity = cart.Quantity,
                };
                _context.OrderDetails.Add(order_detail);
                _context.SaveChanges();

                var product = await _context.Products.FindAsync(cart.ProductId);
                if(product != null)
                {
                    product.Quantity = product.Quantity - order_detail.Quantity;
                    product.Sold += order_detail.Quantity;
                    _context.Products.Update(product);
                    _context.SaveChanges();
                }
            }
            HttpContext.Session.Remove("Cart");
            //Gui mail
            //var receiver = "a@gmail.com";
            //var subject = "Order";
            //var message = "Order success";
            //await _emailSender.SendEmailAsync(receiver, subject, message);

            TempData["success"] = "Add order success";
            return RedirectToAction("History", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInforModel model)
        {
            var response = _momoService.PaymentExecuteMomoAsync(HttpContext.Request.Query);
            var request_query = HttpContext.Request.Query;
            if (request_query["resultCode"] != 0)
            {
                var MomoInfor = new MomoInforModel
                {
                    OrderId = request_query["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(request_query["Amount"]),
                    OrderInfo = request_query["orderInfo"],
                    DatePaid = DateTime.Now,
                };
                _context.MomoInfors.Add(MomoInfor);
                await _context.SaveChangesAsync();
                await Checkout("MoMo " + model.OrderId);
            }
            else
            {
                TempData["success"] = "Transaction failure";
                return RedirectToAction("Index", "Cart");
            }
            TempData["success"] = "Payment success";
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBackVnPay()
        {

            var response = _vpnPayService.PaymentExecute(HttpContext.Request.Query);

            if (response.VnPayResponseCode == "00")
            {
                var model = new VnPayModel
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId,
                    DateCreated = DateTime.Now,
                };
                _context.VnPayModels.Add(model);
                await _context.SaveChangesAsync();
                await Checkout("VnPay " + model.OrderId);
            }
            else
            {
                TempData["success"] = "Transaction failure";
                return RedirectToAction("Index", "Cart");
            }
            TempData["success"] = "Payment success";
            return View(response);
        }
    }
}
