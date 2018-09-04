using System.Collections.Generic;
using System.Linq;
using ColoArk.Models;
using Microsoft.EntityFrameworkCore;


namespace ColoArk.DAL
{
    public class EntityRepository<T> : IRepository<T> where T : BaseModel
    {
        private DbContext _context;

        public EntityRepository(DbContext context)
        {
            this._context = context;
        }

        public T Get(int id)
        {
            return this._context.Set<T>().FirstOrDefault(s => s.ID == id);
        }

        public IEnumerable<T> All()
        {
            return this._context.Set<T>();
        }

        public IQueryable<T> AllQueryable()
        {
            return this._context.Set<T>();
        }

        public void Update(T entity)
        {
            this._context.Entry(entity).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        public void Delete(T entity)
        {
            this._context.Set<T>().Remove(entity);
            this._context.SaveChanges();
        }

        public T Add(T entity)
        {
            this._context.Set<T>().Add(entity);
            this._context.SaveChanges();
            return entity;
        }
    }
}
