using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface IAuthorRepository : ITransientService
    {
        List<Author> ListAuthorsByBookId(Guid bookId);

    }

    public class AuthorRepository : IAuthorRepository
    {
        private MyLibContext DbContext;

        public AuthorRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public List<Author> ListAuthorsByBookId(Guid bookId)
        {
            return DbContext.BookAuthor
                .Where(x => x.BookId == bookId)
                .Join(DbContext.Author, x1 => x1.AuthorId, x2 => x2.Id, (x1, x2) => x2)
                .ToList();
        }
    }
}
