using System;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Repositories;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class UserRepositoryTest
    {
        private ApplicationDbContext context;

        void setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplicationDatabase")
                .Options;

            this.context = new ApplicationDbContext(options);
            this.context.Database.EnsureDeleted();
        }

        [Fact]
        void GetUserByUsernameTest()
        {
            this.setup();
            // Given
            var manager = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr"};
            context.Users.Add(manager);
            context.SaveChanges();
            
            // When 
            var repo = new UserRepository(context);
            var user = repo.GetUserByUsername("Ombrelin");
            
            // Then
            Assert.Equal("Ombrelin", user.Username);
        }
        
        [Fact]
        void FindUsersByUsernameContainsTest()
        {
            this.setup();
            // Given
            var manager1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr"};
            var manager2 = new User {Username = "Jean Michel", Email = "arsene@lapostolet.fr"};
            context.Users.Add(manager1);
            context.Users.Add(manager2);
            context.SaveChanges();
            
            // When 
            var repo = new UserRepository(context);
            var users = repo.findUsersByUsernameContains("Ombre");
            
            // Then
            Assert.Equal(1, users.Count);
        }

    }
}