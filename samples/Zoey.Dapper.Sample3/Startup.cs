using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Zoey.Dapper.Configuration;

namespace Zoey.Dapper.Sample3
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SQL�ļ��Ķ�ȡ�ͼ������� IFileProvider
            var physicalProvider = _env.ContentRootFileProvider;
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddZoeyDapperCore(options =>
            {
                //SQL�ļ��е�·��  ֧�ֶ���ļ���
                options.Path = new List<string>() { "/Config" };
                //����ļ��ĺ�׺(Ĭ��δ *.*)
                options.WatchFileFilter = "*.xml";
                //��������ʱû��
                options.StartProxy = false;
            })
            //���MSSQLserver
            //����˵��һ�� �÷����ǷǱ�Ҫ��,�����˵
            .AddMSSQLserver(option =>
            {
                //������ݿ������ַ���
                //����Ϊʲôû�������ļ���ȡ,���ǵ������õ�(Secret Manager)
                //��������ṩֱ�Ӵ������ļ��ж�ȡ
                option.DatabaseElements = new List<DatabaseElement>()
                {
                    //����1:Ψһ����
                    //����2:�����ַ���
                    new DatabaseElement("TESTDB","Data Source=.;Initial Catalog=Test;Integrated Security=True")
                };
                //�˴����������ᵽ�� domain �ڵ�
                //ÿ��domain�����и�Ψһ����(xml�ļ�domain�Ľڵ�)
                //ÿ��domain������ Master(����) �� Slave(�ӿ�) ������(����������Ϣ������)
                option.DomainElements = new List<DomainElement>()
                {
                    new DomainElement()
                    {
                        //xml�ļ�domain�ڵ������
                        Name = "Default",
                        //����ʹӿ������(����������Ϣ������)
                        //����ʹӿ������(���ؾ����㷨��ûʵ��)
                        MasterSlaves = new MasterSlaves("TESTDB","TESTDB")
                    }
                };
            });

            services.AddControllersWithViews();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
