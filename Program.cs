using Microsoft.Identity.Web;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSAuth.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;
using MSAuth.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using MSAuth.Filters;
using MSAuth.Services;
using MSAuth.Interfaces;
using MSAuth.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

//? read env file
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");

if (!File.Exists(dotenv))
    return;

foreach (var line in File.ReadAllLines(dotenv))
{
    var parts = line.Split(
        '=',
        StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length != 2)
        continue;

    Environment.SetEnvironmentVariable(parts[0], parts[1]);
}

string clientId = Environment.GetEnvironmentVariable("﻿ClientID")!;
string clientSecret = Environment.GetEnvironmentVariable("ClientSecret")!;
string url = Environment.GetEnvironmentVariable("CRMUrl")!;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MSAuthContextConnection") ?? throw new InvalidOperationException("Connection string 'MSAuthContextConnection' not found.");

builder.Services.AddDbContext<MSAuthContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MSAuthContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ICrmService, CrmService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddTransient<IClaimsTransformation, CustomClaimsTransformer>();

builder.Services.AddTransient<ServiceClient>(provider =>
{
    string connection = $"AuthType=ClientSecret;Url={url};ClientId={clientId};ClientSecret={clientSecret}";
    return new ServiceClient(connection);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Index}/{id?}");

app.Run();
