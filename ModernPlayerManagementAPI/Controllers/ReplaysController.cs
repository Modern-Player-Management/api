using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using RocketLeagueReplayParser;

namespace ModernPlayerManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReplaysController:ControllerBase
    {
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            // var stream = new MemoryStream();
            // file.CopyTo(stream);
            
            var replay = Replay.Deserialize(file.OpenReadStream());
            var playerStats = (List<PropertyDictionary>) replay.Properties.GetValueOrDefault("PlayerStats").Value;
            var names = playerStats.Select(v => v.GetValueOrDefault("Name").Value.ToString());

            return Ok(replay.Properties);
        }
    }
}