namespace OnlineShopping.Libraries.Services
{
    public interface ICloudinaryService
    {
        public string UploadFile(IFormFile file);
        public string GetSharedUrl(string publicId);
        public bool RemoveFile(string publicId);
        public string ImageOrVideo(IFormFile file);
    }
}
