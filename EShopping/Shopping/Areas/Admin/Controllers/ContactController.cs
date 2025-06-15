using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/contact")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ContactController(Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts); 
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            return View(contact);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( ContactModel contact)
        {
            var existed_contact = await _context.Contacts.FindAsync(contact.Id);

            if (ModelState.IsValid) // Kiem tra xem Model co ok het chua
            {
                if (contact.ImageUpload != null)
                {
                    string uploads_dir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    string image_name = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;
                    string file_path = Path.Combine(uploads_dir, image_name);

                    if (existed_contact.LogoImage != null)
                    {
                        //Xoa hinh anh cu
                        string old_file_path = Path.Combine(uploads_dir, existed_contact.LogoImage);
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
                    await contact.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_contact.LogoImage = image_name;
                }
                existed_contact.Name = contact.Name;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;
                existed_contact.LogoImage = contact.LogoImage;
                existed_contact.Email = contact.Email;

                _context.Update(existed_contact);
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
            return View(contact);
        }
    }
}
