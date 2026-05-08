
using BusinessLogic.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

public class PhotoService : IPhotoService
{
    private readonly ApplicationDbContext _context;

    //Lambda
    public PhotoService(ApplicationDbContext context) => _context = context;

    public async Task<string> SaveFileAsync(IFormFile file, string format, bool resize)
    {
        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        var fileName = Guid.NewGuid().ToString() + "." + format.ToLower();
        var filePath = Path.Combine(uploadDir, fileName);

        using var image = Image.Load(file.OpenReadStream());

        if (resize)
        {
            image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(800, 800), Mode = ResizeMode.Max }));
        }

        switch (format.ToLower())
        {
            case "png": await image.SaveAsPngAsync(filePath); break;
            case "bmp": await image.SaveAsBmpAsync(filePath); break;
            default: await image.SaveAsJpegAsync(filePath); break;
        }

        return "/uploads/" + fileName;
    }

    public void DeleteFile(string relativePath)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    public async Task<(List<Photo> Photos, int TotalPages)> GetPagedPhotos(
    int page, int pageSize, string? hashtag, string? author, DateTime? start, DateTime? end)
    {
        var query = _context.Photos.Include(p => p.User).AsQueryable();

        if (!string.IsNullOrEmpty(hashtag))
            query = query.Where(p => p.Hashtags.Contains(hashtag));

        if (!string.IsNullOrEmpty(author))
            query = query.Where(p => (p.User.FirstName + " " + p.User.LastName).Contains(author));

        if (start.HasValue)
            query = query.Where(p => p.UploadDate >= start.Value);

        if (end.HasValue)
            query = query.Where(p => p.UploadDate <= end.Value);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var photos = await query.OrderByDescending(p => p.UploadDate)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize).ToListAsync();  

        return (photos, totalPages);
    }

    /*public async Task<int> GetTodayUploadCount(string userId)
    {
        return await _context.Photos
            .CountAsync(p => p.UserId == userId && p.UploadDate.Date == DateTime.Today);
    }*/
    public async Task<int> GetTodayUploadCount(string userId) =>
    await _context.Photos.CountAsync(p => p.UserId == userId && p.UploadDate.Date == DateTime.Today);


}