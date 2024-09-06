using MongoDB.Bson.Serialization.Attributes;

namespace MiniGithub.Models
{
    public class Repositorio
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]    it causes error of serialization, i.e  asigna un hash en ves de una string como id.
        public string? Id { get; set; }

        public string Nombre { get; set; }

        public string Tags { get; set; }

        public int UserId { get; set; }

        public string Visibilidad { get; set; }

        //faltan branches
        //tags tiene que ser array
        //en general todo esta mal y hay que cambiarlo despues
    }
}
