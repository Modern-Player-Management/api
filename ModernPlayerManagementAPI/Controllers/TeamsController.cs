using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamsController(ITeamService _teamService, IMapper _mapper)
        {
            this._teamService = _teamService;
            this._mapper = _mapper;
        }

        /// <summary>
        /// Create a new team
        /// </summary>
        /// <param name="dto">The team infos</param>
        /// <returns>The created team</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TeamDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTeam([FromBody] UpsertTeamDTO dto)
        {
            var team = this._teamService.createTeam(dto, getCurrentUserId());

            return Created($"api/Teams/${team.Id}", team);
        }

        /// <summary>
        /// Gets all the user's teams (teams in which the user is either manager or member
        /// </summary>
        /// <returns>The corresponding teams</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<TeamDTO>), StatusCodes.Status200OK)]
        public IActionResult GetTeams()
        {
            return Ok(this._teamService.getTeams(this.getCurrentUserId()));
        }

        /// <summary>
        /// Adds a player to a team
        /// </summary>
        /// <param name="teamId">Id of the team in which the player should be added</param>
        /// <param name="playerId">Id of the user to add in a team</param>
        [HttpPost("{teamId:Guid}/player/{playerId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddPlayerToTeam(Guid teamId, Guid playerId)
        
        {
            if (!this._teamService.IsUserTeamManager(teamId, this.getCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.addPlayer(teamId, new UserDTO(){Id = playerId});
            return Ok();
        }

        /// <summary>
        /// Removes a player from a team
        /// </summary>
        /// <param name="teamId">Id of the team from which the player should be removed</param>
        /// <param name="playerId">Id of the user to remove from a team</param>
        [HttpDelete("{teamId:Guid}/player/{playerId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult RemovePlayerToTeam(Guid teamId, Guid playerId)
        {
            if (!this._teamService.IsUserTeamManager(teamId, this.getCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.removePlayer(teamId, new UserDTO(){Id = playerId});
            return Ok();
        }

        /// <summary>
        /// Updates a team
        /// </summary>
        /// <param name="teamId">Id of the team to update</param>
        /// <param name="dto">DTO containing the new info of the team</param>
        [HttpPut("{teamId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateTeam(Guid teamId, [FromBody] UpsertTeamDTO dto)
        {
            if (!this._teamService.IsUserTeamManager(teamId, this.getCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.UpdateTeam(teamId, dto);

            return Ok();
        }

        /// <summary>
        /// Deletes a team
        /// </summary>
        /// <param name="teamId">Id of the team to delete</param>
        [HttpDelete("{teamId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteTeam(Guid teamId)
        {
            if (!this._teamService.IsUserTeamManager(teamId, this.getCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.DeleteTeam(teamId);

            return Ok();
        }

        private Guid getCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}