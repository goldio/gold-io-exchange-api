using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain;
using Gold.IO.Exchange.API.Storage.Interfaces;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class CountryService : BaseCrudService<Country>, ICountryService
    {
        public CountryService(IRepository<Country> repository) : base(repository)
        {
        }
    }
}
