using BusinessLogic.Models;
using Microsoft.AspNetCore.Http; // Add this

public interface IPhotoService
{
    Task<(List<Photo> Photos, int TotalPages)> GetPagedPhotos(
    int page,
    int pageSize,
    string? hashtag = null,
    string? author = null,
    DateTime? start = null,
    DateTime? end = null);
    Task<string> SaveFileAsync(IFormFile file, string format, bool resize); 
    void DeleteFile(string filePath);
    Task<int> GetTodayUploadCount(string userId);
}