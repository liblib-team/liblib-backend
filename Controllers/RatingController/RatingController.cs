using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using liblib_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace liblib_backend.Controllers.RatingController
{
    [Route("api/[controller]/")]
    public class RatingController : ControllerBase
    {
        private IRatingService ratingService;

        public RatingController(IRatingService ratingService)
        {
            this.ratingService = ratingService;
        }

        [Route("list/{bookId}")]
        [HttpGet]
        public List<PostedRatingDTO> ListRatings(Guid bookId)
        {
            return ratingService.ListRatingsByBookId(bookId);
        }

        [Authorize]
        [Route("post")]
        [HttpPost]
        public void PostRating([FromBody] RatingDTO ratingDTO)
        {
            ratingService.PostRating(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), ratingDTO);
        }
    }
}
