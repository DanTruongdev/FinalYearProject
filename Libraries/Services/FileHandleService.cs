using Castle.Core.Internal;

namespace OnlineShopping.Libraries.Services
{
    public class FileHandleService
    {
        private static string FOLDER_ROOT = "Assets";
        private static List<string> SUPPORTED_IMAGE_EXTENSION = new List<string>() { ".apng", ".bmp", ".gif", ".ico", ".cur", ".jpg", ".jpeg", ".jfif", ".pjpeg",
                                                                    ".pjp", ".png", ".svg", ".webp"}; 
        private static List<string> SUPPORTED_VIDEO_EXTENSION = new List<string>() { ".mp4", ".webm", ".ogg"};
        public void DeleteFile(string path)
        {
            System.IO.File.Delete(path);
        }
        public bool CheckFileExtension(List<IFormFile> files)
        {
            foreach(IFormFile file in files)
            {
                if (ImageOrVideo(file).IsNullOrEmpty()) return false;
            }
            return true;
        }

        public bool CheckFileExtension(IFormFile file)
        {
            return CheckFileExtension(new List<IFormFile>() { file });
        }

        public bool CheckFileSize(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > 62914560) return false; // file size > 5mb
            }          
            return true;
        }

        public bool CheckFileSize(IFormFile file)
        {
            return CheckFileSize(new List<IFormFile>() { file });
        }

        public string UploadFile(string model, IFormFile file)
        {
            string fileNameWithPath = "";
            try
            {
                
                    string imageOrVideo = ImageOrVideo(file);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), $"{FOLDER_ROOT}\\{imageOrVideo}\\{model}");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    fileNameWithPath = Path.Combine(path, file.FileName);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
            }
            catch (Exception ex)
            {
                return "Error";
            }
            return fileNameWithPath;
        }

        public string ImageOrVideo(IFormFile file)
        {
            string fileExt = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (SUPPORTED_IMAGE_EXTENSION.Contains(fileExt)) return "Images";
            else if (SUPPORTED_VIDEO_EXTENSION.Contains(fileExt)) return "Videos";
            return null;         
        }


    }
}
