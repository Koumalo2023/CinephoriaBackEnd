namespace CinephoriaServer.API.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string imageUrl);
    }

    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Le fichier est vide ou non valide.");
            if (string.IsNullOrEmpty(_environment.WebRootPath))
                throw new InvalidOperationException("Le chemin racine pour les fichiers statiques (WebRootPath) n'est pas configuré.");

            // Chemin pour enregistrer le fichier
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Construit une URL absolue en utilisant l'URL de base de l'application
            string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            string imageUrl = Path.Combine(baseUrl, "images", folder, uniqueFileName).Replace("\\", "/");

            return imageUrl;
        }

        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                throw new ArgumentException("L'URL de l'image est invalide.");

            var filePath = Path.Combine(_environment.WebRootPath, imageUrl);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
