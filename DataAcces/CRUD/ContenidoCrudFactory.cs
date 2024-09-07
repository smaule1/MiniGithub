using DataAccess.CRUD;
using DataAccess.DAOs;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces.CRUD
{
    public class ContenidoCrudFactory : CrudFactory
    {

        public ContenidoCrudFactory()
        {
            _dao = SqlDao.GetInstance();
        }
        
        public override void Create(BaseDTO baseDTO)
        {

            var contenido = baseDTO as Contenido;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_CreateContenido"
            };

            sqlOperation.AddVarCharParameter("Titulo", contenido.Titulo);
            sqlOperation.AddVarCharParameter("Autor", contenido.Autor);
            sqlOperation.AddVarCharParameter("TipoContenido", contenido.TipoContenido);

            _dao.ExecuteProcedure(sqlOperation);



        }

        public override void Delete(BaseDTO baseDTO)
        {
            var contenido = baseDTO as Contenido;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_DeleteContenido"
            };

            sqlOperation.AddIntParameter("Id", contenido.Id);
            _dao.ExecuteProcedure(sqlOperation);

        }

        public override T Retrieve<T>(BaseDTO baseDTO)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveAll<T>()
        {
            var listaContenidos = new List<T>();
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetAllContenidos"
            };
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if (data.Count > 0)
            {
                foreach (var row in data)
                {
                    var contenido = BuildObject(row);
                    listaContenidos.Add((T)Convert.ChangeType(contenido, typeof(T)));
                }
            }
            return listaContenidos;
        }

        public override T RetrieveByID<T>(int id)
        {
            
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetContenidoById"
            };
            sqlOperation.AddIntParameter("Id", id);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if (data.Count > 0)
            {
                var row = data[0];
                var contenido = BuildObject(row);
                return (T)Convert.ChangeType(contenido, typeof(T));
            }
            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var contenido = baseDTO as Contenido;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_UpdateContenido"
            };

            sqlOperation.AddIntParameter("Id", contenido.Id);
            sqlOperation.AddVarCharParameter("Titulo", contenido.Titulo);
            sqlOperation.AddVarCharParameter("Autor", contenido.Autor);

            sqlOperation.AddVarCharParameter("TipoContenido", contenido.TipoContenido);
            _dao.ExecuteProcedure(sqlOperation);
        }
        /*
         private Usuario BuildObject(Dictionary<string, object> row)
        {
            var usuario = new Usuario
            {
                Id = (int)row["id"],
                Nombre = (string)row["nombre"],
                Email = (string)row["email"],
                Password = (string)row["contraseña"]

            };
            return usuario;
        }
         */
        private Contenido BuildObject(Dictionary<string, object> row)
        {
            var contenido = new Contenido
            {
                Id = (int)row["Id"],
                Titulo = (string)row["Titulo"],
                Autor = (string)row["Autor"],
                TipoContenido = (string)row["TipoContenido"]



            };
            return contenido;
        }
    }
}
