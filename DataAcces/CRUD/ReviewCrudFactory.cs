using DataAccess.CRUD;
using DataAccess.DAOs;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcces.CRUD
{
    public class ReviewCrudFactory : CrudFactory
    {
        
        public ReviewCrudFactory()
        {
            _dao = SqlDao.GetInstance();
        }


        public override void Create(BaseDTO baseDTO)
        {
            var review = (Review)baseDTO;
            var sqloperation = new SqlOperation()
            {
                ProcedureName = "sp_CreateReview"
            };

            sqloperation.AddIntParameter("Rating", review.Rating);
            sqloperation.AddIntParameter("UserId", review.UsuarioId);  // Cambio de 'usuarioId'
            sqloperation.AddIntParameter("ContentId", review.ContenidoId);  // Cambio de 'contenidoId'
            sqloperation.AddVarCharParameter("Texto", review.Texto);

            _dao.ExecuteProcedure(sqloperation);  // Ejecutar procedimiento
        }

        public override void Delete(BaseDTO baseDTO)
        {
            var review = baseDTO as Review;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_DeleteReview"
            };

            sqlOperation.AddIntParameter("Id", review.Id);
            _dao.ExecuteProcedure(sqlOperation);

        }

        public override T Retrieve<T>(BaseDTO baseDTO)
        {
            throw new NotImplementedException();
        }

        public override List<T> RetrieveAll<T>()
        {
            var listaReview = new List<T>();
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetAllReviews"
            };
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if(data.Count > 0)
            {
                foreach(var row in data)
                {
                    var review = BuildObject(row);
                    listaReview.Add((T)Convert.ChangeType(review, typeof(T)));
                }
            }
            return listaReview;
        }

        public override T RetrieveByID<T>(int id)
        {
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetReviewById"
            };
            sqlOperation.AddIntParameter("Id", id);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            if(data.Count > 0)
            {
                var row = data[0];
                var review = BuildObject(row);
                return (T)Convert.ChangeType(review, typeof(T));
            }
            return default(T);
        }

        public List<T> RetrieveByUserID<T>(int UsuarioId)
        {
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetReviewByUsuarioId"
            };
            sqlOperation.AddIntParameter("UsuarioId", UsuarioId);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            var listaReviews = new List<T>();

            if (data.Count > 0)
            {
                foreach (var row in data)
                {
                    var review = BuildObject(row);
                    listaReviews.Add((T)Convert.ChangeType(review, typeof(T)));
                }
            }
            return listaReviews;
        }


        public List<T> RetrieveByContentID<T>(int ContenidoId)
        {
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_GetReviewByContentId"
            };
            sqlOperation.AddIntParameter("ContentId", ContenidoId);
            var data = _dao.ExecuteQueryProcedure(sqlOperation);

            var listaReviews = new List<T>();

            if (data.Count > 0)
            {
                foreach (var row in data)
                {
                    var review = BuildObject(row);
                    listaReviews.Add((T)Convert.ChangeType(review, typeof(T)));
                }
            }
            return listaReviews;
        }



        public override void Update(BaseDTO baseDTO)
        {
            var review = baseDTO as Review;
            var sqlOperation = new SqlOperation()
            {
                ProcedureName = "sp_UpdateReview"
            };

            sqlOperation.AddIntParameter("Id", review.Id);
            sqlOperation.AddIntParameter("Rating", review.Rating);
            sqlOperation.AddIntParameter("UserId", review.UsuarioId);
            sqlOperation.AddIntParameter("ContentId", review.ContenidoId);
            sqlOperation.AddVarCharParameter("Texto", review.Texto);
            _dao.ExecuteProcedure(sqlOperation);
        }

        private Review BuildObject(Dictionary<string, object> row)
        {
            var review = new Review
            {
                Id = (int)row["Id"],
                Rating = (int)row["Rating"],
                UsuarioId = (int)row["UserId"],
                ContenidoId = (int)row["ContentId"],
                Texto = (string)row["Texto"]

            };

            return review;
        }

    }
    
    }

