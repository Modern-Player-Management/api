using System;

namespace ModernPlayerManagementAPI.Services.Interfaces
{
    public interface IGameService
    {
        void DeleteGame(Guid gameId);
    }
}