using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.CityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace backend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }

        public Startup(IConfiguration configuration, ILoggerFactory lf)
        {
            Configuration = configuration;
            LoggerFactory = lf;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<City>(modelSetup => {
                var cityConfig = Configuration.GetValue<Config>("City");
                modelSetup.Configure(cityConfig);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var logger = LoggerFactory.CreateLogger<Startup>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseWebSockets();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
