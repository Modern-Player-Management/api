using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameService;

        public GamesController(IGameService gameService)
        {
            this.gameService = gameService;
        }

        /// <summary>
        /// Deletes a game from its Id
        /// </summary>
        /// <param name="gameId">Id of the game to delete</param>
        [HttpDelete("{gameId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteGame(Guid gameId)
        {
            this.gameService.DeleteGame(gameId);
            return Ok();
        }
    }
}