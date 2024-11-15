using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using TopLearn.Core.Convertors;
using TopLearn.Core.Services.Interfaces;
using TopLearnWeb.DataLayer.Context;

internal class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        #region Authentication

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);
        });

        #endregion

        #region DataBase Context

        builder.Services.AddDbContext<TopLearnContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("TopLearnConnection"));
        });

        #endregion

        #region IoC

        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IViewRenderService, RenderViewToString>();
        builder.Services.AddTransient<IPermissionService, PermissionService>();
        #endregion

        builder.Services.AddControllersWithViews(options =>
        {
            options.EnableEndpointRouting = false; // Disable endpoint routing
        });

        var app = builder.Build();

        //Configure Http Request PipeLine :

        #region API PipeLine
        // if(app.Environment.IsDevelopment())
        // {
        //     app.UseSwagger();
        //     app.UseSwaggerUI();
        // }
        // app.UseRouting();
        // app.UseAuthorization();
        // app.UseEndpoints(endpoints => 
        // {
        //     endpoints.MapControllers();
        // });
        // app.MapControllers();
        #endregion

        #region WebAPP PipeLine
        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // Map Razor Pages
            endpoints.MapRazorPages();

            endpoints.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            // Map default route for MVC
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        #endregion

        app.Run();
    }
}