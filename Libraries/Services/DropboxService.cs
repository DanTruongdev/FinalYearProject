using Dropbox.Api.Files;
using Dropbox.Api;
using Castle.Core.Internal;
using OnlineShopping.Libraries.Models;

namespace OnlineShopping.Libraries.Services
{
    public class DropboxService : IDropboxService
    {
        private static List<string> SUPPORTED_IMAGE_EXTENSION = new List<string>() { ".apng", ".bmp", ".gif", ".ico", ".cur", ".jpg", ".jpeg", ".jfif", ".pjpeg",
                                                                    ".pjp", ".png", ".svg", ".webp"};
        private static List<string> SUPPORTED_VIDEO_EXTENSION = new List<string>() { ".mp4", ".webm", ".ogg" };
        private static string AccessToken = "";

        private readonly IConfiguration _configuration;
        public DropboxService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
         
        public async Task<string> UploadAsync(IFormFile file)
        {
            using (var dropbox = new DropboxClient(_configuration["DropBox:AccessToken"]))
            {
                var re = CheckFileExtension(file);
                if (file != null && file.Length > 0 && CheckFileExtension(file))
                {
                    try
                    {
                        string folderName = ImageOrVideo(file);
                        if (folderName.IsNullOrEmpty()) throw new Exception();
                        var fileName = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName).ToLower();
                        using (var fileStream = file.OpenReadStream())
                        {

                            // Upload file
                            var uploadedFile = await UploadFileAsync(dropbox, fileStream, folderName, fileName);
                            return uploadedFile.PathDisplay;
                        }
                    }
                    catch
                    {
                        return "";
                    }

                }
                return "";
              
            }
        }
        private async Task<FileMetadata> UploadFileAsync(DropboxClient dropbox, Stream fileStream, string folderName, string fileName)
        {

            var fileMetaData = await dropbox.Files.UploadAsync(
                "/" + folderName + "/" + fileName,
                WriteMode.Overwrite.Instance,
                body: fileStream);

            return fileMetaData;
        }
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            using (var dropbox = new DropboxClient(_configuration["DropBox:AccessToken"]))
            {
                try
                {
                    await dropbox.Files.DeleteV2Async(filePath);
                    return true;
            }
                catch
                {
                return false;
            }

        }
        }
       

        public  async Task<string> GetDownloadLinkAsync(string filePath)
        {
            using (var dropbox = new DropboxClient(_configuration["DropBox:AccessToken"]))
            {
                try
                {
                    var sharedLink = await dropbox.Sharing.CreateSharedLinkWithSettingsAsync(filePath);                 
                    var downloadLink = sharedLink.Url.Replace("www.dropbox.com", "dl.dropboxusercontent.com");
                    return downloadLink;
                }
                catch
                {
                    try
                    {
                        var sharedLinks = await dropbox.Sharing.ListSharedLinksAsync(filePath);
                        if (sharedLinks.Links.Any())
                        {
                            var shareLink = sharedLinks.Links.First().Url;
                            var downloadLink = shareLink.Replace("www.dropbox.com", "dl.dropboxusercontent.com");
                            return downloadLink;
                        }
                    }
                    catch
                    {
                        return "";
                    }
                    return "";
                }

            }          
        }

        public bool CheckFileExtension(List<IFormFile> files)
        {
            foreach (IFormFile file in files)
            {
               
                if (ImageOrVideo(file).IsNullOrEmpty()) return false;
            }
            return true;
        }
        public bool CheckFileExtension(IFormFile file)
        {
            return CheckFileExtension(new List<IFormFile>() { file });
        }
        public string ImageOrVideo(IFormFile file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (SUPPORTED_IMAGE_EXTENSION.Contains(fileExt)) return "images";
            else if (SUPPORTED_VIDEO_EXTENSION.Contains(fileExt)) return "videos";
            return "";
        }
    }
}
