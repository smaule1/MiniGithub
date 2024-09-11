using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class Commit : BaseDTO
    {
        public string Key_id { get; set; }
        public int Version { get; set; }

    }
}