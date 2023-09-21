using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // It is an Ienumerable as a list of all categories will be returned by the function
        IEnumerable<T> GetAll();
        // An object of type T will be returned.
        // Parameters: LINQ Expression
        T Get(Expression<Func<T,bool>> filter);

        // No return type
        // Parameter: Object of type T 
        void Add(T item);

        // No return type
        // Parameter: Object of type T 
        void Delete(T entity);

        // No return type
        // Parameter: list of Objects of type T 
        void DeleteRange(IEnumerable<T> entities);

    }
}
