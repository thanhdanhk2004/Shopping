using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Publisher, Author")]
    public class BrandController : Controller
    {
        private readonly Context _context;
        public BrandController(Context context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            return View(_context.Brands.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                var slug = _context.Brands.Where(c => c.Slug == brand.Slug).FirstOrDefault();
                if (slug != null)
                {
                    ModelState.AddModelError("", "Tồn tại slug rồi");
                    return View(brand);
                }
                _context.Brands.Add(brand);
                _context.SaveChanges();
                TempData["success"] = "Add Success a Category";
                return RedirectToAction("index");
            }
            TempData["error"] = "Have a modle error";
            List<string> errors = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            string error_message = string.Join("\n", errors);
            return BadRequest(error_message);
        }


        public async Task<IActionResult> Delete(int id)
        {
            BrandModel brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            TempData["success"] = "Delete Success a category";
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            return View(_context.Brands.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            var exist_brand = _context.Brands.Find(brand.Id);
            if (ModelState.IsValid)
            {
                brand.Slug = brand.Name.Replace(" ", "-");
                exist_brand.Name = brand.Name;
                exist_brand.Description = brand.Description;
                exist_brand.Status = brand.Status;
                _context.Brands.Update(exist_brand);
                _context.SaveChanges();
                TempData["success"] = "Edit succes";
                return RedirectToAction("index");
            }

            List<string> errors = new List<string>();
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            string error_message = string.Join("\n", errors);
            return BadRequest(error_message);
        }
    }
}
