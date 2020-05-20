using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModernPlayerManagementAPI.Database;

namespace ModernPlayerManagementAPI.Models.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public ICollection<T> GetAll()
        {
            return _entities.OrderBy(entity => entity.Created).ToList();
        }

        public T GetById(Guid id)
        {
            return (
                from entity in _entities
                where entity.Id == id
                select entity
            ).First();
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            entity.Created = DateTime.Now;
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            this._entities.Update(entity);
            this._context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException();
            }

            T entity = this.GetById(id);
            _entities.Remove(entity);
            this._context.SaveChanges();
        }
    }
}