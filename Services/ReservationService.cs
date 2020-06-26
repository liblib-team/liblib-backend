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
        List<ReservationDTO> ListReservationsWithStatus(Guid accountId, params string[] status);
        ResultDTO CancelReservation(Guid accountId, Guid reservationId);
    }

    public class ReservationService : IReservationService
    {

        private IReservationRepository reservationRepository;
        private ILendingRepository lendingRepository;
        private IBookRepository bookRepository;
        private IAccountRepository userRepository;

        private ILendingService lendingService;

        public ReservationService(IReservationRepository reservationRepository, ILendingRepository lendingRepository, IBookRepository bookRepository, IAccountRepository userRepository, ILendingService lendingService)
        {
            this.reservationRepository = reservationRepository;
            this.lendingRepository = lendingRepository;
            this.bookRepository = bookRepository;
            this.userRepository = userRepository;
            this.lendingService = lendingService;
        }

        public List<ReservationDTO> ListReservations(Guid accountId)
        {
            List<BookReservation> reservations = reservationRepository.ListReservationsByAccountIdWithStatus(accountId, "Pending");
            foreach (BookReservation reservation in reservations)
            {
                UpdateReservation(reservation.BookId);
            }

            reservations = reservationRepository.ListReservationsByAccountIdWithStatus(accountId, "Pending", "Rejected", "Cancelled");
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
                    Status = Utility.TranslateStatusToVI(reservation.Status),
                    Title = book.Title
                });
            }

            return result.OrderBy(x => x.ReservationDate).ToList();
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

            if (reservationRepository.CreateReservation(reservation))
            {
                // Auto accept
                if (reservation.Status.Equals("Accepted"))
                {
                    return lendingService.AssignHardbook(accountId, bookRepository.ListAvailableHardbooks(bookId).FirstOrDefault().Barcode);
                }

                return new ResultDTO()
                {
                    Success = true,
                    Message = "Đặt sách thành công"
                };
            } 
            else
            {
                return new ResultDTO()
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
            foreach (BookReservation reservation in acceptedReservations)
            {
                if (reservation.ReservationDate + limit > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    reservation.Status = "Rejected";
                    reservationRepository.UpdateReservations(reservation);
                }
            }

            List<BookReservation> pendingReservations = reservationRepository.ListReservationsByBookIdWithStatus(bookId, "Pending");
            int availableHardbook = bookRepository.CountAvalableHardbook(bookId);
            while (availableHardbook-- > 0 && pendingReservations.Count != 0)
            {
                pendingReservations[0].Status = "Accepted";
                lendingService.AssignHardbook(pendingReservations[0].AccountId, bookRepository.ListAvailableHardbooks(bookId).FirstOrDefault().Barcode);
                reservationRepository.UpdateReservations(pendingReservations[0]);
                pendingReservations.RemoveAt(0);
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

        public List<ReservationDTO> ListReservationsWithStatus(Guid accountId, params string[] status)
        {
            List<BookReservation> reservations = reservationRepository.ListReservationsByAccountIdWithStatus(accountId, "Pending");
            foreach (BookReservation reservation in reservations)
            {
                UpdateReservation(reservation.BookId);
            }

            reservations = reservationRepository.ListReservationsByAccountIdWithStatus(accountId, status);
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
                    Status = Utility.TranslateStatusToVI(reservation.Status),
                    Title = book.Title
                });
            }

            return result.OrderBy(x => x.ReservationDate).ToList();
        }
    }
}
