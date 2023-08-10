namespace OnlineShopping.Libraries.Services
{
    public interface IHandleFileService
    {
        public string UploadFile(List<IFormFile> files, int EntityId, string AttachmentOwer, string type);
        public string DownloadFile(int id);
    }
}
