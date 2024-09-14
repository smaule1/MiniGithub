using Microsoft.Build.Framework;

namespace ApiWeb.Models
{
    public class Branch
    {
        [Required]
        public string Nombre { get; set; }        //Unique
        public string? Latest_Commit { get; set; }

        public Branch(string nombre, string? latest_Commit)
        {          
            Nombre = nombre;
            Latest_Commit = latest_Commit;
        }
    }
}
