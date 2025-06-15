using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.OrderBy(p => p.Id).Include(p => p.Category).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid) // Kiem tra xem Model co ok het chua
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _context.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "San pham da ton tai");
                    return View(product);
                }
                else
                {
                    if(product.ImageUpload != null)
                    {
                        string uploads_dir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                        string image_name = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string file_path = Path.Combine(uploads_dir, image_name);

                        FileStream fs = new FileStream(file_path, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        product.Image = image_name;

                    }
                }
                _context.Add(product);
                _context.SaveChanges();
                TempData["success"] = "Them thanh cong";
                return RedirectToAction("index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
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
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ProductModel product = await _context.Products.FindAsync(id);
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_context.Brands, "Id", "Name", product.BrandId);

            var existed_product = _context.Products.Find(product.Id);

            if (ModelState.IsValid) // Kiem tra xem Model co ok het chua
            {
                product.Slug = product.Name.Replace(" ", "-");
                if (product.ImageUpload != null)
                {
                    string uploads_dir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string image_name = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string file_path = Path.Combine(uploads_dir, image_name);

                    if(existed_product.Image != null)
                    {
                        //Xoa hinh anh cu
                        string old_file_path = Path.Combine(uploads_dir, existed_product.Image);
                        try
                        {
                            if (System.IO.File.Exists(old_file_path))
                                System.IO.File.Delete(old_file_path);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An error occurred while deleting the product image");
                        }
                    }
                    FileStream fs = new FileStream(file_path, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = image_name;
                }
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CapitalPrice = product.CapitalPrice;
                existed_product.CategoryId = product.CategoryId;
                existed_product.BrandId = product.BrandId;

                _context.Update(existed_product);
                _context.SaveChanges();
                TempData["success"] = "Cap nhat san pham thanh cong";
                return RedirectToAction("index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lỗi";
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
            return View(product);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            ProductModel product = await _context.Products.FindAsync(id);
            string uploads_dir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            if(product.Image != null)
            {
                string old_file_path = Path.Combine(uploads_dir, product.Image);
                if (System.IO.File.Exists(old_file_path))
                {
                    System.IO.File.Delete(old_file_path);
                }
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            TempData["success"] = "Xoa san pham thanh cong";
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("AddQuantity")]
        public async Task<IActionResult> AddQuantity(int id)
        {
            var product_quantyties = await _context.ProductQuantity.Where(q => q.ProductId == id).ToListAsync(); 
            ViewBag.Id = id;
            ViewBag.ProductByQuantity = product_quantyties;
            return View();
        }

        [HttpPost]
        [Route("AddQuantity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuantity(int id,ProductQuantityModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
                return NotFound();
            product.Quantity += model.Quantity;
            ProductQuantityModel productQuantityModel = new ProductQuantityModel
            {
                ProductId = product.Id,
                Quantity = model.Quantity,
                DateCreate = DateTime.Now,
            };
            _context.ProductQuantity.Add(productQuantityModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Product");
        }
    }

}
