// Server side

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Notes2022.Server;
using Notes2022.Server.Data;
using Notes2022.Server.Models;
using Notes2022.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<NotesDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NotesDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, NotesDbContext>();

//builder.Services.AddSignalR();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

});

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);



Globals.StartupDateTime = DateTime.Now.ToUniversalTime();

Globals.ProductionUrl = builder.Configuration["ProductionUrl"];

Globals.ImportRoot = builder.Configuration["ImportRoot"];

Globals.TimeZoneDefaultID = 54; // not needed with blazor

Globals.SendGridApiKey = builder.Configuration["SendGridApiKey"];
Globals.SendGridEmail = builder.Configuration["SendGridEmail"];
Globals.SendGridName = builder.Configuration["SendGridName"];

Globals.DBConnectString = builder.Configuration.GetConnectionString("DefaultConnection");

Globals.PrimeAdminName = "Dale Sinder";
Globals.PrimeAdminEmail = "sinder@illinois.edu";

try  // replace default with config values,  fails during migration so try
{
    Globals.PrimeAdminName = builder.Configuration["PrimeAdminName"];
    Globals.PrimeAdminEmail = builder.Configuration["PrimeAdminEmail"];
}
catch
{
    //ignore
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

//app.UsePathBase(Globals.PathBase);

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
