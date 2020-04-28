using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Hardbook
    {
        public Hardbook()
        {
            BookLending = new HashSet<BookLending>();
        }

        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public bool IsReferenceOnly { get; set; }
        public string Barcode { get; set; }
        public bool CanBorrowed { get; set; }

        public virtual Book Book { get; set; }
        public virtual ICollection<BookLending> BookLending { get; set; }
    }
}
