using System;
using Engine.Geobase;
using Engine.Geobase.Combined;
using Engine.Geobase.Dirrect;
using Engine.Geobase.Marshal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WepApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }
        private string GeobaseFileName { get; }
        private string GeobaseEngineType { get; }
        

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
            GeobaseFileName = configuration["GeobaseFileName"];
            GeobaseEngineType = configuration["GeobaseEngineType"] ?? string.Empty;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var geobasePath =  System.IO.Path.Combine(HostingEnvironment.ContentRootPath, GeobaseFileName);

            IGeobaseEngine geobaseEngine;
            if (GeobaseEngineType.ToLower() == "marshal")
            {
                geobaseEngine = new GeobaseEngineMarshal(geobasePath);
            }
            else if (GeobaseEngineType.ToLower() == "dirrect")
            {
                geobaseEngine = new GeobaseEngineDirrect(geobasePath);
            }
            else if (GeobaseEngineType.ToLower() == "combined")
            {
                geobaseEngine = new GeobaseEngineCombined(geobasePath);
            }
            else
            {
                throw new ArgumentException("Incorrect parameter 'GeobaseEngineType' in the config file. Please use 'marshal' or 'dirrect'.");
            }

            
            services.AddSingleton<IGeobaseEngine>(geobaseEngine);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
