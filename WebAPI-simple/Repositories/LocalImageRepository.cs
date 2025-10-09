using WebAPI_simple.Data;
using WebAPI_simple.Models.Domain;

namespace WebAPI_simple.Repositories
{
    public class LocalImageRepository: IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _dbContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }//constructor

        public Image Upload(Image image)
        {
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtension}");
            //upload Image to local path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            image.File.CopyTo(stream);

            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";
            image.FilePath = urlFilePath;

            //add image to the Images table
            _dbContext.Images.Add(image);
            _dbContext.SaveChanges();

            return image;
        }

        public List<Image> GetAllInfoImages()
        {
            var allImages = _dbContext.Images.ToList();
            return allImages;
        }

        public (byte[], string, string) DowndloadFile(int Id)
        {
            try
            {
                var FileById = _dbContext.Images.Where(x => x.Id == Id).FirstOrDefault();
                var path = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", $"{FileById.FileName}{FileById.FileExtension}");
                var stream = File.ReadAllBytes(path);
                var filename = FileById.FileName + FileById.FileExtension;
                return (stream,"application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
