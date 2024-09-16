using Microsoft.Build.Framework;

namespace ApiWeb.Models
{
    public class Branch
    {
        [Required]
        public string Name { get; set; }        //Unique
        [Required]
        public string? LatestCommit { get; set; }

        public Branch(string name, string? latestCommit)
        {
            Name = name;
            LatestCommit = latestCommit;
        }
    }
}
