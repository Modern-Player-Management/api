using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Models.Repository;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginDTO dto)
        {
            var user = this._userService.Authenticate(dto.Username, dto.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            LoggedUserDTO responseDTO = new LoggedUserDTO()
            {
                Username = user.Username,
                Email = user.Email,
                Token = user.Token
            };

         
            
            return Ok(responseDTO);
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            var userNameUnique = this._userService.IsUniqueUser(dto.Username);
            if (!userNameUnique)
            {
                return BadRequest("User already exists");
            }

            var user = this._userService.Register(dto.Username, dto.Email,dto.Password);
            if (user == null)
            {
                return BadRequest("Error registering");
            }

            return Ok();
        }
    }
}