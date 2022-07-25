using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quant.BackTesting.Service.DataApi.Implementation;
using Quant.BackTesting.Service.DataApi.Interface;
using Quant.BackTesting.RSIIntradayMean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quant.BackTesting.Strategies.FadeTheGap.implementation;
using Quant.BackTesting.Strategies.FadeTheGap.Interface;
using Microsoft.EntityFrameworkCore;
using Quant.BackTesting.Infrastructure.Data;
using Quant.BackTesting.Service.SyncData.Implementation;
using Quant.BackTesting.Service.SyncData.Interface;

namespace Quant.BackTesting.Web
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


            services.AddControllersWithViews();
            services.AddSession(options =>
                      {
                          options.IdleTimeout = TimeSpan.FromHours(16);
                      });
            var conStr = "Data Source=.;Initial Catalog=HistoricalData;Integrated Security=True";
            services.AddDbContext<HistoricalDataContext>(options => options.UseSqlServer(conStr));
            services.AddScoped<IHistoricalDataPuller, HistoricalDataPuller>();
            services.AddScoped<IFlowClass, FlowClass>();
            services.AddScoped<IFadeTheGapFlowClass, FadeTheGapFlowClass>();
            services.AddScoped<ISyncStockData, SyncStockData>();
            services.AddScoped<IHistoricalStockData, HistoricalStockData>();
            services.AddScoped<ITrendFollowingFlowClass, TrendFollowingFlowClass>();
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
