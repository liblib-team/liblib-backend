using liblib_backend.Common;
using liblib_backend.Controllers.RatingController;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface IRatingRepository : ITransientService
    {
        List<Rating> ListRatingByBookId(Guid bookId);
        void AddRating(Rating rating);
        void UpdateRating(Rating rating);
        Rating GetRating(Guid accountId, Guid bookId);
    }

    public class RatingRepository : IRatingRepository
    {

        private MyLibContext DbContext;

        public RatingRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public void AddRating(Rating rating)
        {
            try
            {
                DbContext.Rating.Add(rating);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public Rating GetRating(Guid accountId, Guid bookId)
        {
            return DbContext.Rating.FirstOrDefault(x => x.BookId == bookId && x.AccountId == accountId);
        }

        public List<Rating> ListRatingByBookId(Guid bookId)
        {
            return DbContext.Rating.Where(x => x.BookId == bookId).ToList();
        }

        public void UpdateRating(Rating rating)
        {
            try
            {
                DbContext.Rating.Update(rating);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}
