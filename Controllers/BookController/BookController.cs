using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using liblib_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace liblib_backend.Controllers.BookController
{
    [Route("api/[controller]/")]
    public class BookController : ControllerBase
    {

        private IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [Route("list/rating")]
        [HttpGet]
        public List<BookDTO> ListRatingBooks()
        {
            return bookService.ListBooksOrderByRating();
        }

        [Route("list/popular")]
        [HttpGet]
        public List<BookDTO> ListPopularBooks()
        {
            return bookService.ListBooksOrderByPopular();
        }

        [Route("list/relevance/{bookId}")]
        [HttpGet]
        public List<BookDTO> ListRelevanceBooks(Guid bookId)
        {
            return bookService.ListRelevanceBooks(bookId);
        }

        [Route("list/author/{authorId}")]
        [HttpGet]
        public List<BookDTO> ListAuthorBooks(Guid authorId)
        {
            return bookService.ListBooksWithSameAuthorId(authorId);
        }

        [Route("list/subject/{subjectId}")]
        [HttpGet]
        public List<BookDTO> ListSubjectBooks(Guid subjectId)
        {
            return bookService.ListBooksWithSameSubjectId(subjectId);
        }
        
        [Route("detail/{bookId}")]
        [HttpGet]
        public BookDetailDTO GetBookDetail(Guid bookId)
        {
            return bookService.GetBookDetail(bookId);
        }

    }
}
