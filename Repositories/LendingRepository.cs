using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface ILendingRepository : ITransientService
    {
        BookLending GetLendingByReservationId(Guid reservationId);
        BookLending GetLendingByHardbookId(Guid hardbookId);
        bool AddLending(BookLending lending);
        List<BookLending> ListLendings(Guid accountId);
        void UpdateLending(BookLending lending);
    }

    public class LendingRepository : ILendingRepository
    {
        private MyLibContext DbContext;

        public LendingRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public bool AddLending(BookLending lending)
        {
            try
            {
                DbContext.BookLending.Add(lending);
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public BookLending GetLendingByHardbookId(Guid hardbookId)
        {
            return DbContext.BookLending.FirstOrDefault(x => x.HardbookId == hardbookId && x.ReturnDate == null);
        }

        public BookLending GetLendingByReservationId(Guid reservationId)
        {
            return DbContext.BookLending.FirstOrDefault(x => x.Id == reservationId);
        }

        public List<BookLending> ListLendings(Guid accountId)
        {
            return DbContext.BookReservation.Where(x => x.AccountId == accountId)
                            .Join(DbContext.BookLending, x1 => x1.Id, x2 => x2.Id, (x1, x2) => x2)
                            .OrderBy(x => x.BorrowedDate)
                            .ToList();
        }

        public void UpdateLending(BookLending lending)
        {
            try
            {
                DbContext.BookLending.Update(lending);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}
