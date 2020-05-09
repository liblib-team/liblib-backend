using liblib_backend.Common;
using liblib_backend.Controllers;
using liblib_backend.Controllers.ReservationController;
using liblib_backend.Models;
using liblib_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Services
{
    public interface IReservationService : ITransientService
    {
        ResultDTO CreateReservation(Guid accountId, Guid bookId);
        List<ReservationDTO> ListReservations(Guid accountId);
        ResultDTO CancelReservation(Guid accountId, Guid reservationId);
    }

    public class ReservationService : IReservationService
    {

        private IReservationRepository reservationRepository;
        private ILendingRepository lendingRepository;
        private IBookRepository bookRepository;
        private IUserRepository userRepository;

        private ILendingService lendingService;

        public ReservationService(IReservationRepository reservationRepository, ILendingRepository lendingRepository, IBookRepository bookRepository, IUserRepository userRepository, ILendingService lendingService)
        {
            this.reservationRepository = reservationRepository;
            this.lendingRepository = lendingRepository;
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.lendingService = lendingService;
        }

        public List<ReservationDTO> ListReservations(Guid accountId)
        {
            List<BookReservation> reservations = reservationRepository.ListReservationsByAccountId(accountId);
            foreach (BookReservation reservation in reservations)
            {
                UpdateReservation(reservation.BookId);
            }

            reservations = reservationRepository.ListReservationsByAccountId(accountId);
            List<ReservationDTO> result = new List<ReservationDTO>();
            foreach (BookReservation reservation in reservations)
            {
                Book book = bookRepository.GetBookById(reservation.BookId);
                result.Add(new ReservationDTO()
                {
                    Id = reservation.Id,
                    BookId = reservation.BookId,
                    ReservationDate = reservation.ReservationDate,
                    Image = book.Image,
                    Status = reservation.Status,
                    Title = book.Title
                });
            }
           
            return result;
        }

        public ResultDTO CreateReservation(Guid accountId, Guid bookId)
        {
            UpdateReservation(bookId);
            BookReservation reservation = reservationRepository.GetReservationWithStatus(accountId, bookId, "Pending", "Accepted", "Borrowing", "Overdue");
            if (reservation != null)
            {
                return new ResultDTO()
                {
                    Success = false,
                    Message = "Đã có yêu cầu đặt sách này"
                };
            }

            reservation = new BookReservation()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                BookId = bookId,
                ReservationDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            if (reservationRepository.CountReservationWithStatus(bookId, "Accepted") < bookRepository.CountAvalableHardbook(bookId))
            {
                reservation.Status = "Accepted";
            }
            else
            {
                reservation.Status = "Pending";
            }

            if (reservationRepository.AddReservation(reservation))
            {
                // Auto accept
                if (reservation.Status.Equals("Accepted"))
                {
                    return lendingService.AssignHardbook(accountId, bookRepository.ListAvailableHardbooks(bookId).FirstOrDefault().Barcode);
                }

                return new ResultDTO
                {
                    Success = true,
                    Message = "Đặt sách thành công"
                };
            } 
            else
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Lỗi hệ thống"
                };
            }
        }

        private void UpdateReservation(Guid bookId)
        {
            int limit = 24 * 60 * 60;
            List<BookReservation> acceptedReservations = reservationRepository.ListReservationsByBookIdWithStatus(bookId, "Accepted");
            List<BookReservation> pendingReservations = reservationRepository.ListReservationsByBookIdWithStatus(bookId, "Pending");
            foreach (BookReservation reservation in acceptedReservations)
            {
                if (reservation.ReservationDate + limit > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    reservation.Status = "Rejected";
                    reservationRepository.UpdateReservations(reservation);
                    if (pendingReservations.Count != 0)
                    {
                        pendingReservations[0].Status = "Accepted";
                        reservationRepository.UpdateReservations(pendingReservations[0]);
                        pendingReservations.RemoveAt(0);
                    }
                }
            }

            List<BookReservation> borrowingReservations = reservationRepository.ListReservationsByBookIdWithStatus(bookId, "Borrowing");
            foreach (BookReservation reservation in borrowingReservations)
            {
                if (lendingRepository.GetLendingByReservationId(reservation.Id).DueDate < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    reservation.Status = "Overdue";
                    reservationRepository.UpdateReservations(reservation);
                }
            }
        }

        public ResultDTO CancelReservation(Guid accountId, Guid reservationId)
        {
            Account account = userRepository.GetAccountWithId(accountId);
            BookReservation reservation = reservationRepository.GetReservationWithId(reservationId);
            if (account.Role.Equals("Member"))
            {
                if (reservation.AccountId != accountId)
                {
                    return new ResultDTO()
                    {
                        Success = false,
                        Message = "Không thể hủy yêu cầu của người khác"
                    };
                }
                if (reservation.Status.Equals("Pending"))
                {
                    reservation.Status = "Cancelled";
                    reservationRepository.UpdateReservations(reservation);
                    return new ResultDTO()
                    {
                        Success = true,
                        Message = ""
                    };
                }
                if (reservation.Status.Equals("Accepted"))
                {
                    reservation.Status = "Cancelled";
                    reservationRepository.UpdateReservations(reservation);
                    UpdateReservation(reservation.BookId);
                    return new ResultDTO()
                    {
                        Success = true,
                        Message = ""
                    };
                }
                return new ResultDTO()
                {
                    Success = false,
                    Message = "Không thể hủy yêu cầu đã nhận"
                };
            }

            if (reservation.Status.Equals("Pending") || reservation.Status.Equals("Accepted"))
            {
                reservation.Status = "Rejected";
                reservationRepository.UpdateReservations(reservation);
                UpdateReservation(reservation.BookId);
                return new ResultDTO()
                {
                    Success = true,
                    Message = ""
                };
            }
            else
            {
                return new ResultDTO()
                {
                    Success = false,
                    Message = "Không thể hủy yêu cầu đã nhận"
                };
            }            
        }
    }
}
