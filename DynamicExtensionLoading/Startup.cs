using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;
using Services;

namespace DynamicExtensionLoading
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
            var mvcBuilder = services.AddControllers().AddControllersAsServices();

            AddExtensionEndPoints(mvcBuilder);
            services.AddSwaggerGen();

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IDataService, DataService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }

        private static IMvcBuilder AddExtensionEndPoints(IMvcBuilder mvcBuilder)
        {
            var baseDirectory =  Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if(string.IsNullOrEmpty(baseDirectory)) throw new Exception("Invalid base directory");

            var extensionsPath = Path.Combine(baseDirectory, "Extensions\\netcoreapp3.1");
            foreach (var dll in Directory.GetFiles(extensionsPath, "*.dll"))
            {
                mvcBuilder.AddApplicationPart(Assembly.LoadFile(dll));
            }

            return mvcBuilder;
        }
    }
}
