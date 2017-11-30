using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpPlug.Core;
using SharpPlug.WebApi.Configuration;
using SharpPlug.WebApi.Router;
using Swashbuckle.AspNetCore.Swagger;
using SharpPlug.WebApi.Swashbuckle;
namespace SharpPlug.WebApiTest
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSharpPlugCore()
                .AddWebApiRouter(custom =>
            {
                custom.CustomRule.Add("Get1", HttpVerbs.HttpGet);
            }).AddSharpPlugSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "WebApi Test",
                    Description = "WebApi Test",
                });
                c.DocInclusionPredicate((docname, des) => true);
             
            });
            services.AddMvc();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi Test V1");
                c.ShowRequestHeaders();
            });
            app.UseMvc();
        }
    }
}
