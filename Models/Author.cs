﻿using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Author
    {
        public Author()
        {
            BookAuthor = new HashSet<BookAuthor>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual ICollection<BookAuthor> BookAuthor { get; set; }
    }
}
