using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Reponitory.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly Context _context;
        public FooterViewComponent(Context context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _context.Contacts.FirstOrDefaultAsync());
        }
    }
}
