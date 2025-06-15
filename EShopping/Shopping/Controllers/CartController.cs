using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class CartController : Controller
    {
        private readonly Context _context;
        public CartController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            
            List<CartItemModel> items = HttpContext.Session.get_Json<List<CartItemModel>>("Cart")?? new List<CartItemModel>();
            //Add shipping tu cookies
            var shipping_price_cookies = Request.Cookies["shipping_price"];
            decimal shipping_price = 0;
            if(shipping_price_cookies != null)
            {
                var shipping_price_json = shipping_price_cookies;
                shipping_price = JsonConvert.DeserializeObject<decimal>(shipping_price_json);
            }

            //
            var coupon_title = Request.Cookies["CouponTitle"];
            CartItemViewModel cart_vm = new()
            {
                CartItems = items,
                GrandTotal = items.Sum(x => x.Quantity*x.Price),
                PriceShipping = shipping_price,
                CouponTitle = coupon_title,
            };

            return View(cart_vm);
        }

        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add(int id)
        {
            ProductModel product = await _context.Products.FindAsync(id);
            List<CartItemModel> items = HttpContext.Session.get_Json<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItems = items.Where(c => c.ProductId == id).FirstOrDefault();

            if(cartItems == null)
            {
                items.Add(new CartItemModel(product));
            }
            else
            {
                cartItems.Quantity += 1;
            }
            HttpContext.Session.set_json("Cart", items);
            return Ok(new { success = true, message = "Add cart success" });
        }

        public async Task<IActionResult> Decreare(int id)
        {
            List<CartItemModel> items = HttpContext.Session.get_Json<List<CartItemModel>>("Cart");

            CartItemModel cartItems = items.Where(c => c.ProductId == id).FirstOrDefault();
            if(cartItems.Quantity > 1)
            {
                --cartItems.Quantity;
            }
            else
            {
                items.RemoveAll(p => p.ProductId == id);
            }

            if(items.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.set_json("Cart", items);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Increare")]
        public async Task<IActionResult> Increare(int id)
        {
            List<CartItemModel> items = HttpContext.Session.get_Json<List<CartItemModel>>("Cart");
            var products = await _context.Products.FindAsync(id);

            CartItemModel cartItems = items.Where(c => c.ProductId == id).FirstOrDefault();
            if (cartItems.Quantity >= 1 && products.Quantity > cartItems.Quantity)
            {
                ++cartItems.Quantity;
            }
            else
            {
                TempData["success"] = "Sold out"; 
            }

            if (items.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.set_json("Cart", items);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            List<CartItemModel> items = HttpContext.Session.get_Json<List<CartItemModel>>("Cart");
            items.RemoveAll(p => p.ProductId == id);

            if (items.Count == 0)
                HttpContext.Session.Remove("Cart");
            else
                HttpContext.Session.set_json("Cart", items);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");   
        }
        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/index.cshtml");
        }


        [HttpPost]
        [Route("Cart/GetShipping")]
        public async Task<IActionResult> GetShipping(string city, string district, string ward)
        {
            var shipping = await _context.Shipings.Where(s => s.City == city && s.District == district && s.Ward == ward).FirstOrDefaultAsync();
            decimal shipping_price = 0;
            if (shipping != null)
            {
                shipping_price = shipping.Price;
            }
            else
            {
                shipping_price = 5000;
            }
            var shipping_price_json = JsonConvert.SerializeObject(shipping_price);
            try
            {
                var cookies_option = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(30),
                    Secure = true
                };
                Response.Cookies.Append("shipping_price", shipping_price_json, cookies_option);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Json(new { shipping_price });
        }

        [HttpPost]
        [Route("Cart/GetCoupon")]
        public async Task<IActionResult> GetCoupon(string code)
        {
            var coupon = await _context.Coupons.Where(c => c.Name == code).FirstOrDefaultAsync();
            string coupon_title = coupon.Name + " | " + coupon?.Description;

            if(coupon_title != null)
            {
                TimeSpan time = coupon.DateExpired - DateTime.Now;
                int day = time.Days;
                if (day >= 0)
                {
                    try
                    {
                        var cookies_option = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTime.Now.AddMinutes(30),
                            Secure = true,
                            SameSite = SameSiteMode.Strict //Kiem tra tuong thich trinh duyet
                        };
                        Response.Cookies.Append("CouponTitle", coupon_title, cookies_option);
                        return Ok(new { success = true, message = "Add coupon success" });
                    }
                    catch (Exception ex)
                    {
                        return Ok(new { success = false, message = "Add coupon fail" });
                    }
                }
                else
                {
                    return Ok(new { success = false, message = "Coupon expired" });
                }
            }
            return Ok(new { success = false, message = "Coupon not found" });
        }
    }
}
