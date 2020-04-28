using System;
using System.Collections.Generic;

namespace MyLib.Models
{
    public partial class BookSubject
    {
        public Guid BookId { get; set; }
        public Guid SubjectId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
