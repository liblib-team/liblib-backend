using liblib_backend.Common;
using liblib_backend.Models;
using liblib_backend.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface IBookRepository : ITransientService
    {
        List<Book> ListBooksOrderByRating();
        List<Book> ListBooksOrderByPopular();
        List<Book> ListBooksWithSameSubjectId(Guid subjectId);
        List<Book> ListBooksWithSameAuthorId(Guid authorId);
        List<Book> ListBooksContainName(string name);
        Book GetBookById(Guid bookId);
        void UpdateBook(Book book);
        Ebook GetEbook(Guid bookId);
        Hardbook GetHardbook(string barcode);
        void UpdateHardbook(Hardbook hardbook);
        List<Hardbook> ListAvailableHardbooks(Guid bookId);
        int CountAvalableHardbook(Guid bookId);
    }

    public class BookRepository : IBookRepository
    {

        private MyLibContext DbContext;

        public BookRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public List<Book> ListBooksOrderByPopular()
        {
            return DbContext.Book.OrderBy(book => book.Views).ToList();
        }

        public List<Book> ListBooksOrderByRating()
        {
            return DbContext.Book.OrderByDescending(book => book.NumberOfRating == 0 ? 0 : book.Point / book.NumberOfRating).ToList();
        }

        public List<Book> ListBooksWithSameSubjectId(Guid subjectId)
        {
            return DbContext.BookSubject
                .Where(x => x.SubjectId == subjectId)
                .Join(DbContext.Book, x1 => x1.BookId, x2 => x2.Id, (x1, x2) => x2)
                .ToList();
        }

        public List<Book> ListBooksWithSameAuthorId(Guid authorId)
        {
            return DbContext.BookAuthor
                .Where(x => x.AuthorId == authorId)
                .Join(DbContext.Book, x1 => x1.BookId, x2 => x2.Id, (x1, x2) => x2)
                .ToList();
        }

        public Book GetBookById(Guid bookId)
        {
            return DbContext.Book.FirstOrDefault(x => x.Id == bookId);
        }

        public void UpdateBook(Book book)
        {
            try
            {
                DbContext.Book.Update(book);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public Ebook GetEbook(Guid bookId)
        {
            return DbContext.Ebook.FirstOrDefault(x => x.BookId == bookId);
        }

        public List<Hardbook> ListAvailableHardbooks(Guid bookId)
        {
            return DbContext.Hardbook.Where(x => x.BookId == bookId && x.CanBorrowed).ToList();
        }

        public int CountAvalableHardbook(Guid bookId)
        {
            return DbContext.Hardbook.Count(x => x.BookId == bookId && x.CanBorrowed);
        }

        public Hardbook GetHardbook(string barcode)
        {
            return DbContext.Hardbook.FirstOrDefault(x => x.Barcode.Equals(barcode));
        }

        public void UpdateHardbook(Hardbook hardbook)
        {
            try
            {
                DbContext.Hardbook.Update(hardbook);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        public List<Book> ListBooksContainName(string name)
        {
            return DbContext.Book.Where(x => Utility.Contains(x.Title, name)).ToList();
        }
    }
}
