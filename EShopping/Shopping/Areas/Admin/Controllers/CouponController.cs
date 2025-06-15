using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly Context _context;
        public CouponController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons.ToArrayAsync();
            ViewBag.Coupons = coupons;
            return View();
        }

        [HttpPost]
        [Route("Coupon/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponModel model)
        {
            if(ModelState.IsValid)
            {
                CouponModel coupon = new CouponModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    DateStart = model.DateStart,
                    DateExpired = model.DateExpired,
                    Status = model.Status,
                };
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Add coupon success";
                return RedirectToAction("Index", "Coupon");
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
    }
}
