using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Reponitory;
using Microsoft.AspNetCore.Identity;
using Shopping.Models;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly Context _context;
        private RoleManager<IdentityRole> _roleManager;
        public RoleController(Context context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }


        //Create Role
        [Route("Create")]
        public IActionResult Create()
        {
            
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if(!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
                TempData["success"] = "Add role success";
                return RedirectToAction("Index", "Role");
            }

            List<string> errors = new List<string>();
            foreach(var value in ModelState.Values)
            {
                foreach (var errer in value.Errors)
                    errors.Add(errer.ErrorMessage);
            }
            return BadRequest(errors);
        }

        //Edit
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id,IdentityRole role)
        {
            var role_existed = await _roleManager.FindByIdAsync(Id);
            if (role_existed == null) 
                return NotFound();
            role_existed.Name = role.Name;
            _roleManager.UpdateAsync(role_existed).GetAwaiter().GetResult();
            TempData["success"] = "update success";
            return RedirectToAction("Index", "Role");
        }


        //Xoa role
        [Route("Delete")]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
                return NotFound();
            var role = await _roleManager.FindByIdAsync(id);
            if(role == null)
                return NotFound();
            IdentityResult result = await _roleManager.DeleteAsync(role);
            TempData["success"] = "Delete cuccess role";
            return RedirectToAction("Index", "Role");
        }
    }
}
