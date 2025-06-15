using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models.ViewModels;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class CompareController : Controller
    {
        private readonly Context _context;
        public CompareController(Context context) {  _context = context; }
        public async Task<IActionResult> Index()
        {
            var data = await (from c in _context.Compares
                              join p in _context.Products
                              on c.ProductId equals p.Id
                              select new CompareViewModel
                              {
                                  Id = p.Id,
                                  ProductName = p.Name,
                                  ProductDescription = p.Description,
                                  ProductPrice = p.Price,
                                  Image = p.Image,
                              }).ToListAsync();
                             
            return View(data);
        }

        [HttpGet]
        [Route("Comapre/Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var wish = await _context.Compares.Where(c => c.ProductId == id).FirstOrDefaultAsync();
            if (wish != null)
            {
                _context.Compares.Remove(wish);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Compare");
        }
    }
}
