using MongoDB.Bson.Serialization.Attributes;

namespace ApiWeb.Models
{
    public class Repositorio
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]    it causes error of serialization, i.e  asigna un hash en ves de una string como id.               
        public string? Id { get; set; } //TODO: the id must be composed by: userId-nombre       

        
        public string Nombre { get; set; }

        public string Tags { get; set; }

        public int UserId { get; set; }

        public string Visibilidad { get; set; }

        //faltan branches
        //tags tiene que ser array
        //en general todo esta mal y hay que cambiarlo despues
    }
}
