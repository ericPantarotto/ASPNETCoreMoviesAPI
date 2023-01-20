using MoviesAPI.Validations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The field with FieldName '{0}' is required")]
        [StringLength(10)]
        //[FirstLetterUppercase]
        public string Name { get; set; }

        //IValidatableObject is hard-coded at the model level while ValidationAttribute can be re-used
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                string firstLetter = Name[0].ToString();
                if (firstLetter != firstLetter.ToUpper())
                {
                    yield return new ValidationResult("First letter should be upper case", new string[] { nameof(Name) });
                }

                //if (Name.Length > 5)
                //{
                //    yield return new ValidationResult("why is it greater than 5", new string[] { nameof(Name) });
                //}
            }
            //if (Id > 0)
            //{
            //    yield return new ValidationResult("Id shouldn't be provided!", new string[] { nameof(Name) });
            //}

        }
    }    
}