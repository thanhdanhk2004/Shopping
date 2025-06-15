using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Models.ViewModels;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;
        private UserManager<AppUserModel> _userManager;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, Context context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

      

        public async Task<IActionResult> Index(int page = 1)
        {
            var products = await _context.Products.Include("Category").Include("Brand").ToListAsync();
            if (page < 0)
                page = 1;

            var pager = new Paginate(products.Count, page);
            int rec_skip = (page - 1) * 6;
            var data = products.Skip(rec_skip).Take(6).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
                return View("NotFound");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        [Route("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var products = await _context.Products.Include("Category").Include("Brand").Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword)).ToListAsync();
            ViewBag.Keyword = keyword;
            return View(products);
        }

        [HttpGet]
        [Route("Detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = _context.Products.Include(p => p.Ratings).Where(p => p.Id == id).FirstOrDefault();
            var products = await _context.Products.Include("Brand").Include("Category").Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id).Take(3).ToListAsync();
            ViewBag.Products = products;

            var product_detail_view_model = new ProductDetailsViewModel
            {
                Product = product,
            };
            return View(product_detail_view_model);
        }

        public IActionResult Contact()
        {
            return View(_context.Contacts.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> AddWishList(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            WishListModel wish_list = new WishListModel
            {
                ProductId = id,
                UserId = user.Id,
            };
            _context.WishList.Add(wish_list);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to wish list success" });
            }catch(Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCompare(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            CompareModel compare = new CompareModel
            {
                ProductId = id,
                UserId = user.Id,
            };
            _context.Compares.Add(compare);
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Add to wish list success" });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
