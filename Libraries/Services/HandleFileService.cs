using OnlineShopping.Data;
using OnlineShopping.Models.Gallary;



namespace OnlineShopping.Libraries.Services
{
    public class HandleFileService: IHandleFileService 
    {
        private static readonly IList<string> ImageExtension = new List<string>() { ".jpeg", ".jpg", ".png", ".gif", ".bmp", "tiff", "tif" };
        private static readonly IList<string> VideoExtension = new List<string>() { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };     
        private readonly ApplicationDbContext _dbContext;
        public HandleFileService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public string DownloadFile(int id)
        {

            return "Ok";
        }

        public string UploadFile(List<IFormFile> files, int EntityId, string AttachmentOwer, string type)
        {
            var attachments = new List<Attachment>();
            foreach (var file in files)
            {
                string path = GeneratePath(file);
                if (path == null) return "The file extension is not supported";
                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileNameWithPath = Path.Combine(path, file.FileName);
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);

                }
                var attachement = new Attachment()
                {
                    FileName = file.FileName,                   
                    Path = path,
                    AttachmentFor = AttachmentOwer,
                    LikedItemId = EntityId
                };
                if (type.Equals("ADD"))
                {
                    _dbContext.Add(attachement);
                }
                else if (type.Equals("UPDATE"))
                {
                    _dbContext.Update(attachement);
                }
                attachments.Add(attachement);
                _dbContext.SaveChanges();
            }
            return "Upload file sucessfully";

        }

        private string GeneratePath(IFormFile file)
        {
            FileInfo fi = new FileInfo(file.FileName);
            string ext = fi.Extension;
            if (ImageExtension.Contains(ext)) return Path.Combine(Directory.GetCurrentDirectory(), "Assets/Images");
            else if (VideoExtension.Contains(ext)) return Path.Combine(Directory.GetCurrentDirectory(), "Assets/Videos");
            else return null;
        }


    }

}
