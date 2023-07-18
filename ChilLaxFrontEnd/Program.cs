using ChilLaxFrontEnd.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ChilLaxContext>(
      options => options.UseSqlServer(
      builder.Configuration.GetConnectionString("ChilLax")));

// �[�J�o�q�]�w�A�N�i�H�bControllers���غc�禡�`�J������
builder.Services.AddDbContext<ChilLaxContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("ChilLaxConnection")
    ));  // �إߤ@�� ChilLaxContext ����X��

var app = builder.Build();

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
