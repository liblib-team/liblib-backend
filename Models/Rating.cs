using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Rating
    {
        public Guid AccountId { get; set; }
        public Guid BookId { get; set; }
        public double Point { get; set; }
        public string Comment { get; set; }

        public virtual Account Account { get; set; }
        public virtual Book Book { get; set; }
    }
}
