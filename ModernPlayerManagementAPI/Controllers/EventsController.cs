using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(401)]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("{eventId:Guid}/confirm")]
        public IActionResult ConfirmEvent(Guid eventId)
        {
            if (this._eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this._eventService.ConfirmEvent(eventId, this.GetCurrentUserId());
            return Ok();
        }

        [HttpPost("{eventId:Guid}/discrepancies")]
        public IActionResult AddDiscrepancy(Guid eventId, [FromBody] UpsertDiscrepancyDTO dto)
        {
            if (this._eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this._eventService.AddDiscrepancy(eventId, dto, this.GetCurrentUserId());
            return Ok();
        }

        [HttpPut("{eventId:Guid}")]
        public IActionResult UpdateEvent([FromBody] UpsertEventDTO dto, Guid eventId)
        {
            if (this._eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this._eventService.UpdateEvent(dto, eventId);

            return Ok();
        }

        [HttpDelete("{eventId:Guid}")]
        public IActionResult DeleteEvent(Guid eventId)
        {
            if (this._eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this._eventService.DeleteEvent(eventId);

            return Ok();
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}