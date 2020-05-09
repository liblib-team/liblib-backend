using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface IReservationRepository : ITransientService
    {
        BookReservation GetReservationWithId(Guid reservationId);
        BookReservation GetReservationWithStatus(Guid accountId, Guid bookId, params string[] status);
        List<BookReservation> ListReservationsByBookIdWithStatus(Guid bookId, params string[] status);
        List<BookReservation> ListReservationsByAccountId(Guid accountId);
        List<BookReservation> ListReservationsByAccountIdWithStatus(Guid accountId, params string[] status);
        bool AddReservation(BookReservation reservation);
        void UpdateReservations(params BookReservation[] reservation);
        int CountReservationWithStatus(Guid bookId, string status);
    }

    public class ReservationRepository : IReservationRepository
    {
        private MyLibContext DbContext;

        public ReservationRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public bool AddReservation(BookReservation reservation)
        {
            try
            {
                DbContext.BookReservation.Add(reservation);
                DbContext.SaveChanges();
                return true;
            } 
            catch (Exception)
            {
                return false;
            }
        }

        public int CountReservationWithStatus(Guid bookId, string status)
        {
            return DbContext.BookReservation.Count(x => x.BookId == bookId && x.Status.Equals(status));
        }

        public BookReservation GetReservationWithId(Guid reservationId)
        {
            return DbContext.BookReservation.FirstOrDefault(x => x.Id == reservationId);
        }

        public BookReservation GetReservationWithStatus(Guid accountId, Guid bookId, params string[] status)
        {
            return DbContext.BookReservation.FirstOrDefault(x => x.AccountId == accountId && x.BookId == bookId && status.Contains(x.Status));
        }

        public List<BookReservation> ListReservationsByAccountId(Guid accountId)
        {
            return DbContext.BookReservation.Where(x => x.AccountId == accountId).OrderBy(x => x.ReservationDate).ToList();
        }

        public List<BookReservation> ListReservationsByAccountIdWithStatus(Guid accountId, params string[] status)
        {
            return DbContext.BookReservation.Where(x => x.AccountId == accountId && status.Contains(x.Status)).ToList();
        }

        public List<BookReservation> ListReservationsByBookIdWithStatus(Guid bookId, params string[] status)
        {
            return DbContext.BookReservation.Where(x => x.BookId == bookId && status.Contains(x.Status)).OrderBy(x => x.ReservationDate).ToList();
        }

        public void UpdateReservations(params BookReservation[] reservation)
        {
            try
            {
                DbContext.BookReservation.UpdateRange(reservation);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}
