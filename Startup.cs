/*
Copyright (c) 2022 Jim Thatcher

Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/
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
