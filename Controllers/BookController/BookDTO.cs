using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Controllers.BookController
{
    public class BookDTO
    {

        public Guid Id;

        public string Title;

        public string Image;

        public string Description;

        public List<AuthorDTO> Authors;

    }
}
