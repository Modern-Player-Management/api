using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEmailValidator emailValidator;

        public AuthController(IUserService userService, IEmailValidator emailValidator)
        {
            this.userService = userService;
            this.emailValidator = emailValidator;
        }

        /// <summary>
        /// Authenticate an user with his username and password
        /// </summary>
        /// <param name="dto">The user's login infos</param>
        /// <returns>Infos about the user and his JWT Token</returns>
        [HttpPost("authenticate")]
        [ProducesResponseType(typeof(LoggedUserDTO), StatusCodes.Status200OK)]
        public ActionResult<LoggedUserDTO> Authenticate([FromBody] LoginDTO dto)
        {
            var user = this.userService.Authenticate(dto.Username, dto.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var responseDto = new LoggedUserDTO()
            {
                Username = user.Username,
                Email = user.Email,
                Token = user.Token,
                Id = user.Id,
                Image = user.Image
            };
            return responseDto;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="dto">The user's infos (email, username and password)</param>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            var userNameUnique = this.userService.IsUniqueUser(dto.Username);
            if (!userNameUnique)
            {
                return BadRequest("User already exists");
            }

            if (!this.emailValidator.IsValidEmail(dto.Email))
            {
                return BadRequest("Invalid Email");
            }

            var user = this.userService.Register(dto.Username, dto.Email, dto.Password);
            if (user == null)
            {
                throw new Exception("Error registering user");
            }

            return Ok();
        }

        /// <summary>
        /// Checks if a username is available
        /// </summary>
        /// <param name="dto">The username</param>
        /// <returns>True if the username is available, false otherwise</returns>
        [HttpPost("available")]
        [ProducesResponseType(typeof(UsernameAvailabilityDTO), StatusCodes.Status200OK)]
        public ActionResult<UsernameAvailabilityDTO> CheckUsernameUsage([FromBody] UsernameCheckDTO dto)
        {
            var responseDto = new UsernameAvailabilityDTO()
            {
                Username = dto.Username,
                IsAvailable = this.userService.IsUniqueUser(dto.Username)
            };

            return responseDto;
        }
    }
}