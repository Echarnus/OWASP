using Microsoft.AspNetCore.Antiforgery;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/* 
 * Headers part for security
 * 
 */

// OWASP: Adds Strict-Transport-Security header
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
});

// OWASP: Surpress removal of the xframeoptions header done by AddAntiForgery
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler("/Home/Error");


// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

// OWASP: Adds Strict-Transport-Security header
app.UseHsts(); // 
app.UseHttpsRedirection();


app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");

    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");

    context.Response.Headers.Add("Referrer-Policy", "no-referrer");

    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");

    // OWASP: sets X-Frame-Options to disallow iframes only to sites specified. Or are set via the AddAntiForgery middleware (used to be, aren't done anymore)
    // Optional: Can be done at a server level as well (IIS, Ingress, ... ) 
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
