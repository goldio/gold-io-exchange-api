using Gold.IO.Exchange.API.BusinessLogic;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Storage;
using Gold.IO.Exchange.API.WebSocketManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Gold.IO.Exchange.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddBuisnessServices();
            services.AddNHibernate("Server=localhost;Port=3306;Uid=root;Pwd=admin123;Database=exchange_db;SslMode=none;");
            //services.AddNHibernate("Server=localhost;Port=3306;Uid=root;Pwd=vUw7pf9GALbg;Database=goldio_exchange;SslMode=none;");
            services.AddCors();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddWebSocketManager();
            services.AddScoped<TradeManager.TradeManager>();
            services.AddScoped<TransactionsManager.TransactionsManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            app.UseCors(builder =>
                builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();

            //app.UseHttpsRedirection();
            app.UseMvc();

            app.UseWebSockets();

            var notifMessageHandler = serviceProvider.GetService<NotificationsMessageHandler>();
            notifMessageHandler.SetServices(
                serviceProvider.GetService<IUserService>(),
                serviceProvider.GetService<IOrderService>());

            app.MapWebSocketManager("/notifications", notifMessageHandler);

            var tradeManager = serviceProvider.GetService<TradeManager.TradeManager>();
            tradeManager.SetServices(
                serviceProvider.GetService<IOrderService>(),
                serviceProvider.GetService<IUserWalletService>());

            var transactionsManager = serviceProvider.GetService<TransactionsManager.TransactionsManager>();
            transactionsManager.SetServices(
                serviceProvider.GetService<IUserWalletService>(), 
                serviceProvider.GetService<IUserWalletOperationService>(),
                serviceProvider.GetService<ICoinAddressService>());
        }
    }
}
