using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RateLimitThrottle.Demo
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
            services.AddOptions();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //添加限流组件
            services.AddRateLimiting(options =>
            {
                //前缀，区分不同接口站点
                options.RateLimitCounterPrefix = "testApi1";
                //策略缓存时间
                options.ProlicyCacheTime = 300;
                //限流后返回的http状态码
                options.HttpStatusCode = 429;
                //限流后返回的http消息
                options.HttpResponseFomatterMessage = "请求过于频繁";
                //自定义日志输出
                options.LogHandler = msg =>
                {
                    Console.WriteLine(msg);
                };

                //自定义client获取方式，根据host
                options.OnGetClient = httpContext =>
                {
                    var host = httpContext.Request.Host.Host;
                    return host;
                };

                //自定义ip获取方式, 获取nginx反向代理请求头
                //options.OnGetIpAddress = httpContext =>
                //{
                //    var ip = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                //    return ip;
                //};
            })
            //使用内存缓存
            .AddMemoryPolicyCache()
            //Memory计数器
            .AddMemoryCounter()
            //Redis计数器
            //.AddRedisCounter(options =>
            //{
            //    options.ConnectionString = "192.168.1.19:6379,defaultDatabase=1,poolsize=50";
            //})
            //策略 读取文件
            .AddDefaultPolicyStore(Configuration.GetSection("RateLimitStoreSettings"));
            //策略 读取mysql
            //.AddMySqlPolicyStore(options =>
            //{
            //    options.ConnectionString = "Server=192.168.1.19;User Id=admin;Password=123456;Database=bihu_sso;Allow User Variables=True;";
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseClientRateLimiting();//使用Client策略
            //app.UseIPRateLimiting();//使用IP策略

            app.UseMvc();
        }
    }
}
