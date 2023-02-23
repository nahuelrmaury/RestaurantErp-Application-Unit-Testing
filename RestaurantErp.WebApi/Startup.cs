using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Helpers;
using RestaurantErp.Core.Models;
using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Providers;
using RestaurantErp.Core.Providers.Discount;
using System;
using System.Globalization;

namespace RestaurantErp.WebApi
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestaurantErp.WebApi", Version = "v1" });
            });

            var productProvider = new ProductProvider();

            services.AddSingleton<IDiscountByTimeProvider, DiscountByTimeProvider>()
                .AddSingleton<IDiscountProvider, DiscountByTimeProvider>()
                .AddSingleton<IDiscountCalculator, DiscountCalculator>()
                .AddSingleton<IOrderProvider, OrderProvider>()
                .AddSingleton<IServiceChargeProvider, ServiceChargeProvider>()
                .AddSingleton<ITimeHelper, TimeHelper>()
                .AddSingleton<BillHelper>()

                //.AddSingleton<IPriceStorage, ProductProvider>()
                //.AddSingleton<IProductProvider>(p => p.GetService<IPriceStorage>())

                .AddSingleton<ProductProvider>()
                .AddSingleton<IPriceStorage>(p => p.GetRequiredService<ProductProvider>())
                .AddSingleton<IProductProvider>(p => p.GetRequiredService<ProductProvider>())

                .AddSingleton((p => new DiscountCalculatorSettings
                {
                    MinimalProductPrice = decimal.Parse(p.GetService<IConfiguration>()["MinimalProductPrice"])
                }))

                .AddSingleton((p => new DiscountByTimeProviderSettings
                {
                    EndDiscountDelay = TimeSpan.FromSeconds(double.Parse(p.GetService<IConfiguration>()["EndDiscountDelaySeconds"]))
                }))

                .AddSingleton((p => new ServiceChargeProviderSettings
                {
                    ServiceRate = decimal.Parse(p.GetService<IConfiguration>()["ServiceChargeRate"])
                }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.CurrencySymbol = "£";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                await next.Invoke();
            });

            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
