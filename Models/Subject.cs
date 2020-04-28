using System;
using System.Collections.Generic;

namespace MyLib.Models
{
    public partial class Subject
    {
        public Subject()
        {
            BookSubject = new HashSet<BookSubject>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BookSubject> BookSubject { get; set; }
    }
}
