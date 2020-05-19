using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType(typeof(TeamDTO), 401)]
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
        [ProducesResponseType(typeof(TeamDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTeam([FromBody] UpsertTeamDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var team = this._teamService.createTeam(new Team() {Name = dto.Name},
                getCurrentUserId());

            team = this._teamService.getTeamById(team.Id);

            var teamDTO = new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = true,
                Manager = _mapper.Map<UserDTO>(team.Manager),
                Members = team.Members.Select(member => _mapper.Map<UserDTO>(member)).ToList(),
                Created = team.Created
            };

            return Created($"api/Teams/${team.Id}", teamDTO);
        }

        // [HttpGet("{teamId:Guid}")]
        // [ProducesResponseType(typeof(TeamDTO), StatusCodes.Status200OK)]
        // public IActionResult GetTeam(Guid teamId)
        // {
        //     var team = this._teamService.getTeamById(teamId);
        //     return Ok(new TeamDTO()
        //     {
        //         Id = team.Id,
        //         Name = team.Name,
        //         isCurrentUserManager = team.ManagerId == getCurrentUserId(),
        //         Manager = _mapper.Map<UserDTO>(team.Manager),
        //         Members = team.Members.Select(member => _mapper.Map<UserDTO>(member)).ToList()
        //     });
        // }

        [HttpGet]
        [ProducesResponseType(typeof(ICollection<TeamDTO>), StatusCodes.Status200OK)]
        public IActionResult GetTeams()
        {
            return Ok(this._teamService.getTeams(this.getCurrentUserId()).Select(team => new TeamDTO()
            {
                Id = team.Id,
                Name = team.Name,
                isCurrentUserManager = team.ManagerId == getCurrentUserId(),
                Manager = _mapper.Map<UserDTO>(team.Manager),
                Members = team.Members.Select(member => _mapper.Map<UserDTO>(member)).ToList(),
                Created = team.Created
            }));
        }

        private Guid getCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}