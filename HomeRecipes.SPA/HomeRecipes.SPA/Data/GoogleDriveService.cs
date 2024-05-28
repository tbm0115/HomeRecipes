using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Text;

namespace HomeRecipes.SPA.Data
{
    public class GoogleDriveService
    {

        public async Task<DriveService> GetDriveServiceAsync(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "BlazorApp",
            });
        }

        public async Task<Google.Apis.Drive.v3.Data.File> GetOrCreateFileAsync(DriveService driveService, string fileName)
        {
            var request = driveService.Files.List();
            request.Q = $"name='{fileName}'";
            var result = await request.ExecuteAsync();
            var file = result.Files.FirstOrDefault();
            if (file == null)
            {
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = fileName,
                    MimeType = "application/json"
                };
                var createRequest = driveService.Files.Create(fileMetadata);
                file = await createRequest.ExecuteAsync();
            }
            return file;
        }

        public async Task<string> ReadFileContentAsync(DriveService driveService, string fileId)
        {
            var request = driveService.Files.Get(fileId);
            var stream = new MemoryStream();
            await request.DownloadAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public async Task UpdateFileContentAsync(DriveService driveService, string fileId, string content)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var updateRequest = driveService.Files.Update(fileMetadata, fileId, stream, "application/json");
            await updateRequest.UploadAsync();
        }
    }
}
