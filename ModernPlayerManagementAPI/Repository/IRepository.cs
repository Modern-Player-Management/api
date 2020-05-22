using System;
using System.Collections.Generic;
using ModernPlayerManagementAPI.Models.DTOs;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        ICollection<T> GetAll();
        T GetById(Guid id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(Guid id);
    }
}