using liblib_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace liblib_backend.Controllers.ReservationController
{
    [Route("api/[controller]/")]
    public class ReservationController : ControllerBase
    {

        private IReservationService reservationService;

        public ReservationController(IReservationService reservationService)
        {
            this.reservationService = reservationService;
        }

        [Authorize]
        [Route("create/{bookId}")]
        [HttpPost]
        public ResultDTO CreateReservation(Guid bookId)
        {
            return reservationService.CreateReservation(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), bookId);
        }

        [Authorize]
        [Route("list")]
        [HttpGet]
        public List<ReservationDTO> ListReservations()
        {
            return reservationService.ListReservations(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [Authorize(Policy = "MemberNotAllowed")]
        [Route("list/{accountId}")]
        [HttpGet]
        public List<ReservationDTO> ListReservations(Guid accountId)
        {
            return reservationService.ListReservations(accountId);
        }

        [Authorize]
        [Route("list/{status}")]
        [HttpGet]
        public List<ReservationDTO> ListReservations(string status)
        {
            return reservationService.ListReservationsWithStatus(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), status.Split(','));
        }

        [Authorize]
        [Route("cancel/{ReservationId}")]
        [HttpPost]
        public ResultDTO CancelReservation(Guid reservationId) 
        {
            return reservationService.CancelReservation(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), reservationId);
        }

    }

}
