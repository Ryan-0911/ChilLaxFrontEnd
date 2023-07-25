using CoreMVC_SignalR_Chat.Hubs;
using ChilLaxFrontEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddDbContext<ChilLaxContext>(
      options => options.UseSqlServer(
      builder.Configuration.GetConnectionString("ChilLax")));

builder.Services.AddSession();

string MyAllowOrign = "AllowAny";
builder.Services.AddCors(options => {
    options.AddPolicy(
    name: MyAllowOrign,
    policy => policy.WithOrigins("*").WithHeaders("*").WithMethods("*")
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");
app.Run();