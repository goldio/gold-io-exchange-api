using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class LocaleService : BaseCrudService<Locale>, ILocaleService
    {
        public LocaleService(IRepository<Locale> repository) : base(repository)
        {
        }
    }
}
