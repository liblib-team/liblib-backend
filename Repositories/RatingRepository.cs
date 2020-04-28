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

        bool PostRating(Rating rating);
    }

    public class RatingRepository : IRatingRepository
    {

        private MyLibContext DbContext;

        public RatingRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public List<Rating> ListRatingByBookId(Guid bookId)
        {
            return DbContext.Rating.Where(x => x.BookId == bookId).ToList();
        }

        public bool PostRating(Rating rating)
        {
            try
            {
                DbContext.Rating.Add(rating);
                DbContext.SaveChanges();
                return true;
            } 
            catch (Exception)
            {
                return false;
            }
        }
    }
}
