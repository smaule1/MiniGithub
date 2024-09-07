using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Review : BaseDTO
    {
        public int Rating { get; set; }
        public int UsuarioId { get; set; }  // Cambio de 'usuarioId'
        public int ContenidoId { get; set; } // Cambio de 'contenidoId'
        public string Texto { get; set; }
    }
}

