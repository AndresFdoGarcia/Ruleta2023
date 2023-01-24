using Autofac;
using Microsoft.Extensions.Configuration;
using Ruleta2023.Business.Core.IoCContainer;
using Ruleta2023.Business.Core.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Ruleta2023
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            string executionEnvironment = Environment.GetEnvironmentVariable("RULETA2023_ENVIRONMENT");
            string environmentConfigurationFile = EnvironmentFinder.GetConfigurationFileName(executionEnvironment);

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile(environmentConfigurationFile, true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            IoCServiceCollection.BuildContext(builder, Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen();
            services.AddLogging();
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });            
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UsePathBase("/Ruleta2023");            

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ruleta2023 V1");
            });

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
