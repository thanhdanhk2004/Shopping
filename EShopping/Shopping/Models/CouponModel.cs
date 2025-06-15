using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Please input name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please input Description")]
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }
        public int Quantity {  get; set; }
        public int Status {  get; set; }
    }
}
