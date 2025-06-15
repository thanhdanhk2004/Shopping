using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Reponitory.Components
{
    public class BrandsViewComponent:ViewComponent
    {
        private readonly Context _context;
        public BrandsViewComponent(Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _context.Brands.ToListAsync());
        }
    }
}
