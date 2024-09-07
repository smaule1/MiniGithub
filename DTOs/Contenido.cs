using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Contenido : BaseDTO
    {

        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string TipoContenido { get; set; }

    }
}
