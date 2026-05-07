using BusinessLogic.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPTesting
{
    public class PhotoServiceTests
    {

        /*
         * Creates a file and it makes sure it gets deleted from the file system
         */
        [Fact]
        public void DeleteFile_RemovesFile_WhenFileExists()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);

            var relativePath = "existingFile.jpg";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            var directory = Path.GetDirectoryName(fullPath);
            File.WriteAllText(fullPath, "Lorem ipsum"); 

            var service = new PhotoService(context); 
            
            service.DeleteFile(relativePath);

            Assert.False(File.Exists(fullPath));
        }

        /*
         * Checks if error is returned for non existent files
         */
        [Fact]
        public void DeleteFile_DoesNotThrow_WhenFileDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);
            
            var fakePath = "nonexistentfile.jpg";
            var service = new PhotoService(context);

            var exception = Record.Exception(() => service.DeleteFile(fakePath));
            Assert.Null(exception); 
        }


        /*
         * Check amount of returned pictures is as asked for
         */
        [Fact]
        public async Task GetPagedPhotos_ShouldReturnExactlyPageSizeCount()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            using var context = new ApplicationDbContext(options);
            var testUser = new ApplicationUser { Id = "user1", FirstName = "Test", LastName = "User" };
            context.Users.Add(testUser);
            for (int i = 1; i <= 15; i++)
            {
                context.Photos.Add(new Photo
                {
                    Id = i,
                    ImageUrl = $"/test{i}.jpg",
                    Description = "Test",
                    Hashtags = "#test",
                    User = testUser,
                    UserId = "user1",
                    UploadDate = DateTime.Now.AddMinutes(i)
                });
            }
            await context.SaveChangesAsync();

            var service = new PhotoService(context); 

            var (photos, totalPages) = await service.GetPagedPhotos(page: 1, pageSize: 10, null, null, null, null);

            Assert.Equal(10, photos.Count);
        }
    }
}
