using AutoMapper;
using FraudReporting.Controllers;
using FraudReporting.Services;
using FraudReporting.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FraudReporting
{
    public class Startup
    {
        //Services
        public IConfiguration Configuration { get; private set; }
        public IWebHostEnvironment WebHostingEnvironment { get; private set; }
        public IApplicationBuilder ApplicationBuilder { get; private set; }

        //Methods
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostingEnvironment)
        {
            if (System.Diagnostics.Debugger.IsAttached && (webHostingEnvironment.IsStaging() || webHostingEnvironment.IsProduction()))
                throw new Exception("Running in staging or production build on localhost");

            this.Configuration = configuration;
            this.WebHostingEnvironment = webHostingEnvironment;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            //Enable Sessions
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(3600); //Default APF session timeout is 1 hour (3600 seconds)
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //Enable cooke-based authentication (used instead of ASP.NET Core Identity) - https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = $"/{NotificationController.NameOf}/{nameof(NotificationController.AccessDenied)}";
                    options.ReturnUrlParameter = "ReturnUrl";
                });

            //Set up base MVC services
            services.AddSession();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation(); //Enable view changes when refreshing page when on localhost. Need to have Nuget package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation - see here: https://docs.microsoft.com/en-us/aspnet/core/mvc/views/view-compilation?view=aspnetcore-3.1&tabs=visual-studio
            services.AddAutoMapper(typeof(Startup).Assembly); //See here for AutoMapper .Net Core examples: https://tutexchange.com/how-to-set-up-automapper-in-asp-net-core-3-0/

            //Set the HTTPS redirection in non-local/dev environments
            if (WebHostingEnvironment.IsStaging() || WebHostingEnvironment.IsProduction())
            {
                services.AddHttpsRedirection(options =>
                {
                    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                    options.HttpsPort = 443;
                });
            }

            //Set HSTS policy expiration time
            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365); //Arrowhead standard policy is 1 year
            });

            //Add core web singletons to inject into other services as needed
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWebHostEnvironment>(WebHostingEnvironment);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            //Add the rest of the services - each of these AddScoped<>() so only one object for each is created an re-used throughout a given request 
            services.AddScoped<ApfVariablesService>();
            services.AddScoped<ComponentsValidationService>();
            services.AddScoped<FileIOService>();
            services.AddScoped<FraudReportingService>();
            services.AddScoped<IPathService, PathService>();
            services.AddScoped<PasswordManagerService>();
            services.AddScoped<SessionService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            //Set up the static service locator. Note this this is not a good practice, and should only be used as a last resort. See here: https://dotnetcoretutorials.com/2018/05/06/servicelocator-shim-for-net-core/
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
            ServiceLocator.SetServiceCollection(services);
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Entity Framework - Run any necessary migrations to update the database schema to the latest version - need to do this before anything else
            //promoContext.Database.Migrate();


            //Define the base cookie policty
            var cookiePolicy = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
            };


            //Configure based on environment
            if (env.EnvironmentName == "Local" || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //Displays the full stack trace of any exceptions - should not be used in any client or user facing deployment (stage/live)
                //app.UseExceptionHandler($"/{NotificationController.NameOf}/{nameof(NotificationController.Error)}");
            }
            else
            {
                app.UseExceptionHandler($"/{NotificationController.NameOf}/{nameof(NotificationController.Error)}");
                if (ApfSettings.requireHttps)
                {
                    app.UseHttpsRedirection();
                    app.UseHsts(); //Max age is set in ConfigureServices()
                    cookiePolicy.Secure = CookieSecurePolicy.Always; //Only set secure on live sites, since localhost and apfcodev won't have HTTPS - can disable for Intranet15 sites
                }
            }


            //Adjust any HTTP response headers
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("X-Frame-Options");
                context.Response.Headers.Remove("Content-Security-Policy");

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self'; default-src * 'unsafe-inline' data: blob:;");

                await next();
            });


            //Add & configure any additional functionality needed
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy(cookiePolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession(); //Must put before UseEndpoints: https://bytenota.com/solved-invalidoperationexception-session-has-not-been-configured-for-this-application-or-request/


            //Configure endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            //Finally do the APF-specific initialization
            ApfSettings.Initialize();
        }

    }
}
