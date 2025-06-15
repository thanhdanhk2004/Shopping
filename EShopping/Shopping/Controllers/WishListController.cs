using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models.ViewModels;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class WishListController : Controller
    {
        private readonly Context _context;
        public WishListController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var data = await (from w in _context.WishList
                              join p in _context.Products
                              on w.ProductId equals p.Id
                              select new WishListViewModel
                              {
                                  Id = w.Id,
                                  ProductName = p.Name,
                                  ProductDescription = p.Description,
                                  ProductPrice = p.Price,
                                  Image = p.Image,
                              }).ToListAsync();
            return View(data);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var wish = await _context.WishList.FindAsync(id);
            if (wish != null)
            {
                _context.WishList.Remove(wish);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "WishList");
        }
    }
}
