using Bogus.Bson;
using Castle.Core.Internal;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineShopping.Libraries.Models;
using OnlineShopping.ViewModels.User;
using System.Security.AccessControl;

namespace OnlineShopping.Libraries.Services
{
    public class FirebaseService : IFirebaseService
    {
        private static List<string> SUPPORTED_IMAGE_EXTENSION = new List<string>() { ".apng", ".bmp", ".gif", ".ico", ".cur", ".jpg", ".jpeg", ".jfif", ".pjpeg",
                                                                    ".pjp", ".png", ".svg", ".webp"};
        private static List<string> SUPPORTED_VIDEO_EXTENSION = new List<string>() { ".mp4", ".webm", ".ogg" };

        private readonly FirebaseConfiguration _firebaseConfiguration;
        GoogleCredential googleCredential;
        StorageClient storage;

        public FirebaseService(FirebaseConfiguration firebaseConfiguration)
        {
            _firebaseConfiguration = firebaseConfiguration;
            var credential = new
            {
                type = _firebaseConfiguration.Type,
                project_id = _firebaseConfiguration.ProjectId,
                private_key_id = _firebaseConfiguration.PrivateKeyId,
                client_email = _firebaseConfiguration.ClientEmail,
                private_key = _firebaseConfiguration.PrivateKey,
                client_id = _firebaseConfiguration.ClientId,
                auth_uri = _firebaseConfiguration.AuthUri,
                token_uri = _firebaseConfiguration.TokenUri,
                auth_provider_x509_cert_url = _firebaseConfiguration.AuthProviderX509CertUrl,
                client_x509_cert_url = _firebaseConfiguration.ClientX509CertUrl,
                universe_domain = _firebaseConfiguration.UniverseDomain
            };
            string json = JsonConvert.SerializeObject(credential);
            googleCredential = GoogleCredential.FromJson(json);
            storage = StorageClient.Create(googleCredential);

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

        public string UploadFile(IFormFile file)
        {          
           
            if (file != null && file.Length > 0 && CheckFileExtension(file))
              {
                try
                {
                    var bucketName = _firebaseConfiguration.BucketName;
                    var fileExtension = Path.GetExtension(file.FileName);
                    var objectName = ImageOrVideo(file) + "/" + Guid.NewGuid().ToString() + fileExtension;
                    using (var fileStream = file.OpenReadStream())
                    {
                        storage.UploadObject(bucketName, objectName, null, fileStream);
                    }                   
                    return objectName;
                }
                catch
                {
                    return "";
                }
                 
              }             
            return "";
        }
      
        public bool RemoveFile(string objectName)
        {
            if (objectName.IsNullOrEmpty()) return true;
            try
            {
                storage.DeleteObject(_firebaseConfiguration.BucketName, objectName);
                return true;
            } catch { return false; }        
        }
        public string GetDownloadUrl(string objectName)
        {
            if (!objectName.IsNullOrEmpty())
            {
                var serviceAccountCredential = googleCredential.CreateScoped("https://www.googleapis.com/auth/devstorage.full_control").UnderlyingCredential as ServiceAccountCredential;
                var urlSigner = UrlSigner.FromServiceAccountCredential(serviceAccountCredential);
                string downloadUrl = urlSigner.Sign(_firebaseConfiguration.BucketName, objectName, TimeSpan.FromDays(3), HttpMethod.Get);
                return downloadUrl;
            }
            return "";
            
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
