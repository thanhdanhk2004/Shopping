using Microsoft.EntityFrameworkCore;
using Shopping.Models;

namespace Shopping.Reponitory
{
    public class SeedData
    {
        public static void SeedingData(Context _context)
        {
            _context.Database.Migrate();
            if(!_context.Products.Any())
            {
                CategoryModel macbook = new CategoryModel {
                    Name = "Macbook",
                    Slug = "macbook",
                    Description = "Macbook is Large Brand in the world",
                    Status = 1
                };
                CategoryModel pc = new CategoryModel
                {
                    Name = "Pc",
                    Slug = "pc",
                    Description = "pc is Large Brand in the world",
                    Status = 1
                };

                BrandModel apple = new BrandModel
                {
                    Name = "Apple",
                    Slug = "apple",
                    Description = "Apple is Large Brand in the world",
                    Status = 1
                };
                BrandModel samsung = new BrandModel
                {
                    Name = "Samsung",
                    Slug = "samsung",
                    Description = "Samsung is Large Brand in the world",
                    Status = 1
                };

                _context.Products.AddRange(
                    new ProductModel { Name = "Macbook", Slug = "macbook", Description = "Microsoft is best", Image = "1.jpg", Category = macbook, Price = 1223, Brand=apple },
                    new ProductModel { Name = "Pc", Slug = "pc", Description = "Pc is best", Image = "1.jpg", Category = pc, Price = 1223, Brand = samsung }
                );
                _context.SaveChanges();
            }
        }
    }
}
