namespace GlassesShop.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxFileSize = 5 * 1024 * 1024;

        public static bool IsValidImage(IFormFile file, out string error)
        {
            error = string.Empty;

            if (file == null || file.Length == 0)
            {
                error = "File rỗng.";
                return false;
            }

            if (file.Length > MaxFileSize)
            {
                error = $"File \"{file.FileName}\" vượt quá 5MB.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
            {
                error = $"File \"{file.FileName}\" không đúng định dạng ảnh (jpg, png, webp, gif).";
                return false;
            }

            return true;
        }

        public static async Task<string> SaveImageAsync(IFormFile file, string webRootPath)
        {
            var folder = Path.Combine(webRootPath, "images", "products");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/products/{fileName}";
        }

        public static void DeleteImage(string imageUrl, string webRootPath)
        {
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith("/images/products/"))
                return;

            var fullPath = Path.Combine(webRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fullPath))
            {
                try { File.Delete(fullPath); } catch { }
            }
        }
    }
}