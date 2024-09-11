using DataAcces.CRUD;
using DTOs;

namespace CoreApp
{
    public class CommitManager
    {

        public void Create(Review review)
        {
            var rev = new CommitCrudFactory();
            rev.Create(review);
        }

        public void Update(Review review)
        {
            var rev = new CommitCrudFactory();
            rev.Update(review);
        }

        public void Delete(Review review)
        {
            var rev = new CommitCrudFactory();
            rev.Delete(review);
        }

        public List<Review> RetrieveAll()
        {
            var rev = new CommitCrudFactory();
            return rev.RetrieveAll<Review>();
        }

        public Review RetrieveById(int id)
        {
            var rev = new CommitCrudFactory();
            return rev.Retrieve<Review>(new Review { Id = id });
        }

        public List<Review> RetrieveByUserId(int usuarioId)
        {
            var rev = new CommitCrudFactory();
            return rev.RetrieveByUserID<Review>(usuarioId);
        }


        public List<Review> RetrieveByContentId(int contenidoId)
        {
            var rev = new CommitCrudFactory();
            return rev.RetrieveByContentID<Review>(contenidoId);
        }




    }
}
