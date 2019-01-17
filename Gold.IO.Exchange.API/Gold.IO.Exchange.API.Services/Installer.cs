using Microsoft.Extensions.DependencyInjection;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.BusinessLogic.Services;

namespace Gold.IO.Exchange.API.BusinessLogic
{
    public static class Installer
    {
        public static void AddBuisnessServices(this IServiceCollection container)
        {
            container
                .AddScoped<IUserService, UserService>()
                .AddScoped<IPersonService, PersonService>()
                .AddScoped<ILocaleService, LocaleService>()
                .AddScoped<ICountryService, CountryService>()
                .AddScoped<ICityService, CityService>();
        }
    }
}
