using System;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Repositories;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> gameRepository;

        public GameService(IRepository<Game> gameRepository)
        {
            this.gameRepository = gameRepository;
        }

        public void DeleteGame(Guid gameId)
        {
            this.gameRepository.Delete(gameId);
        }
    }
}