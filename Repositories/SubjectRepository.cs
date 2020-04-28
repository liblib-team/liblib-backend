using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface ISubjectRepository : ITransientService
    {
        List<Subject> ListSubjects();
        List<Subject> ListSubjectsByBookId(Guid bookId);
    }

    public class SubjectRepository : ISubjectRepository
    {
        private MyLibContext DbContext;

        public SubjectRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public List<Subject> ListSubjects()
        {
            return DbContext.Subject.ToList();
        }

        public List<Subject> ListSubjectsByBookId(Guid bookId)
        {
            return DbContext.BookSubject
                .Where(x => x.BookId == bookId)
                .Join(DbContext.Subject, x1 => x1.SubjectId, x2 => x2.Id, (x1, x2) => x2)
                .ToList();
        }
    }
}
