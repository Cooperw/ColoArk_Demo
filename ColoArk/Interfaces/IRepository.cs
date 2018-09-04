using ColoArk.Models;
using System.Collections.Generic;
using System.Linq;

namespace ColoArk.DAL
{
    public interface IRepository<T> where T : BaseModel
    {
        T Get(int id);
        IEnumerable<T> All();
        IQueryable<T> AllQueryable();
        void Update(T entity);
        void Delete(T entity);
        T Add(T entity);
    }
}
