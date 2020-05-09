using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class BookReservation
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public int ReservationDate { get; set; }
        public string Status { get; set; }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Book Book { get; set; }
        public virtual BookLending IdNavigation { get; set; }
    }
}
