using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Zoey.Dapper.DependencyInjection;

namespace Zoey.Dapper.Sample
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var physicalProvider = _env.ContentRootFileProvider;
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddZoeyDapperCore(options =>
            {
                options.Path = new List<string>() { "/Config" };
                options.WatchFileFilter = "*.xml";
                options.StartProxy = false;
            })
            .AddMSSQLserver(option =>
            {
                option.DatabaseElements = new List<DatabaseElement>()
               {
                   new DatabaseElement("Default","Data Source=.;Initial Catalog=Test;Integrated Security=True")
               };
                option.DomainElements = new List<DomainElement>()
               {
                   new DomainElement()
                   {
                       Name = "Default",
                       MasterSlaves = new MasterSlaves("Default","Default")
                   }
               };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
