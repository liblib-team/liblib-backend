using liblib_backend.Common;
using liblib_backend.Controllers.SubjectController;
using liblib_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Services
{
    public interface ISubjectService : ITransientService
    {
        List<SubjectDTO> ListSubjects();
    }

    public class SubjectService : ISubjectService
    {
        private ISubjectRepository subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            this.subjectRepository = subjectRepository;
        }

        public List<SubjectDTO> ListSubjects()
        {
            return subjectRepository.ListSubjects().Select(x => new SubjectDTO()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

    }
}
