using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class FirstLetterUppercaseAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            string firstLetter = value.ToString()[0].ToString();
            if (firstLetter  != firstLetter.ToUpper())
            {
                return new ValidationResult("First letter should be upper case");
            }
            return ValidationResult.Success;

        }
    }
}
