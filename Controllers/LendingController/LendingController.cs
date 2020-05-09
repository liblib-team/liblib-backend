using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using liblib_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace liblib_backend.Controllers.LendingController
{
    [Route("api/[controller]/")]
    public class LendingController : ControllerBase
    {
        private ILendingService lendingService;

        public LendingController(ILendingService lendingService)
        {
            this.lendingService = lendingService;
        }

        [Authorize]
        [Route("list")]
        [HttpGet]
        public List<LendingDTO> ListLendings()
        {
            return lendingService.ListLendings(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [Authorize(Policy = "MemberNotAllowed")]
        [Route("close/{barcode}")]
        [HttpPost]
        public ResultDTO CloseLending(string barcode)
        {
            return lendingService.CloseLending(barcode);
        }
    }
}
