﻿using Microsoft.Extensions.DependencyInjection;
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
                .AddScoped<ICityService, CityService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<ICoinService, CoinService>()
                .AddScoped<ICoinAddressService, CoinAddressService>()
                .AddScoped<ICoinAccountService, CoinAccountService>()
                .AddScoped<IUserWalletService, UserWalletService>()
                .AddScoped<IUserWalletOperationService, UserWalletOperationService>()
                .AddScoped<IOrderService, OrderService>()
                .AddScoped<IUserKeyService, UserKeyService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IUserNotificationsService, UserNotificationsService>()
                .AddScoped<IApiKeyService, ApiKeyService>()
                .AddScoped<IUserSessionService, UserSessionService>()
                .AddScoped<IBitcoinService, BitcoinService>()
                .AddScoped<IEthereumService, EthereumService>()
                .AddScoped<IRabbitMQService, RabbitMQService>();
        }
    }
}
