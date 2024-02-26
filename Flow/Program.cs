using Flow.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<FlowContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Connection");
    options.UseSqlServer(cs);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FlowContext>();
builder.Services.AddControllersWithViews();


// Add session support
builder.Services.AddSession(options =>
{
    // Configure session options if needed
});

var app = builder.Build();

// Custom middleware for handling 404 errors and redirecting to "/Home"
app.Use(async (context, next) =>
{
    await next();

    // If the response status code is 404 (Not Found)
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        // Redirect to "/Home"
        context.Request.Path = "/Home";
        await next();
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use session
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
