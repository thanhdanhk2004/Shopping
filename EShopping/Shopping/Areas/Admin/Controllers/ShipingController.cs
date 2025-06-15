using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Reponitory;

namespace Shopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Authorize]
    public class ShipingController : Controller
    {
        private readonly Context _context;
        public ShipingController(Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var list_shipping = await _context.Shipings.ToListAsync();
            ViewBag.Shipings = list_shipping;
            return View();
        }

        [HttpPost]
        [Route("AddShipping")]
       
        public async Task<IActionResult> AddShipping(string city, string district, string ward, decimal price)
        {
            ShipingModel model = new ShipingModel
            {
                City = city,
                District = district,
                Ward = ward,
                Price = price
            };
            try
            {
                var shipping = await _context.Shipings.Where(s => s.City ==  city && s.District == district && s.Ward == ward).FirstOrDefaultAsync();
                if (shipping != null)
                    return Ok(new { duplicate = true, message = "Data duplicate" });
                _context.Shipings.Add(model);
                await _context.SaveChangesAsync();
                return Ok(new {success = true, message = "Add shipping success"});
            }catch (Exception ex)
            {
                return StatusCode(500, "An error occured");
            }
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var shipping = await _context.Shipings.FindAsync(id);
            _context.Shipings.Remove(shipping);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Shiping");
        }
    }
}
