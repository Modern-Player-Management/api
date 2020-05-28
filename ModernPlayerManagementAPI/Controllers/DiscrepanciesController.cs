using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPut("{discrepancyId:Guid")]
        public IActionResult UpdateDiscrepancy(Guid discrepancyId, [FromBody] UpsertDiscrepancyDTO dto)
        {
            if (!this.discrepancyService.IsUserDiscrepancyIssuer(this.GetCurrentUserId(), discrepancyId))
            {
                return Unauthorized();
            }

            this.discrepancyService.UpdateDiscrepancy(discrepancyId, dto);
            return Ok();
        }

        [HttpDelete("{discrepancyId:Guid")]
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