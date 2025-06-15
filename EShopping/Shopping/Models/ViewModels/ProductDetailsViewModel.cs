using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel Product { get; set; }
        [Required(ErrorMessage = "Require input Comment")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Require input Comment")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Require input Comment"), EmailAddress]
        public string Email { get; set; }
    }
}
