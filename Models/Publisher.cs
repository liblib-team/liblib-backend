using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Publisher
    {
        public Publisher()
        {
            BookPublisher = new HashSet<BookPublisher>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BookPublisher> BookPublisher { get; set; }
    }
}
