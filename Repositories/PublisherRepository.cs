using liblib_backend.Common;
using liblib_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liblib_backend.Repositories
{
    public interface IPublisherRepository : ITransientService
    {
        Publisher GetPublisherById(Guid? publisherId);
    }

    public class PublisherRepository : IPublisherRepository
    {
        private MyLibContext DbContext;

        public PublisherRepository(MyLibContext DbContext)
        {
            this.DbContext = DbContext;
        }

        public Publisher GetPublisherById(Guid? publisherId)
        {
            return DbContext.Publisher.FirstOrDefault(x => x.Id == publisherId);
        }
    }
}
