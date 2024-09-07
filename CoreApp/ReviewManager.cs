using DataAcces.CRUD;
using DTOs;

namespace CoreApp
{
    public class ReviewManager
    {

        public void Create(Review review)
        {
            var rev = new ReviewCrudFactory();
            rev.Create(review);
        }

        public void Update(Review review)
        {
            var rev = new ReviewCrudFactory();
            rev.Update(review);
        }

        public void Delete(Review review)
        {
            var rev = new ReviewCrudFactory();
            rev.Delete(review);
        }

        public List<Review> RetrieveAll()
        {
            var rev = new ReviewCrudFactory();
            return rev.RetrieveAll<Review>();
        }

        public Review RetrieveById(int id)
        {
            var rev = new ReviewCrudFactory();
            return rev.Retrieve<Review>(new Review { Id = id });
        }

        public List<Review> RetrieveByUserId(int usuarioId)
        {
            var rev = new ReviewCrudFactory();
            return rev.RetrieveByUserID<Review>(usuarioId);
        }


        public List<Review> RetrieveByContentId(int contenidoId)
        {
            var rev = new ReviewCrudFactory();
            return rev.RetrieveByContentID<Review>(contenidoId);
        }




    }
}
