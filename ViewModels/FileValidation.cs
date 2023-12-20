using OnlineShopping.Libraries.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class FileValidation : ValidationAttribute
    {
        private static List<string> SUPPORTED_EXTENSION = new List<string>() { ".apng", ".bmp", ".gif", ".ico", ".cur", ".jpg", ".jpeg", ".jfif", ".pjpeg",
                                                                    ".pjp", ".png", ".svg", ".webp", ".mp4", ".webm", ".ogg" };
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
                  
            List<IFormFile> files = new List<IFormFile>();
            try
            {
                var file = (IFormFile)value;
                if (file != null) files.Add(file);
            }
            catch 
            {
                files = value as List<IFormFile>;
            }
           
           
            if (files.Count > 0)
            {
                foreach(var file in files)
                {
                    if (!SUPPORTED_EXTENSION.Contains(Path.GetExtension(file.FileName).ToLower())) return new ValidationResult("File extension is not supported");
                    if (file.Length > 60 * 1024 * 1024) return new ValidationResult("File size must be less than 60MB");
                }              
            }
             
            return ValidationResult.Success;
        }
    }
}
