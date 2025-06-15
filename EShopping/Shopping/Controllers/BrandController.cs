using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class BrandController : Controller
    {
        private readonly Context _context;
        public BrandController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string slug = "")
        {
            int id = _context.Brands.Where(b => b.Slug == slug).Select(b => b.Id).FirstOrDefault();
            if(id == 0) 
                return RedirectToAction("Index");
            var products = _context.Products.Where(p => p.BrandId == id);

            return View(await products.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
