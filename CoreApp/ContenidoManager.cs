using DTOs;
using DataAccess.CRUD;
using DataAcces.CRUD;


namespace CoreApp
{
    public class ContenidoManager
    {
        public void Create(Contenido contenido)
        {
            var cont = new ContenidoCrudFactory();
            cont.Create(contenido);
        }

        public void Update(Contenido contenido)
        {
            var cont = new ContenidoCrudFactory();
            cont.Update(contenido);
        }

        public void Delete(Contenido contenido)
        {
            var cont = new ContenidoCrudFactory();
            cont.Delete(contenido);
        }

        public List<Contenido> RetrieveAll()
        {
            var cont = new ContenidoCrudFactory();
            return cont.RetrieveAll<Contenido>();
        }

        public Contenido RetrieveById(int id)
        {
            var cont = new ContenidoCrudFactory();
            return cont.Retrieve<Contenido>(new Contenido { Id = id });
        }


    }
}
