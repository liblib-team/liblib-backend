using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using liblib_backend.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Route("list/author/{bookId}")]
        [HttpGet]
        public List<BookDTO> ListAuthorBooks(Guid bookId)
        {
            return bookService.ListBooksWithSameAuthor(bookId);
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

        [Authorize]
        [AllowAnonymous]
        [Route("read/{bookId}")]
        [HttpGet]
        public PhysicalFileResult ReadEbook(Guid bookId)
        {
            string role = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return bookService.GetEbook(role, bookId);
        }

    }
}
