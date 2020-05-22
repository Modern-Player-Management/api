using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// Static search of a string in the users
        /// </summary>
        /// <param name="search">The string to search for</param>
        /// <returns>A list of users whom username contains the search string</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ICollection<UserDTO>), StatusCodes.Status200OK)]
        public IActionResult SearchUser([FromQuery(Name = "search")] string search)
        {
            return Ok(this._userService.SearchUser(search));
        }

        /// <summary>
        /// Gets an User from its username
        /// </summary>
        /// <param name="username">The username of the user to get</param>
        /// <returns>Infos about the users</returns>
        [HttpGet("{username:string}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(string username)
        {
            UserDTO dto = null;
            try
            {
                dto = this._userService.GetFromUsername(username);
            }
            catch
            {
                return NotFound();
            }

            return Ok(dto);
        }
        
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserDTO dto)
        {
            this._userService.Update(dto);
            return Ok();
        }
    }
}