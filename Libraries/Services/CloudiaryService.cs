using Castle.Core.Internal;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace OnlineShopping.Libraries.Services
{
    public class CloudiaryService:ICloudinaryService
    {
        private readonly List<string> SUPPORTED_IMAGE_EXTENSION = new List<string>() { ".apng", ".bmp", ".gif", ".ico", ".cur", ".jpg", ".jpeg", ".jfif", ".pjpeg",
                                                                    ".pjp", ".png", ".svg", ".webp"};
        private readonly List<string> SUPPORTED_VIDEO_EXTENSION = new List<string>() { ".mp4", ".webm", ".ogg" };
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;
        public CloudiaryService(IConfiguration config)
        {
            _config = config;
            _cloudinary = new Cloudinary(new Account(
            _config["Cloudinary:CloudName"],
            _config["Cloudinary:ApiKey"],
            _config["Cloudinary:ApiSecret"]
            ));
        }

        public string UploadFile(IFormFile file)
        {
            if (file == null || file.Length <= 0 || !CheckFileExtension(file)) return "";

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = ImageOrVideo(file)
                };
                var uploadResult = _cloudinary.Upload(uploadParams);
                var publicId = uploadResult.PublicId;
                return publicId;
            }
        }

        public string GetSharedUrl(string publicId)
        {
            try
            {
                string fileUrl = _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
                return fileUrl;
            }
            catch
            {
                return "";
            }
        }

        public bool RemoveFile(string publicId)
        {
            var destroyParams = new DeletionParams(publicId);
            var destroyResult = _cloudinary.Destroy(destroyParams);
            if (destroyResult.Result == "ok")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ImageOrVideo(IFormFile file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (SUPPORTED_IMAGE_EXTENSION.Contains(fileExt)) return "images";
            else if (SUPPORTED_VIDEO_EXTENSION.Contains(fileExt)) return "videos";
            return "";
        }

        private bool CheckFileExtension(List<IFormFile> files)
        {
            foreach (IFormFile file in files)
            {

                if (ImageOrVideo(file).IsNullOrEmpty()) return false;
            }
            return true;
        }
        private bool CheckFileExtension(IFormFile file)
        {
            return CheckFileExtension(new List<IFormFile>() { file });
        }
       
    }
}
