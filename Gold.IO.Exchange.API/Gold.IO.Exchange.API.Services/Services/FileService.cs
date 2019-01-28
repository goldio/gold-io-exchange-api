using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class FileService : BaseCrudService<File>, IFileService
    {
        public FileService(IRepository<File> repository) : base(repository)
        {
        }
    }
}
