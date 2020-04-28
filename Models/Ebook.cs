using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Ebook
    {
        public Guid BookId { get; set; }
        public bool IsPublic { get; set; }
        public string Location { get; set; }

        public virtual Book Book { get; set; }
    }
}
