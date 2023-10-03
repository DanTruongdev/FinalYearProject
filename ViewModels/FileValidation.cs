using OnlineShopping.Libraries.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class FileValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var fileHandle = new FileHandleService();
            List<IFormFile> files = new List<IFormFile>();
            try
            {
                var file = (IFormFile)value;
                if (file != null) files.Add(file);
            }
            catch (Exception ex)
            {
                files = value as List<IFormFile>;
            }
           
           
            if (files.Count > 0)
            {
                if (!fileHandle.CheckFileExtension(files)) return new ValidationResult("File extension is not supported");
                if (!fileHandle.CheckFileSize(files)) return new ValidationResult("File size must be less than 60MB");
            }
             
            return ValidationResult.Success;
        }
    }
}
