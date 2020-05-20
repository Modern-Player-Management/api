using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ModernPlayerManagementAPI.Models.DTOs;
using ModernPlayerManagementAPI.Services;

namespace ModernPlayerManagementAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(401)]
    public class FilesController : ControllerBase
    {
        private readonly IFilesService _filesService;

        public FilesController(IFilesService filesService)
        {
            _filesService = filesService;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var fileId = this._filesService.Upload(file.FileName, stream.ToArray());
            return Ok(new DbFileDTO() {Id = fileId, Name = file.FileName, Path = new Uri($"{Request.GetDisplayUrl()}/{fileId}").ToString()});
        }

        [HttpGet("{fileId:Guid}")]
        public IActionResult Download(Guid fileId)
        {
            var dbFile = this._filesService.Download(fileId);
            return File(dbFile.FileData, "application/octet-stream", dbFile.Name);
        }
    }
}