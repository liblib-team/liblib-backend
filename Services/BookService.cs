using liblib_backend.Common;
using liblib_backend.Controllers.BookController;
using liblib_backend.Controllers.SubjectController;
using liblib_backend.Models;
using liblib_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace liblib_backend.Services
{
    public interface IBookService : ITransientService
    {
        List<BookDTO> ListBooksOrderByRating();
        List<BookDTO> ListBooksOrderByPopular();
        List<BookDTO> ListRelevanceBooks(Guid bookId);
        List<BookDTO> ListBooksWithSameAuthor(Guid bookId);
        List<BookDTO> ListBooksWithSameSubjectId(Guid subjectId);
        BookDetailDTO GetBookDetail(Guid bookId);
        PhysicalFileResult GetEbook(string role, Guid bookId);
    }

    public class BookService : IBookService
    {

        private IBookRepository bookRepository;
        private IAuthorRepository authorRepository;
        private ISubjectRepository subjectRepository;
        private IPublisherRepository publisherRepository;

        public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, ISubjectRepository subjectRepository, IPublisherRepository publisherRepository)
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.subjectRepository = subjectRepository;
            this.publisherRepository = publisherRepository;
        }

        public List<BookDTO> ListBooksOrderByPopular()
        {
            return bookRepository.ListBooksOrderByPopular().Select(x => new BookDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Image = x.Image,
                Description = x.Description,
                Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Image = y.Image
                }).ToList()
            }).ToList();
        }

        public List<BookDTO> ListBooksOrderByRating()
        {
            return bookRepository.ListBooksOrderByRating().Select(x => new BookDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Image = x.Image,
                Description = x.Description,
                Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Image = y.Image
                }).ToList()
            }).ToList();
        }

        public List<BookDTO> ListRelevanceBooks(Guid bookId)
        {
            List<Subject> subjects = subjectRepository.ListSubjectsByBookId(bookId);
            Dictionary<Guid, BookDTO> dictionary = new Dictionary<Guid, BookDTO>();
            if (subjects.Count != 0)
            {
                Subject subject = subjects[new Random().Next(subjects.Count)];
                dictionary = bookRepository.ListBooksWithSameSubjectId(subject.Id).Select(x => new BookDTO()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Image = x.Image,
                    Title = x.Title,
                    Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Image = y.Image
                    }).ToList()
                }).ToDictionary(x => x.Id);
            }
            List<Author> authors = authorRepository.ListAuthorsByBookId(bookId);
            if (authors.Count != 0)
            {
                Author author = authors[new Random().Next(authors.Count)];
                IEnumerable<BookDTO> temp = bookRepository.ListBooksWithSameAuthorId(author.Id).Select(x => new BookDTO()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Image = x.Image,
                    Title = x.Title,
                    Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                    {
                        Id = y.Id,
                        Name = y.Name,
                        Image = y.Image
                    }).ToList()
                });
                foreach (BookDTO book in temp)
                {
                    dictionary[book.Id] = book;
                }
            }
            return dictionary.Values.ToList();
        }

        public List<BookDTO> ListBooksWithSameAuthor(Guid bookId)
        {
            List<Author> authors = authorRepository.ListAuthorsByBookId(bookId);
            List<Book> result = new List<Book>();
            foreach (Author author in authors)
            {
                result.AddRange(bookRepository.ListBooksWithSameAuthorId(author.Id));
            }
            return result.Select(x => new BookDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Image = x.Image,
                Description = x.Description,
                Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Image = y.Image
                }).ToList()
            }).ToList();
        }

        public List<BookDTO> ListBooksWithSameSubjectId(Guid subjectId)
        {
            return bookRepository.ListBooksWithSameSubjectId(subjectId).Select(x => new BookDTO()
            {
                Id = x.Id,
                Title = x.Title,
                Image = x.Image,
                Description = x.Description,
                Authors = authorRepository.ListAuthorsByBookId(x.Id).Select(y => new AuthorDTO()
                {
                    Id = y.Id,
                    Name = y.Name,
                    Image = y.Image
                }).ToList()
            }).ToList();
        }

        public BookDetailDTO GetBookDetail(Guid bookId)
        {
            Book book = bookRepository.GetBookById(bookId);
            if (book == null)
            {
                return null;
            }
            book.Views++;
            bookRepository.UpdateBook(book);
            return new BookDetailDTO()
            {
                Id = book.Id,
                Authors = authorRepository.ListAuthorsByBookId(book.Id).Select(x => new AuthorDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image
                }).ToList(),
                Language = book.Language,
                Subjects = subjectRepository.ListSubjectsByBookId(book.Id).Select(x => new SubjectDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList(),
                Description = book.Description,
                Image = book.Image,
                Point = book.NumberOfRating == 0 ? 0 : book.Point / book.NumberOfRating,
                Title = book.Title,
                Publisher = publisherRepository.GetPublisherById(book.PublisherId)?.Name
            };
        }

        public PhysicalFileResult GetEbook(string role, Guid bookId)
        {
            Ebook ebook = bookRepository.GetEbook(bookId);
            if (ebook == null)
            {
                throw new FileNotFoundException();
            }
            if (!ebook.IsPublic)
            {
                if (string.IsNullOrWhiteSpace(role) || role.Equals("Member"))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Ebook", ebook.Location);
            return new PhysicalFileResult(path, "application/pdf");
        }
    }
}
