using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models.DTOs;
using RocketLeagueReplayParser;

namespace ModernPlayerManagementAPI.Services
{
    public interface ITeamService
    {
        TeamDTO createTeam(InsertTeamDTO team, Guid currentUserId);
        public TeamDTODetailed GetTeam(Guid id, Guid userId);
        ICollection<TeamDTO> GetTeams(Guid userId);
        void AddPlayer(Guid teamId, UserDTO player);
        void RemovePlayer(Guid teamId, UserDTO dto);
        void UpdateTeam(Guid teamId, UpdateTeamDTO team);
        void DeleteTeam(Guid Id);
        bool IsUserTeamManager(Guid teamId, Guid userId);
        EventDTO AddEvent(Guid teamId, UpsertEventDTO dto);
        GameDTO AddGame(Replay replay, Guid teamId);
    }
}