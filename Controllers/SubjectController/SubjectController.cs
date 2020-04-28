using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using liblib_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace liblib_backend.Controllers.SubjectController
{
    [Route("api/[controller]/")]
    [ApiController]
    public class SubjectController : ControllerBase
    {

        private ISubjectService subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            this.subjectService = subjectService;
        }

        [Route("list")]
        [HttpGet]
        public List<SubjectDTO> ListSubjects()
        {
            return subjectService.ListSubjects();
        }

    }
}