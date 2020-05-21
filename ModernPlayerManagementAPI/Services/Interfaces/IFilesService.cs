using System;
using ModernPlayerManagementAPI.Models;

namespace ModernPlayerManagementAPI.Services
{
    public interface IFilesService
    {
        Guid Upload(string fileName, byte[] fileData);
        DbFile Download(Guid fileId);
        void Delete(Guid fileId);
    }
}