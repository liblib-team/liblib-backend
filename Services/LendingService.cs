using liblib_backend.Common;
using liblib_backend.Controllers;
using liblib_backend.Controllers.LendingController;
using liblib_backend.Models;
using liblib_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Services
{
    public interface ILendingService : ITransientService
    {
        ResultDTO AssignHardbook(Guid accountId, string barcode);
        List<LendingDTO> ListLendings(Guid accountId);
        ResultDTO CloseLending(string barcode);
    }

    public class LendingService : ILendingService
    {

        private ILendingRepository lendingRepository;
        private IReservationRepository reservationRepository;
        private IBookRepository bookRepository;

        public LendingService(ILendingRepository lendingRepository, IReservationRepository reservationRepository, IBookRepository bookRepository)
        {
            this.lendingRepository = lendingRepository;
            this.reservationRepository = reservationRepository;
            this.bookRepository = bookRepository;
        }

        public ResultDTO AssignHardbook(Guid accountId, string barcode)
        {
            Hardbook hardbook = bookRepository.GetHardbook(barcode);
            BookReservation reservation = reservationRepository.ListReservationsByAccountId(accountId).FirstOrDefault(x => x.BookId == hardbook.BookId);
            hardbook.CanBorrowed = false;
            reservation.Status = "Borrowing";
            reservationRepository.UpdateReservations(reservation);
            bookRepository.UpdateHardbook(hardbook);
            lendingRepository.AddLending(new BookLending()
            {
                Id = reservation.Id,
                HardbookId = hardbook.Id,
                BorrowedDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                DueDate = (int)DateTimeOffset.UtcNow.AddMonths(1).ToUnixTimeSeconds()
            });
            return new ResultDTO()
            {
                Success = true,
                Message = ""
            };
        }

        public ResultDTO CloseLending(string barcode)
        {
            int returnTime = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Hardbook hardbook = bookRepository.GetHardbook(barcode);
            BookLending lending = lendingRepository.GetLendingByHardbookId(hardbook.Id);
            BookReservation reservation = reservationRepository.GetReservationWithId(lending.Id);
            reservation.Status = "Returned";
            hardbook.CanBorrowed = true;
            lending.ReturnDate = returnTime;
            bookRepository.UpdateHardbook(hardbook);
            reservationRepository.UpdateReservations(reservation);
            lendingRepository.UpdateLending(lending);
            string message;
            if (lending.DueDate > returnTime)
            {
                message = "Trả đúng hạn";
            } 
            else
            {
                TimeSpan t = TimeSpan.FromSeconds(returnTime - lending.DueDate);
                message = "Trả muộn " + t.Days + " ngày, " + t.Hours + " giờ, " + t.Minutes + " phút";
            }

            return new ResultDTO()
            {
                Success = true,
                Message = message
            };
        }

        public List<LendingDTO> ListLendings(Guid accountId)
        {
            return lendingRepository.ListLendings(accountId).Select(x =>
            {
                BookReservation reservation = reservationRepository.GetReservationWithId(x.Id);
                Book book = bookRepository.GetBookById(reservation.BookId);
                return new LendingDTO()
                {
                    BorrowedDate = x.BorrowedDate,
                    DueDate = x.DueDate,
                    BookId = book.Id,
                    Image = book.Image,
                    Title = book.Title
                };
            }).ToList();
        }
    }
}
