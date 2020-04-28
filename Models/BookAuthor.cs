using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class BookAuthor
    {
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }

        public virtual Author Author { get; set; }
        public virtual Book Book { get; set; }
    }
}
