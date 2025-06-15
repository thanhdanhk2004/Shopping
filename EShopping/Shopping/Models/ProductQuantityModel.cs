using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int Id { get; set; } 
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Require input quantity product")]
        public int Quantity {  get; set; }
        public DateTime DateCreate { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductModel Product { get; set; } 
    }
}
