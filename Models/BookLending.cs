using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class BookLending
    {
        public Guid Id { get; set; }
        public Guid HardbookId { get; set; }
        public int Borrowed { get; set; }
        public int DueDate { get; set; }
        public int? ReturnDate { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Hardbook Hardbook { get; set; }
    }
}
