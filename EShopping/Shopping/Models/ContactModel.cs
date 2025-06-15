using Shopping.Reponitory.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Require input name")]
        public string Name {  get; set; }
        [Required(ErrorMessage = "Require input Map")]
        public string Map {  get; set; }
        [Required(ErrorMessage = "Require input Email")]
        public string Email {  get; set; }
        [Required(ErrorMessage = "Require input Phone")]
        public string Phone {  get; set; }
        public string Description {  get; set; }
        public string LogoImage {  get; set; }

        [NotMapped]
        [FileExtention]
        public IFormFile? ImageUpload { get; set; }
    }
}
