using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Shopping.Models;
using Shopping.Reponitory;
using Shopping.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shopping.Areas.Admin.Reponsitory;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace Shopping.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManager;
        private SignInManager<AppUserModel> _signInManager;
        private Context _context;
        private IEmailSender _emailSender;
        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, Context context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }

        public IActionResult Login(string url)
        {
            return View(new LoginViewModel { ReturnUrl = url});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
                if(result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Invalid username and password");
            }
            return View(loginVM);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();//Logout signin normal
            await HttpContext.SignOutAsync(); // Logout signin by gg 
            return Redirect("/");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if(ModelState.IsValid)
            {
                AppUserModel new_user = new AppUserModel { UserName = user.Username, Email = user.Email };
                IdentityResult result = await _userManager.CreateAsync(new_user, user.Password);
                if (result.Succeeded)
                {
                    return Redirect("/Account/login");
                }
                foreach(IdentityError error in result.Errors) 
                    ModelState.AddModelError("", error.Description);

            }
            return View(user);
        }

        //History order
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var user_id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user_email = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _context.Orders.Where(o => o.Username == user_email).ToListAsync();
            ViewBag.Email = user_email;
         
            return View(orders);
        }

        [HttpGet]
        [Route("Account/Cancel")]
        public async Task<IActionResult> Cancel(string OrderCode)
        {
            var order = await _context.Orders.Where(o => o.OrderCode == OrderCode).FirstOrDefaultAsync();
            if(order != null)
            {
                order.Status = 2;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            TempData["success"] = "Cancel Order Success";
            return RedirectToAction("History", "Account");
        }

        [HttpGet]
        [Route("Account/NewPass")]
        public async Task<IActionResult> NewPass(string email, string token)
        {
            var user = await _userManager.Users.Where(u => u.Email == email && u.Token == token).FirstOrDefaultAsync();
            if(user != null)
            {
                ViewBag.Email = email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email not found or token not right";
                return RedirectToAction("ForgotPass", "Account");
            }
            return View();
        }

    
        [Route("Account/ForgotPass")]
        public async Task<IActionResult> ForgotPass()
        {
            return View();
        }

        [HttpPost]
        [Route("Account/ForgotPass")]
        public async Task<IActionResult> ForgotPass(AppUserModel user)
        {
            var user_email = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if(user_email == null)
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgotPass", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                user_email.Token = token;
                _context.Users.Update(user_email);
                await _context.SaveChangesAsync();
                var receiver = "thanhdanhk2004@gmail.com";
                var subject = "Change password for user" ;
                var message = "Click to link to change password" +
                    "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + user_email.Email +
                    "&token=" + user_email.Token + "'>";
                await _emailSender.SendEmailAsync(receiver, subject, message);
                TempData["success"] = "Please check email";
            }
            return RedirectToAction("ForgotPass", "Account");
        }

        //Update new password
        [HttpPost]
        [Route("Account/UpdateNewPass")]
        public async Task<IActionResult> UpdateNewPass(AppUserModel user)
        {
            var user_check = await _userManager.Users.Where(u => u.Email == user.Email && u.Token == user.Token).FirstOrDefaultAsync();
            if(user == null)
            {
                TempData["errorr"] = "Not found user";
                return RedirectToAction("ForgotPass", "Account");
            }
            string new_token = Guid.NewGuid().ToString();
            string password_hash = new PasswordHasher<AppUserModel>().HashPassword(user_check, user.PasswordHash);
            user_check.Token = new_token;
            user_check.PasswordHash = password_hash;
            await _userManager.UpdateAsync(user_check);
            TempData["success"] = "Change password success";
            return RedirectToAction("Login", "Account");
        }

        //Login gg
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.
                AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if(!result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            var claims = result.Principal.Identities.FirstOrDefault()
                .Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string email_name = email.Split('@')[0];

            var check_user = await _userManager.FindByEmailAsync(email);
            if(check_user == null)
            {
                var password_hasher = new PasswordHasher<AppUserModel>();
                var password_hash = password_hasher.HashPassword(null, "dan@123");
                //Tao user moi
                var new_user = new AppUserModel { UserName = email_name, Email = email };
                new_user.PasswordHash = password_hash;
                var create_user = await _userManager.CreateAsync(new_user);

                if(!create_user.Succeeded)
                {
                    TempData["error"] = "Add account google failure";
                    return RedirectToAction("Login", "Account");
                }
                await _signInManager.SignInAsync(new_user, isPersistent: false);
                TempData["success"] = "Login success";
                return RedirectToAction("Index", "Home");
            }
            await _signInManager.SignInAsync(check_user, isPersistent: false);
            TempData["success"] = "Login success";
            return RedirectToAction("Index", "Home");
            //return Json(claims);
        }
    }
}
