using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Mapper;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Models.Repository;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamsController(ITeamService _teamService, IMapper _mapper)
        {
            this._teamService = _teamService;
            this._mapper = _mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TeamDTO), 201)]
        public IActionResult CreateTeam([FromBody] UpsertTeamDTO dto)
        {
            var team = this._teamService.createTeam(new Team() {Name = dto.Name},
                getCurrentUserId());

            team = this._teamService.getTeamById(team.Id);

            var teamDTO = new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = true,
                Manager = _mapper.Map<UserDTO>(team.Manager),
                Members = team.Members.Select(member => _mapper.Map<UserDTO>(member)).ToList()
            };

            return Created($"api/Teams/${team.Id}", team);
        }


        [HttpGet]
        [ProducesResponseType(typeof(ICollection<TeamDTO>), 200)]
        public IActionResult GetTeams()
        {
            return Ok(this._teamService.getTeams(this.getCurrentUserId()).Select(team => new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = team.ManagerId == getCurrentUserId(),
                Manager = _mapper.Map<UserDTO>(team.Manager),
                Members = team.Members.Select(member => _mapper.Map<UserDTO>(member)).ToList()
            }));
        }

        private Guid getCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}