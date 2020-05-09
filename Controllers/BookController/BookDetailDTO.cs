using liblib_backend.Controllers.SubjectController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Controllers.BookController
{
    public class BookDetailDTO
    {
        public Guid Id;

        public string Title;

        public string Image;

        public string Description;

        public string Publisher;

        public double Point;

        public string Language;

        public List<AuthorDTO> Authors;

        public List<SubjectDTO> Subjects;

    }

}
