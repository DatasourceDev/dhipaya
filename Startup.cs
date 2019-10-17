using System;
using System.Collections.Generic;
using System.Globalization;
using Dhipaya.DAL;
using Dhipaya.ModelsDapper;
using Dhipaya.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.Extensions.Logging;

namespace Dhipaya
{
   public class Smtp
   {
      public string SMTP_SERVER { get; set; }
      public int SMTP_PORT { get; set; }
      public string SMTP_USERNAME { get; set; }
      public string SMTP_PASSWORD { get; set; }
      public bool STMP_SSL { get; set; }
      public string SMTP_FROM { get; set; }

   }
   public class TIPMobile
   {
      public string Url { get; set; }

   }
   public class SystemConf
   {
      public string Environment { get; set; }
      public string Url { get; set; }
      public bool SendSMS { get; set; }
      public bool SendEmail { get; set; }
      public string SupportEmail { get; set; }
      public string FBAppID { get; set; }
   }
   public class IIA
   {
      public string call_sms { get; set; }
      public string GetPolicyActive { get; set; }
      public string CheckMemberStatus { get; set; }
      public string GetPolicy { get; set; }
      public string GetAmphurData { get; set; }
      public string GetDistrictData { get; set; }

      public string EndPoint { get; set; }
      public string sms_endpoint { get; set; }

   }
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

         var connectionString = Configuration.GetConnectionString("DefaultConnection");
         services.AddMvc().AddSessionStateTempDataProvider();

         services.AddSession();
         services.AddDbContext<ChFrontContext>(options => options.UseSqlServer(connectionString, b => b.UseRowNumberForPaging()));
         services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         services.AddTransient<ILoginServices, LoginServices>();
         services.AddTransient<LoginServices>();
         services.AddTransient<ICustomerRepository, CustomerRepository>();
         services.AddTransient<IReportRepository, ReportRepository>();
         services.AddTransient<IPrivilegeRepository, PrivilegeRepository>();

         services.AddScoped<IViewRenderService, ViewRenderService>();
         services.Configure<FormOptions>(options =>
         {
            options.ValueCountLimit = 100000; // 10000 items max
            options.ValueLengthLimit = 1024 * 1024 * 500; // 100MB max len form data
         });

         services.Configure<Smtp>(Configuration.GetSection("Smtp"));
         services.Configure<IIA>(Configuration.GetSection("IIA"));
         services.Configure<TIPMobile>(Configuration.GetSection("TIPMobile"));
         services.Configure<SystemConf>(Configuration.GetSection("SystemConf"));
         services.AddMvc().AddRazorPagesOptions(options =>
         {
            options.Conventions.AddPageRoute("/Home/Index", "");
         });

         services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
         {
            options.LoginPath = "/Accounts/Login";
            options.LogoutPath = "/Accounts/Logout";
         });


         services.ConfigureApplicationCookie(options =>
         {
            options.AccessDeniedPath = "/Accounts/Login";
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
      {

         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();
         }
         else
         {
            app.UseExceptionHandler("/Home/Error");

         }
         loggerFactory.AddConsole();
         loggerFactory.AddDebug();

         using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
         {
            var context = serviceScope.ServiceProvider.GetService<ChFrontContext>();
            context.Database.Migrate();
            context.EnsureSeedData();
         }

         string enUSCulture = "en-US";
         var supportedCultures = new[]
         {
                new CultureInfo("th-TH"),
                new CultureInfo("th"),
            };
         app.UseRequestLocalization(new RequestLocalizationOptions
         {
            DefaultRequestCulture = new RequestCulture(enUSCulture),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
         });


         app.UseStaticFiles();

         app.UseSession();

         app.UseAuthentication();

         app.UseMvc(routes =>
         {
            routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}/{pno?}");

            routes.MapRoute(
                  name: "api",
                  template: "rewardpoint/customerprofile/{action}",
                  defaults: new { controller = "API" });

            routes.MapRoute(
                  name: "sso",
                  template: "rewardpoint/{action}",
                  defaults: new { controller = "Accounts" });

            routes.MapRoute(
                             name: "webview",
                             template: "rewardpoint/view/{action}",
                             defaults: new { controller = "API" });

         });
      }
   }
}
