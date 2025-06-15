using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Require input Comment")]
        public string Comment {  get; set; }
        [Required(ErrorMessage = "Require input Comment")]
        public string Name {  get; set; }
        [Required(ErrorMessage = "Require input Comment"), EmailAddress]
        public string Email {  get; set; }
        public string Stars {  get; set; }
        [Required]
        public int ProductId {  get; set; }
        public ProductModel Product { get; set; }

    }
}
