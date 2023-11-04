using OnlineShopping.Libraries.Models;

namespace OnlineShopping.Libraries.Services
{
    public interface IFirebaseService
    {
        public string UploadFile(IFormFile file);
        public bool RemoveFile(string objectName);
        public string ImageOrVideo(IFormFile file);
        public string GetDownloadUrl(string objectName);
    }

  
}
