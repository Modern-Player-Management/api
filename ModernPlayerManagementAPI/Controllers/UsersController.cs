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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFilesService _filesService;

        public UsersController(IUserService userService, IFilesService filesService)
        {
            _userService = userService;
            _filesService = filesService;
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserDTO dto)
        {
            this._userService.Update(dto);
            return Ok();
        }
    }
}