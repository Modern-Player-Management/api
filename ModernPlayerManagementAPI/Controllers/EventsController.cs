using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(401)]
    public class EventsController : ControllerBase
    {
        private readonly IEventService eventService;
        private readonly IDiscrepancyService discrepancyService;
        
        public EventsController(IEventService eventService, IDiscrepancyService discrepancyService)
        {
            this.eventService = eventService;
            this.discrepancyService = discrepancyService;
        }

        [HttpPost("{eventId:Guid}/confirm")]
        public IActionResult ConfirmEvent(Guid eventId)
        {
            this.eventService.ConfirmEvent(eventId, this.GetCurrentUserId());
            return Ok();
        }

        [HttpPost("{eventId:Guid}/discrepancies")]
        public IActionResult AddDiscrepancy(Guid eventId, [FromBody] UpsertDiscrepancyDTO dto)
        {
            this.eventService.AddDiscrepancy(eventId, dto, this.GetCurrentUserId());
            return Ok();
        }

        [HttpPut("{eventId:Guid}")]
        public IActionResult UpdateEvent([FromBody] UpsertEventDTO dto, Guid eventId)
        {
            if (this.eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this.eventService.UpdateEvent(dto, eventId);

            return Ok();
        }

        [HttpDelete("{eventId:Guid}")]
        public IActionResult DeleteEvent(Guid eventId)
        {
            if (this.eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this.eventService.DeleteEvent(eventId);

            return Ok();
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}