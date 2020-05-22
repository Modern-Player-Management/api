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

        /// <summary>
        /// Saves a file in the backend
        /// </summary>
        /// <param name="file">The file as multipart formdata</param>
        /// <returns>A DTO containing infos about the saved files and its location</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DbFileDTO), StatusCodes.Status200OK)]
        public IActionResult Upload(IFormFile file)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var fileId = this._filesService.Upload(file.FileName, stream.ToArray());
            return Ok(new DbFileDTO() {Id = fileId, Name = file.FileName, Path = new Uri($"{Request.GetDisplayUrl()}/{fileId}").ToString()});
        }

        /// <summary>
        /// Downloads a file from the backend
        /// </summary>
        /// <param name="fileId">Id of the file</param>
        /// <returns>The file as octet stream</returns>
        [HttpGet("{fileId:Guid}")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public IActionResult Download(Guid fileId)
        {
            var dbFile = this._filesService.Download(fileId);
            return File(dbFile.FileData, "application/octet-stream", dbFile.Name);
        }
    }
}