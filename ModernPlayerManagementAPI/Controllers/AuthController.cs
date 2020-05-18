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
        private readonly IEmailValidator _emailValidator;
        
        public AuthController(IUserService userService, IEmailValidator emailValidator)
        {
            _userService = userService;
            _emailValidator = emailValidator;
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

            if (!this._emailValidator.IsValidEmail(dto.Email))
            {
                return BadRequest("Invalid Email");
            }
            
            var user = this._userService.Register(dto.Username, dto.Email,dto.Password);
            if (user == null)
            {
                return BadRequest("Error registering");
            }

            return Ok();
        }

        [HttpPost("available")]
        public IActionResult CheckUsernameUsage([FromBody] UsernameCheckDTO dto)
        {
            var responseDTO = new UsernameAvailabilityDTO()
            {
                Username = dto.Username,
                IsAvailable = this._userService.IsUniqueUser(dto.Username)
            };

            return Ok(responseDTO);
        }
    }
}