using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeChatAppDAL.Repos.Base
{
    public interface IRepo<T>
    {
        int Add(T entity);
        int AddRange(IList<T> entities);
        int Save(T entity);
        int Delete(int id, byte[] timeStamp);
        int Delete(T entity);
        T GetOne(int? id);
        List<T> GetAll();
    }
}
