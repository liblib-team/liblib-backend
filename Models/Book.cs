using System;
using System.Collections.Generic;

namespace MyLib.Models
{
    public partial class Book
    {
        public Book()
        {
            BookAuthor = new HashSet<BookAuthor>();
            BookPublisher = new HashSet<BookPublisher>();
            BookReservation = new HashSet<BookReservation>();
            BookSubject = new HashSet<BookSubject>();
            Hardbook = new HashSet<Hardbook>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public int NumberOfReservation { get; set; }

        public virtual Ebook Ebook { get; set; }
        public virtual ICollection<BookAuthor> BookAuthor { get; set; }
        public virtual ICollection<BookPublisher> BookPublisher { get; set; }
        public virtual ICollection<BookReservation> BookReservation { get; set; }
        public virtual ICollection<BookSubject> BookSubject { get; set; }
        public virtual ICollection<Hardbook> Hardbook { get; set; }
    }
}
