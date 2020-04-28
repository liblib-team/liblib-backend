using System;
using System.Collections.Generic;

namespace liblib_backend.Models
{
    public partial class Account
    {
        public Account()
        {
            BookLending = new HashSet<BookLending>();
            BookReservation = new HashSet<BookReservation>();
            Log = new HashSet<Log>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int DateCreated { get; set; }
        public int? DateModified { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BookLending> BookLending { get; set; }
        public virtual ICollection<BookReservation> BookReservation { get; set; }
        public virtual ICollection<Log> Log { get; set; }
    }
}
