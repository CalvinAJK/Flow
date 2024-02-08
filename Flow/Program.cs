using Flow.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<FlowContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Connection");
    options.UseSqlServer(cs);
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
