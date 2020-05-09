using System;

namespace liblib_backend.Controllers.ReservationController
{
    public class ReservationDTO
    {
        public Guid Id;

        public int ReservationDate;

        public string Status;

        public Guid BookId;

        public string Title;

        public string Image;
    }
}