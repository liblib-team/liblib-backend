using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Book
    {
        public Book()
        {
            BookAuthor = new HashSet<BookAuthor>();
            BookReservation = new HashSet<BookReservation>();
            BookSubject = new HashSet<BookSubject>();
            Hardbook = new HashSet<Hardbook>();
            Rating = new HashSet<Rating>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public int Views { get; set; }
        public int NumberOfRating { get; set; }
        public double Point { get; set; }
        public string Image { get; set; }
        public Guid? PublisherId { get; set; }

        public virtual Publisher Publisher { get; set; }
        public virtual Ebook Ebook { get; set; }
        public virtual ICollection<BookAuthor> BookAuthor { get; set; }
        public virtual ICollection<BookReservation> BookReservation { get; set; }
        public virtual ICollection<BookSubject> BookSubject { get; set; }
        public virtual ICollection<Hardbook> Hardbook { get; set; }
        public virtual ICollection<Rating> Rating { get; set; }
    }
}
