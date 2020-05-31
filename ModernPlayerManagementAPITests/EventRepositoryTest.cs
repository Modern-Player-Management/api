using System;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Repositories;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class EventRepositoryTest
    {
        private ApplicationDbContext context;

        public EventRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ApplicationDatabase")
                .Options;

            this.context = new ApplicationDbContext(options);
            this.context.Database.EnsureDeleted();
        }

        [Fact]
        void GetById()
        {
            // Given
            var evt = new Event()
            {
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Event Description"
            };
            context.Events.Add(evt);

            context.SaveChanges();

            // When
            var repo = new EventRepository(context);
            var e = repo.GetById(evt.Id);

            // Then
            Assert.Equal("Test Event", e.Name);
            Assert.Equal("Test Event Description", e.Description);
        }
    }
}