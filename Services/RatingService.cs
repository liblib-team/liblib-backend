using liblib_backend.Common;
using liblib_backend.Controllers.RatingController;
using liblib_backend.Models;
using liblib_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Services
{
    public interface IRatingService : ITransientService
    {
        List<PostedRatingDTO> ListRatingsByBookId(Guid bookId);
        void PostRating(Guid userId, RatingDTO ratingDTO);
    }

    public class RatingService : IRatingService
    {

        private IRatingRepository ratingRepository;
        private IUserRepository userRepository;
        private IBookRepository bookRepository;

        public RatingService(IRatingRepository ratingRepository, IUserRepository userRepository, IBookRepository bookRepository)
        {
            this.ratingRepository = ratingRepository;
            this.userRepository = userRepository;
            this.bookRepository = bookRepository;
        }


        public List<PostedRatingDTO> ListRatingsByBookId(Guid bookId)
        {
            return ratingRepository.ListRatingByBookId(bookId).Select(x => 
            {
                Account account = userRepository.GetAccountWithId(x.AccountId);
                return new PostedRatingDTO()
                {
                    Name = account.Username,
                    Point = x.Point,
                    Comment = x.Comment,
                    Image = account.Image
                };
            }).ToList();
        }

        public void PostRating(Guid userId, RatingDTO ratingDTO)
        {
            if (ratingDTO == null || ratingDTO.BookId == null)
            {
                return;
            }

            bool result = ratingRepository.PostRating(new Rating()
            {
                AccountId = userId,
                BookId = ratingDTO.BookId,
                Point = ratingDTO.Point,
                Comment = ratingDTO.Comment
            });

            if (result)
            {
                Book book = bookRepository.GetBookById(ratingDTO.BookId);
                book.NumberOfRating++;
                book.Point += ratingDTO.Point;
                bookRepository.UpdateBook(book);
            }
        }
    }
}
