using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;
using System.Security.Claims;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin/User")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly Context _context;
        private UserManager<AppUserModel> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserController(Context context, RoleManager<IdentityRole> roleManager, UserManager<AppUserModel> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var list_user = await (from u in _context.Users
                                   join ur in _context.UserRoles on u.Id equals ur.UserId
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   select new { User = u, RoleName = r.Name }).ToListAsync();
            var logged_in_user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.LoggedInUserId = logged_in_user_id;
            return View(list_user);
        }

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create(UserModel user)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if(ModelState.IsValid)
            {
                IdentityResult result = await _userManager.CreateAsync(user, user.PasswordHash);
                if(result.Succeeded)
                {
                    var user_role = await _userManager.FindByEmailAsync(user.Email);
                    var role_id = await _roleManager.FindByIdAsync(user.RoleId);
                    var add_role_to_user = await _userManager.AddToRoleAsync(user_role, role_id.Name);

                    TempData["success"] = "Create success a user";
                    return RedirectToAction("Index");
                }
                foreach(IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);    
            }
            TempData["error"] = "Have a error in model";
            List<string> errors = new List<string>();
            foreach(var value in ModelState.Values)
            {
                foreach(var error in value.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return BadRequest(errors);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "Delete success a user";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Delete error a user";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string Id, AppUserModel user)
        {
            var user_existed = await _userManager.FindByIdAsync(Id);
            if (ModelState.IsValid)
            {
                user_existed.UserName = user.UserName;
                user_existed.Email = user.Email;
                user_existed.PhoneNumber = user.PhoneNumber;
                user_existed.RoleId = user.RoleId;

                IdentityResult result = await _userManager.UpdateAsync(user_existed);
                if (result.Succeeded)
                {
                    TempData["success"] = "Update cuccess";
                    return RedirectToAction("Index", "User");
                }
                foreach(IdentityError error in result.Errors) 
                    ModelState.AddModelError("", error.Description);
            }
            List<string> errors = new List<string>();
            foreach(var value in ModelState.Values)
            {
                foreach(var error in value.Errors)
                    errors.Add(error.ErrorMessage);
            }
            return BadRequest(errors);
        }
    }

}
