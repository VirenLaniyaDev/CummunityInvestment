using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommunityInvestment.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(Expression<Func<T, bool>> filter);
    }
}
