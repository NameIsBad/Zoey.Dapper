using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Zoey.Dapper.Configuration;
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

            //SQL文件的读取和监视依赖 IFileProvider
            var physicalProvider = _env.ContentRootFileProvider;
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddZoeyDapperCore(options =>
            {
                //SQL文件夹的路径  支持多个文件夹
                options.Path = new List<string>() { "/Config" };
                //监控文件的后缀(默认未 *.*)
                options.WatchFileFilter = "*.xml";
                //该属性暂时没用
                options.StartProxy = false;
            })
            //添加MSSQLserver
            //这里说明一下 该方法是非必要的,下面会说
            .AddMSSQLserver(option =>
            {
                //添加数据库连接字符串
                //这里为什么没用配置文件读取,考虑到可能用到(Secret Manager)
                //后面可以提供直接从配置文件中读取
                option.DatabaseElements = new List<DatabaseElement>()
                {
                    //参数1:唯一名称
                    //参数2:连接字符串
                    new DatabaseElement("TESTDB","Data Source=.;Initial Catalog=Test;Integrated Security=True")
                };
                //此处就是上面提到的 domain 节点
                //每个domain对象有个唯一名称(xml文件domain的节点)
                //每个domain对象都有 Master(主库) 和 Slave(从库) 的名称(上面配置信息的名称)
                option.DomainElements = new List<DomainElement>()
                {
                    new DomainElement()
                    {
                        //xml文件domain节点的名称
                        Name = "Default",
                        //主库和从库的名称(上面配置信息的名称)
                        //主库和从库可配多个(负载均衡算法暂没实现)
                        MasterSlaves = new MasterSlaves("TESTDB","TESTDB")
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
