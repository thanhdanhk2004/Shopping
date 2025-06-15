using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly Context _context;
        public OrderController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Orders.ToList());
        }

        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var order_details = await _context.OrderDetails.Include(o => o.Product).Where(o => o.OrderCode == ordercode).ToListAsync();
            var shipping_cost = await _context.Orders.Where(o => o.OrderCode == ordercode).FirstOrDefaultAsync();
            ViewBag.ShippingCost = shipping_cost.PriceShipping;
            var order = await _context.Orders.Where(o => o.OrderCode == ordercode).FirstOrDefaultAsync();
            ViewBag.OrderStatus = order.Status;
            return View(order_details);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(int status, string ordercode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            order.Status = status;
            if(status  == 0)
            {
                var detail_order = await _context.OrderDetails
                    .Include(o => o.Product)
                    .Where(od => od.OrderCode == order.OrderCode)
                    .Select(od => new {
                        od.Quantity,
                        od.Product.Price,
                        od.Product.CapitalPrice,
                    }).ToListAsync();
                var statistis_date =await _context.Statistics
                    .FirstOrDefaultAsync(s => s.DateCreate.Date == order.CreatedDate.Date);
                if(statistis_date != null)
                {
                    foreach(var od in detail_order)
                    {
                        statistis_date.Quantity +=1;
                        statistis_date.Sold += od.Quantity;
                        statistis_date.Revenue += od.Quantity * od.Price;
                        statistis_date.Profit += od.Price - od.CapitalPrice;
                    }
                    _context.Statistics.Update(statistis_date);
                    
                }
                else
                {
                    StatisticModel statistic = new StatisticModel
                    {
                        Quantity = 0,
                        Sold = 0,
                        Revenue = 0,
                        Profit = 0,
                        DateCreate = DateTime.Now,
                    };
                    foreach (var od in detail_order)
                    {
                        statistic.Quantity += 1;
                        statistic.Sold += od.Quantity;
                        statistic.Revenue += od.Quantity * od.Price;
                        statistic.Profit += od.Price - od.CapitalPrice;
                    }
                    _context.Statistics.Add(statistic);

                }
            }
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new {success = true, message="Order status update success"});
            } catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the order status");
            }
            return View();
        }

        [HttpGet]
        [Route("PaymentMomoInfo")]
        public async Task<IActionResult> PaymentMomoInfo(string orderId)
        {
            var payment_momo = await _context.MomoInfors.Where(m => m.OrderId == orderId).FirstOrDefaultAsync();
            if(payment_momo == null)
            {
                return NotFound();
            }
            return View(payment_momo);
        }

        [HttpGet]
        [Route("PaymentVnPayInfo")]
        public async Task<IActionResult> PaymentVnPayInfo(string orderId)
        {
            var payment_vnpay = await _context.VnPayModels.Where(m => m.OrderId == orderId).FirstOrDefaultAsync();
            if (payment_vnpay == null)
            {
                return NotFound();
            }
            return View(payment_vnpay);
        }
    }
}
