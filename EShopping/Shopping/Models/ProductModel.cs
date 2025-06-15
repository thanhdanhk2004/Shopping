using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Shopping.Reponitory.Validation;
namespace Shopping.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap ten san pham")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap mo ta san pham")]
        public string Description { get; set; }
        public string Slug {  get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        //[Column(TypeName = "decimal(8,2 ")]
        public decimal Price { get; set; }
        public decimal CapitalPrice { get; set; }
        public string Image { get; set; }
        public int BrandId {  get; set; }
        public int CategoryId {  get; set; }
        public BrandModel Brand { get; set; }
        public CategoryModel Category { get; set; }
        public List<RatingModel> Ratings { get; set; }

        [NotMapped]
        [FileExtention]
        public IFormFile? ImageUpload { get; set; }
        [AllowNull]
        public int Quantity {  get; set; }
        [AllowNull]
        public int Sold {  get; set; }
    }
}
