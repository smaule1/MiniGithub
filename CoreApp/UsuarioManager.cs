using DataAcces.CRUD;
using DTOs;
using System;
using System.Collections.Generic;

namespace CoreApp
{
    public class UsuarioManager
    {
        private readonly UsuarioCrudFactory _usuarioCrudFactory;

        public UsuarioManager()
        {
            _usuarioCrudFactory = new UsuarioCrudFactory();
        }

        public void Create(Usuario usuario)
        {
            _usuarioCrudFactory.Create(usuario);
        }

        public void Update(Usuario usuario)
        {
            _usuarioCrudFactory.Update(usuario);
        }

        public void Delete(Usuario usuario)
        {
            _usuarioCrudFactory.Delete(usuario);
        }

        public List<Usuario> RetrieveAll()
        {
            return _usuarioCrudFactory.RetrieveAll<Usuario>();
        }

        public Usuario RetrieveById(int id)
        {
            return _usuarioCrudFactory.RetrieveByID<Usuario>(id);
        }

        // Nuevo método para manejar el login
        public Usuario Login(string Email, string Password)
        {
            var uc = new UsuarioCrudFactory();

            var actualUser = uc.RetrieveByEmail(Email, Password);
            if (actualUser == null)
            {
                throw new Exception("Usuario no encontrado");

            }
            else
            {
                return actualUser;
            
            }
        }

        
        
    }
}
