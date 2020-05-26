using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Services
{
    public interface ITeamService
    {
        TeamDTO createTeam(UpsertTeamDTO team, Guid currentUserId);
        TeamDTO getTeamById(Guid id);
        ICollection<TeamDTO> getTeams(Guid userId);
        void addPlayer(Guid teamId, UserDTO player);
        void removePlayer(Guid teamId, UserDTO dto);
        void UpdateTeam(Guid teamId, UpsertTeamDTO team);
        void DeleteTeam(Guid Id);
        bool IsUserTeamManager(Guid teamId, Guid userId);
        EventDTO AddEvent(Guid teamId, UpsertEventDTO dto);
    }
}