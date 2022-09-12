using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using DakAccess;
using Lib.AspNetCore.ServerSentEvents;

namespace WebAPI
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
            // services.AddHostedService<ConsoleData>();
            // services.AddTransient<ConsoleData>();
            services.AddDbContext<GameContext>(opt => opt.UseInMemoryDatabase("ScoreBoard"));
            services.AddDbContext<GameMgrContext>(opt => { 
                
                opt.UseSqlite(Configuration.GetConnectionString("GameMgrContext"));
                
            });
            // The Scorebaord Console connection service needs to have a single instance so that the controllers can read
            // the same data written by the service.
            services.AddControllers();
            services.AddSingleton<ConsoleData>();
            /*
            services.AddMvc(setupAction=> {
                setupAction.EnableEndpointRouting = false;
                }).AddJsonOptions(jsonOptions => {
                    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            */
            // services.AddRazorPages();
            services.AddServerSentEvents();
            //services.AddSingleton<IHostedService, HeartbeatService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }
            AppDomain.CurrentDomain.SetData("WebRootPath", env.WebRootPath);

            // app.UseHttpsRedirection();
            // Instantiate the scoreboard console connection service, and start its connection timer process
            var consoleData = app.ApplicationServices.GetRequiredService<ConsoleData>();
            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
            }
            // TODO: Add web app control to request connect and disconnect to scoreboard console. 
            // Consider whether we want to auto-connect to the console when the app starts.
            // The downside of auto-connect is that the process will claim any serial port connected
            // to the machine unless the console/disconnect API is called.
            consoleData.ConnectConsole();

            app.UseRouting();
            app.UseStaticFiles();
            /*
            var webSocketOptions = new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromMinutes(2)
                };
            app.UseWebSockets(webSocketOptions);
            */
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                // endpoints.MapRazorPages();
                endpoints.MapServerSentEvents("/events");
                // endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
