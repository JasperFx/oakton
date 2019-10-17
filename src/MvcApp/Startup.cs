using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oakton.AspNetCore;
using Oakton.AspNetCore.Descriptions;
using Oakton.AspNetCore.Environment;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MvcApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // SAMPLE: ConfigureService-with-EnvironmentCheck
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Other registrations we don't care about...
            
            // This extension method is in Oakton.AspNetCore
            services.CheckEnvironment<IConfiguration>("Can connect to the application database", config =>
            {
                var connectionString = config["connectionString"];
                using (var conn = new SqlConnection(connectionString))
                {
                    // Just attempt to open the connection. If there's anything
                    // wrong here, it's going to throw an exception
                    conn.Open();
                }
            });
            
            // Ignore this please;)
            services.AddSingleton<IDescribedSystemPart, Describer1>();
            services.AddSingleton<IDescribedSystemPart, Describer2>();
            services.AddSingleton<IDescribedSystemPart, Describer3>();
        }
        // ENDSAMPLE

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#if NETCOREAPP2_2
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        #else
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
#endif
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
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
