using Microsoft.AspNetCore.Mvc;
using Shopping.Reponitory;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Context _context;
        public CategoryController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> index(string slug = "", string sort_by = "", string startprice="", string endprice="")
        {
            int id = _context.Categories.Where(c => c.Slug == slug).Select(c => c.Id).FirstOrDefault();
            if(id == 0)
                return RedirectToAction("Index");
            IQueryable<ProductModel> products = _context.Products.Where(p => p.CategoryId == id);
            int count = products.Count();
            if(count > 0)
            {
                if (sort_by == "price_increases")
                    products = products.OrderBy(p => p.Price);
                else if(sort_by == "price_decreases")
                    products = products.OrderByDescending(p => p.Price);
                else if (sort_by == "price_oldest")
                    products = products.OrderBy(p => p.Id);
                else if (startprice != "" && endprice != "")
                    products = products.Where(p => p.Price >= decimal.Parse(startprice) && p.Price <= decimal.Parse(endprice));
                else
                    products = products.OrderByDescending(p => p.Id);
            }
            return View(await products.ToListAsync());
        }
    }
}
