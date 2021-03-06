﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models;
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

        /// <summary>
        /// Confirms the presence of the current user to an event
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        [HttpPost("{eventId:Guid}/presence")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SetPresence(Guid eventId, [FromBody] EventPresenceDTO dto)
        {
            this.eventService.SetPresence(eventId, this.GetCurrentUserId(), dto);
            return Ok();
        }

        /// <summary>
        /// Adds a discrepancy for the user to an event
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        /// <param name="dto">DTO containing the infos of the discrepancy</param>
        [HttpPost("{eventId:Guid}/discrepancies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddDiscrepancy(Guid eventId, [FromBody] UpsertDiscrepancyDTO dto)
        {
            this.eventService.AddDiscrepancy(eventId, dto, this.GetCurrentUserId());
            return Ok();
        }

        /// <summary>
        /// Updates an event
        /// </summary>
        /// <param name="dto">New infos about the event</param>
        /// <param name="eventId">Id of the event</param>
        [HttpPut("{eventId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateEvent([FromBody] UpsertEventDTO dto, Guid eventId)
        {
            if (!this.eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
            {
                return Unauthorized();
            }

            this.eventService.UpdateEvent(dto, eventId);

            return Ok();
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="eventId">Id of the event</param>
        [HttpDelete("{eventId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteEvent(Guid eventId)
        {
            if (!this.eventService.IsUserTeamManager(eventId, this.GetCurrentUserId()))
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

        /// <summary>
        /// Get the users ICAL feed with all his upcoming events
        /// </summary>
        /// <param name="icalSecret">The users ICAL secret</param>
        /// <returns>The ICAL feed</returns>
        [AllowAnonymous]
        [HttpGet("ical/{icalSecret:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetICalFeed(Guid icalSecret)
        {
            var calendar = this.eventService.GetUserCalendar(icalSecret);
            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(serializedCalendar);

            return File(bytes, contentType, "calendar.ics");
        }

        /// <summary>
        /// Gets a list of all the event types available in MPM
        /// </summary>
        /// <returns>List of the event types</returns>
        [AllowAnonymous]
        [HttpGet("types")]
        [ProducesResponseType(typeof(List<string>),StatusCodes.Status200OK)]
        public IActionResult GetTypes()
        {
            return Ok(Enum.GetValues(typeof(Event.EventType)).Cast<Event.EventType>().Select(v => v.ToString()).ToList());
        }
    }
}