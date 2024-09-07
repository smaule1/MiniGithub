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
    public class UsuarioCrudFactory : CrudFactory 
    {

        public UsuarioCrudFactory()
        {
            _dao = SqlDao.GetInstance();
        }
        
        public override void Create(BaseDTO baseDTO)
        {
            var usuario = baseDTO as Usuario;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_CreateUsuario"
            };

            sqlOperation.AddVarCharParameter("Nombre", usuario.Nombre);
            sqlOperation.AddVarCharParameter("Email", usuario.Email);
            sqlOperation.AddVarCharParameter("Contraseña", usuario.Password);
            _dao.ExecuteProcedure(sqlOperation);
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var usuario = baseDTO as Usuario;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_DeleteUsuario"
            };
        }

        public override T Retrieve<T>(BaseDTO baseDTO)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveAll<T>()
        {
            var listaUsuarios = new List<T>();
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetAllUsuarios"
            };
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if (data.Count > 0)
            {
                foreach (var row in data)
                {
                    var usuario = BuildObject(row);
                    listaUsuarios.Add((T)Convert.ChangeType(usuario, typeof(T)));
                }
            }
            return listaUsuarios;
        }

        public override T RetrieveByID<T>(int id)
        {
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetUsuarioById"
            };
            sqlOperation.AddIntParameter("Id", id);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if (data.Count > 0)
            {
                var row = data[0];
                var usuario = BuildObject(row);
                return (T)Convert.ChangeType(usuario, typeof(T));
            }

            return default(T);
        }

        public override void Update(BaseDTO baseDTO)
        {
            var usuario = baseDTO as Usuario;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_UpdateUsuario"
            };

            sqlOperation.AddIntParameter("Id", usuario.Id);
            sqlOperation.AddVarCharParameter("Nombre", usuario.Nombre);
            sqlOperation.AddVarCharParameter("Email", usuario.Email);
            sqlOperation.AddVarCharParameter("Contraseña", usuario.Password);
            _dao.ExecuteProcedure(sqlOperation);
        }

        private Usuario BuildObject(Dictionary<string, object> row)
        {
            var usuario = new Usuario
            {
                Id = (int)row["Id"],
                Nombre = (string)row["Nombre"],
                Email = (string)row["Email"],
                Password = (string)row["Contraseña"]

            };
            return usuario;
        }

        public Usuario RetrieveByEmail(string Email, string Password)
        {
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetUsuarioByEmail"
            };
            sqlOperation.AddVarCharParameter("Email", Email);
            sqlOperation.AddVarCharParameter("Password", Password);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if (data.Count > 0)
            {
                var row = data[0];
                
                var usuario = BuildObject(row);
                return usuario;
            }
            return null;
        }


    }
}
