using System.Globalization;
using System.ServiceModel;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Menu4Tech.Helper;
using Menu4Tech.IBusinessAPIservice;
using Menu4Tech.Middlewares;
using Menu4Tech.Models.Interfaces;
using Menu4Tech.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using SmartWinners.Helpers;

namespace Menu4Tech
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web hosting environment.</param>
        /// <param name="config">The configuration.</param>
        /// <remarks>
        /// Only a few services are possible to be injected here https://github.com/dotnet/aspnetcore/issues/9337.
        /// </remarks>
        public Startup(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            EnvironmentHelper.Environment = webHostEnvironment;
            
            _env = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceCollection = services.AddHttpContextAccessor();
            EnvironmentHelper.DiContainer = services.BuildServiceProvider();

            FileLogger.Environment = _env;
            
            var api = new BusinessAPIClient(BusinessAPIClient.EndpointConfiguration.IBusinessAPIPort,
                _env.IsDevelopment()
                    ? new EndpointAddress("http://lease4.mekashron.com:33322/soap/IBusinessAPI")
                    : new EndpointAddress("http://lease4.mekashron.com:33322/soap/IBusinessAPI"));

            services.AddSingleton(api);
            services.AddRouting();
            services.AddDetection();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IMenu, MenuService>();
            services.AddSingleton<ICredential, CredentialService>();
            services.AddSingleton<IViewHelper, ViewHelper>();
            services.AddScoped<ILogRequest, LogRequestService>();
            services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
            services.AddControllersWithViews();
            services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo> {
                    new CultureInfo("eng"),
                    new CultureInfo("heb")
                };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("eng");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
            services.AddRouting(x => { x.LowercaseUrls = true; });

            services.AddUmbraco(_env, _config)
                .AddBackOffice()
                .AddWebsite()
                .AddComposers()
                .Build();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            var forwardingOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            app.UseForwardedHeaders(forwardingOptions);
            
            app.UseMiddleware<RedirectMiddleware>();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo>
                {
                    new("he"),
                    new("ru"),
                    new("en")
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    new("he"),
                    new("ru"),
                    new("en")
                },
                DefaultRequestCulture = new RequestCulture("en")
            });


            app.UseDeveloperExceptionPage();
            /*app.UseExceptionHandler("/Error/Handler");*/
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseDetection();
            app.UseRouting();
            app.UseNotyf();
            app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();
                })
                .WithEndpoints(u =>
                {
                    u.UseInstallerEndpoints();
                    u.UseBackOfficeEndpoints();
                    u.UseWebsiteEndpoints();
                });
        }
    }
}