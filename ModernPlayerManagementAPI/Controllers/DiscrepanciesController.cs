using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services.Interfaces;

namespace ModernPlayerManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(401)]
    public class DiscrepanciesController : ControllerBase
    {
        private readonly IDiscrepancyService discrepancyService;

        public DiscrepanciesController(IDiscrepancyService discrepancyService)
        {
            this.discrepancyService = discrepancyService;
        }

        /// <summary>
        /// Updates a discrepancy (Team Manager Only)
        /// </summary>
        /// <param name="discrepancyId">Id of the discrepancy</param>
        /// <param name="dto">DTO containing the new data</param>
        [HttpPut("{discrepancyId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateDiscrepancy(Guid discrepancyId, [FromBody] UpsertDiscrepancyDTO dto)
        {
            if (!this.discrepancyService.IsUserDiscrepancyIssuer(this.GetCurrentUserId(), discrepancyId))
            {
                return Unauthorized();
            }

            this.discrepancyService.UpdateDiscrepancy(discrepancyId, dto);
            return Ok();
        }

        /// <summary>
        /// Deletes a discrepancy (Team Manager Only)
        /// </summary>
        /// <param name="discrepancyId">Id of the discrepancy</param>
        [HttpDelete("{discrepancyId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DeleteDiscrepancy(Guid discrepancyId)
        {
            if (!this.discrepancyService.IsUserDiscrepancyIssuer(this.GetCurrentUserId(), discrepancyId))
            {
                return Unauthorized();
            }

            this.discrepancyService.DeleteDiscrepancy(discrepancyId);
            return Ok();
        }

        private Guid GetCurrentUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}