using Grpc.Core;
using GrpcGreeter.Framework;
using GrpcGreeterClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrpcGreeter.Web.MVC
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
            services.AddRazorPages();

            #region gRPC
            //集中管理
            services.AddGrpcClient<CustomMath.CustomMathClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
                options.Interceptors.Add(new CustomClientLoggerInterceptor());
            });

            services.AddGrpcClient<ZhaoxiLesson.ZhaoxiLessonClient>(options =>
            {
                options.Address = new Uri("https://localhost:6001");
                options.Interceptors.Add(new CustomClientLoggerInterceptor());
            });
            //.ConfigureChannel(grpcOptions =>
            //{
            //    var callCredentials = CallCredentials.FromInterceptor(async (context, metadata) =>
            //    {
            //        string token = JWTTokenHelper.GetJWTToken().Result;//即时获取的--加一层缓存
            //        Console.WriteLine($"token:{token}");
            //        metadata.Add("Authorization", $"Bearer {token}");
            //    });
            //    grpcOptions.Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials);
            //    //请求都带上token，也可以在调用方法时传递： var replyPlus = await client.PlusAsync(requestPara, headers);
            //});

            services.AddGrpcClient<ZhaoxiUser.ZhaoxiUserClient>(options =>
            {
                options.Address = new Uri("https://localhost:443");
                //options.Interceptors.Add(new CustomClientLoggerInterceptor());
            }).ConfigureChannel(grpcOptions =>
            {
                Console.WriteLine("This ZhaoxiUser.ZhaoxiUserClien ConfigureChannel");
                //grpcOptions.HttpClient = new HttpClient(new HttpClientHandler
                //{
                //    ServerCertificateCustomValidationCallback = (msg, cert, chain, error) => true//忽略证书
                //});//HttpClient--443 代替 grpc-https://localhost:5001
                grpcOptions.HttpHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (msg, cert, chain, error) => true
                };
            });
            #endregion
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
