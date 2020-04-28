using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class BookPublisher
    {
        public Guid BookId { get; set; }
        public Guid PublisherId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Publisher Publisher { get; set; }
    }
}
