using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DashboardController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var product_count = _context.Products.Count();
            var order_count = _context.Orders.Count();
            var category_count = _context.Categories.Count();
            var user_count = _context.Users.Count();
            ViewBag.ProductCount = product_count;
            ViewBag.OrderCount = order_count;
            ViewBag.CategoryCount = category_count;
            ViewBag.UserCount = user_count;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GetChartData()
        {
            var data = _context.Statistics.
                Select(s => new
                {
                    date = s.DateCreate.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit,
                }).ToList();
            return Json(data);
        }

        [HttpPost]
        [Route("GetChartDataBySelect")]
        public async Task<IActionResult> GetChartDataBySelect(DateTime startDate, DateTime endDate)
        {
            var data = _context.Statistics
                .Where(s => s.DateCreate >= startDate && s.DateCreate <= endDate)
                .Select(s => new
                {
                    date = s.DateCreate.ToString("yyyy-MM-dd"),
                    sold = s.Sold,
                    quantity = s.Quantity,
                    revenue = s.Revenue,
                    profit = s.Profit,
                }).ToList();
            return Json(data);
        }


        [HttpPost]
        [Route("FilterDate")]
        public async Task<IActionResult> FilterDate(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Statistics.AsQueryable();
            if (startDate.HasValue)
                query = query.Where(s => s.DateCreate >= startDate);
            if(endDate.HasValue)
                query = query.Where(s => s.DateCreate <= endDate);
            var data = query.Select(s => new {
                date = s.DateCreate.ToString("yyyy-MM-dd"),
                sold = s.Sold,
                quantity = s.Quantity,
                revenue = s.Revenue,
                profit = s.Profit,
            }).ToList();
            return Json(data);
        }
    }
}
