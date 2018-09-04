using ColoArk.Models;

namespace ColoArk.DAL
{
    public class EfRepository<T> : EntityRepository<T> where T : BaseModel
    {
        public EfRepository(ColoArkContext context) : base(context)
        {
        }
    }
}
