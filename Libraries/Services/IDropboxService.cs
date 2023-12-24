using Dropbox.Api;

namespace OnlineShopping.Libraries.Services
{
    public interface IDropboxService
    {
  
        public Task<string> UploadAsync(IFormFile file);
        public Task<string> GetDownloadLinkAsync(string filePath);
        public Task<bool> DeleteFileAsync(string filePath);
        public string ImageOrVideo(IFormFile file);
    }
}
