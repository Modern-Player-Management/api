using System;
using ModernPlayerManagementAPI.Models;
using ModernPlayerManagementAPI.Repositories;

namespace ModernPlayerManagementAPI.Services
{
    public class FilesService : IFilesService
    {
        private readonly IRepository<DbFile> filesRepository;

        public FilesService(IRepository<DbFile> filesRepository)
        {
            this.filesRepository = filesRepository;
        }

        public Guid Upload(string fileName, byte[] fileData)
        {
            var file = new DbFile()
            {
                Name = fileName,
                FileData = fileData
            };

            filesRepository.Insert(file);
            return file.Id;
        }

        public DbFile Download(Guid fileId)
        {
            return this.filesRepository.GetById(fileId);
        }

        public void Delete(Guid fileId)
        {
            this.filesRepository.Delete(fileId);
        }
    }
}