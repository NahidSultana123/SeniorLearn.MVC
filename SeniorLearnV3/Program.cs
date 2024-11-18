////using Microsoft.AspNetCore.Authorization;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.EntityFrameworkCore;
////using SeniorLearnV3.Data;
////using SeniorLearnV3.Data.Identity;

////namespace SeniorLearnV3
////{
////    public class Program
////    {
////        public static void Main(string[] args)
////        {
////            var builder = WebApplication.CreateBuilder(args);

////            // Add services to the container.
////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
////            builder.Services.AddDbContext<ApplicationDbContext>(options =>
////                options.UseSqlServer(connectionString));
////            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

////            // Done to simplify
////            builder.Services.AddDefaultIdentity<User>(options =>
////            {
////                options.SignIn.RequireConfirmedAccount = false;
////                options.Password.RequireDigit = false;
////                options.Password.RequireNonAlphanumeric = false;
////                options.Password.RequiredLength = 1;
////                options.Password.RequireLowercase = false;
////                options.Password.RequireUppercase = false;
////            })
////                .AddUserStore<UserStore>()
////                .AddUserManager<UserManager>()
////                .AddRoles<Role>()
////                .AddRoleStore<RoleStore>()
////                .AddEntityFrameworkStores<ApplicationDbContext>();

////            builder.Services.AddTransient<IAuthorizationHandler, ActiveRoleHandler>(); //TODO: Research more

////            //TODO: Configure Cookie/JWT Authentication/Authorization Policy

////            builder.Services.AddControllersWithViews();

////            //TODO: builder.Services.AddAutoMapper(typeof(SeniorLearn.Web.Mapper.Profile)); 

////            var app = builder.Build();

////            //TODO: app.UseRequestLocalization();  ??

////            // Configure the HTTP request pipeline.
////            if (app.Environment.IsDevelopment())
////            {
////                app.UseMigrationsEndPoint();
////            }
////            else
////            {
////                app.UseExceptionHandler("/Home/Error");
////                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
////                app.UseHsts();
////            }

////            app.UseHttpsRedirection();
////            app.UseStaticFiles();

////            app.UseRouting();

////            //TODO: app.UserCors


////            // app.UseAuthentication();  // TODO: extend later
////            app.UseAuthentication();
////            app.UseAuthorization();

////            app.MapControllerRoute(
////            name: "areas",
////            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

////            app.MapControllerRoute(
////                name: "default",
////                pattern: "{controller=Home}/{action=Index}/{id?}");

////            app.MapRazorPages();

////            app.Run();
////        }
////    }
////}
///
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using SeniorLearnV3.Data;
//using SeniorLearnV3.Data.Configuration.Migrations;
using SeniorLearnV3.Data.Identity;
using System;
using System.Globalization;
using System.Text;
using static System.Net.WebRequestMethods;

var random = new Random();

for (int i = 0; i < 1000; i++)
{
    Console.WriteLine((random.Next(8, 200) / 2) * 2);
}


//TODO: Project Creation, MVC scaffolding set up and configured in program.cs file and nuget packages installed
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//Add services to the container.
//Set up dependency injection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//{
//    options.UseSqlServer(connectionString, s =>
//    {
//        s.UseAzureSqlDefaults(true);
//        s.EnableRetryOnFailure(5);
//    })
//    .ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>();
//});

//Setup dependency injection for identity
//TODO: Configure Identity Access Management
builder
    .Services.AddDefaultIdentity<User>(options => {
        options.SignIn.RequireConfirmedAccount = false;//n
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddUserStore<UserStore>()
    .AddUserManager<UserManager>()
    .AddRoles<Role>() // n
    .AddRoleStore<RoleStore>()
    .AddEntityFrameworkStores<ApplicationDbContext>(); //n

builder.Services.AddTransient<IAuthorizationHandler, ActiveRoleHandler>();

//TODO: Customize Idenity Role Authorize to require extended Active property to be true.
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("ActiveRole", p => p.AddRequirements(new ActiveRolePolicy()));
});

//TODO: Configure Cookie/JWT Authentication/Authorization Policy
builder.Services
    .AddAuthentication(o =>
    {
        o.DefaultScheme = "SeniorLearnScheme";  //"JWT_OR_COOKIE"
        o.DefaultChallengeScheme = "SeniorLearnScheme"; //"JWT_OR_COOKIE"
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]!)),
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
    }).AddPolicyScheme("SeniorLearnScheme", "SeniorLearnScheme", o =>
    {
        o.ForwardDefaultSelector = c =>
        {
            string auth = c.Request.Headers[HeaderNames.Authorization]!;
            if (!string.IsNullOrWhiteSpace(auth) && auth.StartsWith("Bearer "))
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
            return IdentityConstants.ApplicationScheme;
        };

    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("en-AU") };
    options.DefaultRequestCulture = new RequestCulture("en-AU");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(SeniorLearnV3.Mapper.Profile));

var app = builder.Build();
app.UseRequestLocalization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//TODO: Cors middleware - checked after sending preflight request
app.UseCors(policy =>
{
    policy.AllowAnyOrigin(); //Allows requests from any origin
    policy.AllowAnyHeader(); //Allows any HTTP headers in the request.
    policy.AllowAnyMethod(); //Allows any HTTP methods(e.g. GET, POST, PUT, DELETE).
});

app.UseAuthentication(); //n
app.UseAuthorization();

app.MapControllerRoute(
     name: "areas",
 pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//This is has replace with the data seeder but am keeping here as a demonstration of how to if need be -)-+{
//await RegisterInitialRolesAndAdministrator(app);

app.Run();

// Additional area routes (if you need specific ones)
//app.MapControllerRoute(
//    name: "Professional",
//    pattern: "Professional/{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "Standard",
//    pattern: "Standard/{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "Administration",
//    pattern: "Administration/{controller=Home}/{action=Index}/{id?}");

//This is has replace with the data seeder but am keeping here as a demonstration of how to if need be -)-+{
//await RegisterInitialRolesAndAdministrator(app);

app.Run();



//HACK: exposed for test program (should probably update csproj to expose internals to test project instead, good enough for now :)
public partial class Program { }
