using System;
using System.Collections.Generic;
using System.Linq;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services;
using ModernPlayerManagementAPI.Services.Interfaces;
using Moq;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class DiscrepancyServiceTest
    {
        private List<Discrepancy> discrepancies;
        private IDiscrepancyService service;

        void setup()
        {
            this.discrepancies = new List<Discrepancy>();

            var repo = new Mock<IRepository<Discrepancy>>();

            repo.Setup(mock => mock.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(discrepancyId => this.discrepancies.First(d => d.Id == discrepancyId));

            repo.Setup(mock => mock.Update(It.IsAny<Discrepancy>()))
                .Callback<Discrepancy>(d =>
                {
                    this.discrepancies.Remove(this.discrepancies.First(e => e.Id == d.Id));
                    this.discrepancies.Add(d);
                });

            repo.Setup(mock => mock.Delete(It.IsAny<Guid>()))
                .Callback<Guid>(id => this.discrepancies.Remove(this.discrepancies.First(e => e.Id == id)));

            service = new DiscrepancyService(repo.Object);
        }

        [Fact]
        public void IsUserDiscrepancyIssuer_Is_Issuer_Returns_True_Test()
        {
            this.setup();

            // Given
            var user = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now, DelayLength = 15, Id = Guid.NewGuid(), Reason = "Test Reason",
                Type = Discrepancy.DiscrepancyType.Absence, User = user, UserId = user.Id
            };

            this.discrepancies.Add(discrepancy);

            // When
            bool result = this.service.IsUserDiscrepancyIssuer(user.Id, discrepancy.Id);

            // Then
            Assert.True(result);
        }

        [Fact]
        public void IsUserDiscrepancyIssuer_Is_Not_Issuer_Returns_False_Test()
        {
            this.setup();

            // Given
            var user1 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var user2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now, DelayLength = 15, Id = Guid.NewGuid(), Reason = "Test Reason",
                Type = Discrepancy.DiscrepancyType.Absence, User = user2, UserId = user2.Id
            };

            this.discrepancies.Add(discrepancy);

            // When
            bool result = this.service.IsUserDiscrepancyIssuer(user1.Id, discrepancy.Id);

            // Then
            Assert.False(result);
        }

        [Fact]
        public void DeleteDiscrepancy_Test()
        {
            this.setup();
            // Given
            var user2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now, DelayLength = 15, Id = Guid.NewGuid(), Reason = "Test Reason",
                Type = Discrepancy.DiscrepancyType.Absence, User = user2, UserId = user2.Id
            };
            this.discrepancies.Add(discrepancy);

            // When
            this.service.DeleteDiscrepancy(discrepancy.Id);

            // Then
            Assert.Empty(this.discrepancies);
        }

        [Fact]
        public void UpdateDiscrepancy_Test()
        {
            this.setup();
            // Given
            var user2 = new User {Username = "Ombrelin", Email = "arsene@lapostolet.fr", Id = Guid.NewGuid()};
            var discrepancy = new Discrepancy()
            {
                Created = DateTime.Now, DelayLength = 15, Id = Guid.NewGuid(), Reason = "Test Reason",
                Type = Discrepancy.DiscrepancyType.Absence, User = user2, UserId = user2.Id
            };
            this.discrepancies.Add(discrepancy);

            var dto = new UpsertDiscrepancyDTO()
            {
                DelayLength = 15,
                Reason = "Test Reason Updated",
                Type = Discrepancy.DiscrepancyType.Absence
            };

            // When
            this.service.UpdateDiscrepancy(discrepancy.Id, dto);

            // Then
            Assert.Equal("Test Reason Updated", this.discrepancies.First().Reason);
        }
    }
}