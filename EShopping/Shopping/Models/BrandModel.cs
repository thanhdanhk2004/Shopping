using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap ten thuong hieu")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap mo ta thuong hieu")]
        public string Description { get; set; }
        public string Slug {  get; set; }
        public int Status {  get; set; }
    }
}
