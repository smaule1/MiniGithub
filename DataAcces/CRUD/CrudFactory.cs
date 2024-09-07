using DataAccess.DAOs;
using DTOs;

namespace DataAccess.CRUD
{
    public abstract class CrudFactory
    {

        protected SqlDao _dao;

        public abstract void Create(BaseDTO baseDTO);
        public abstract void Update(BaseDTO baseDTO);
        public abstract void Delete(BaseDTO baseDTO);

        public abstract T Retrieve<T>(BaseDTO baseDTO);

        public abstract T RetrieveByID<T>(int id);


        public abstract List<T> RetrieveAll<T>();
    }
}
