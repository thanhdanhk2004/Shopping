using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Reponitory.Components
{
    public class CategoriesViewComponent : ViewComponent 
    {
        private readonly Context _context;
        public CategoriesViewComponent(Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(_context.Categories.ToList());
       
    }
}
