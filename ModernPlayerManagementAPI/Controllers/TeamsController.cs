using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;
using RocketLeagueReplayParser;

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
        public IActionResult CreateTeam([FromBody] InsertTeamDTO dto)
        {
            var team = this._teamService.CreateTeam(dto, GetCurrentUserId());

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
            return Ok(this._teamService.GetTeams(this.GetCurrentUserId()));
        }

        /// <summary>
        /// Gets a team from its Id with details like events and games
        /// </summary>
        /// <returns>The corresponding team</returns>
        [HttpGet("{teamId:Guid}")]
        [ProducesResponseType(typeof(TeamDTODetailed), StatusCodes.Status200OK)]
        public IActionResult GetTeam(Guid teamId)
        {
            return Ok(this._teamService.GetTeam(teamId, this.GetCurrentUserId()));
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
            if (!this._teamService.IsUserTeamManager(teamId, this.GetCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.AddPlayer(teamId, new UserDTO() {Id = playerId});
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
            if (!this._teamService.IsUserTeamManager(teamId, this.GetCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.RemovePlayer(teamId, new UserDTO() {Id = playerId});
            return Ok();
        }

        /// <summary>
        /// Updates a team
        /// </summary>
        /// <param name="teamId">Id of the team to update</param>
        /// <param name="dto">DTO containing the new info of the team</param>
        [HttpPut("{teamId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateTeam(Guid teamId, [FromBody] UpdateTeamDTO dto)
        {
            if (!this._teamService.IsUserTeamManager(teamId, this.GetCurrentUserId()))
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
            if (!this._teamService.IsUserTeamManager(teamId, this.GetCurrentUserId()))
            {
                return Unauthorized("You are not the manager of this team");
            }

            this._teamService.DeleteTeam(teamId);

            return Ok();
        }

        /// <summary>
        /// Add an event to a team
        /// </summary>
        /// <param name="teamId">Id of the team in which the team will be added</param>
        /// <param name="dto">Infos about the event</param>
        /// <returns>Infos about the created event</returns>
        [HttpPost("{teamId:Guid}/events")]
        [ProducesResponseType(typeof(EventDTO), StatusCodes.Status200OK)]
        public IActionResult AddEvent(Guid teamId, [FromBody] UpsertEventDTO dto)
        {
            if (this._teamService.IsUserTeamManager(teamId, this.GetCurrentUserId()))
            {
                return Ok(this._teamService.AddEvent(teamId, dto));
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Add a replay file to the team
        /// </summary>
        /// <param name="file">The .replay file from Rocket League</param>
        /// <param name="teamId">The Id of the team on which to add the game</param>
        /// <returns>A DTO tat represents the Game</returns>
        [HttpPost("{teamId:Guid}/games")]
        [ProducesResponseType(typeof(GameDTO), StatusCodes.Status200OK)]
        public IActionResult UploadReplay(IFormFile file, Guid teamId)
        {
            var replay = Replay.Deserialize(file.OpenReadStream());

            return Ok(this._teamService.AddGame(replay, teamId));
        }

        /// <summary>
        /// Computes the average stats of the players based on the uploaded games
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <returns>The average player stat for each player</returns>
        [HttpGet("{teamId:Guid}/stats")]
        [ProducesResponseType(typeof(PlayerStatsAvgDTO), StatusCodes.Status200OK)]
        public IActionResult GetStats(Guid teamId)
        {
            var playerStats = this._teamService.GetStats(teamId);
            return Ok(playerStats);
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}