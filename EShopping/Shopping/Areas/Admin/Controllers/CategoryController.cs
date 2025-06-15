using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Publisher, Author, Admin")]

    public class CategoryController : Controller
    {
        private readonly Context _context;
        public CategoryController(Context context)
        {
            _context = context;
        }

        [Route("Index")]
        public async Task<IActionResult> Index(int page = 1)
        {
            List<CategoryModel> categories = await _context.Categories.ToListAsync();
            if(page < 1)
                page = 1;
            var pager = new Paginate(categories.Count, page);
            int rec_skip = (page - 1) * 5;
            var data = categories.Skip(rec_skip).Take(5).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if(ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                var slug = _context.Categories.Where(c => c.Slug == category.Slug).FirstOrDefault();
                if(slug != null)
                {
                    ModelState.AddModelError("","Tồn tại slug rồi");
                    return View(category);
                }
                _context.Categories.Add(category);
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
            CategoryModel category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["success"] = "Delete Success a category";
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            return View(_context.Categories.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            var exist_category = _context.Categories.Find(category.Id);
            if(ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-");
                exist_category.Name = category.Name;
                exist_category.Description = category.Description;
                exist_category.Status = category.Status;
                _context.Categories.Update(exist_category);
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
