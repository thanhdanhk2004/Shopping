using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Controllers
{
    public class ProductController : Controller
    {
        private readonly Context _context;
        public ProductController(Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {
                RatingModel rating_model = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Stars = rating.Stars,
                };
                _context.Ratings.Add(rating_model);
                await _context.SaveChangesAsync();
                TempData["success"] = "Add comment success";
                return Redirect(Request.Headers["Referer"]);
            }
            return RedirectToAction("Detail", "Home", new {id = rating.ProductId});
        }
        
    }
}
