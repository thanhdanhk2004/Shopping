using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please input username")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Please input password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please input email"), EmailAddress]
        public string Email { get; set; }
    }
}
