using System.ComponentModel.DataAnnotations;

namespace Shopping.Reponitory.Validation
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extention = Path.GetExtension(file.FileName);//Vd: 1.jpg
                string[] extentions = ["jpg", "png", "jpeg"];
                bool result = extentions.Any(x => extention.EndsWith(x));
                if (!result)
                    return new ValidationResult("Allowed extentions are jpg, png or jpeg");
            }
            return ValidationResult.Success;
        }
    }
}
