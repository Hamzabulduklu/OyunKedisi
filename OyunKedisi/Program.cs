using Microsoft.AspNetCore.Authentication.Cookies;
using OyunKedisi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<OyunKedisiDbContext>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";        // Giri� yap�lmayan isteklerde y�nlendirilecek URL
        options.LogoutPath = "/Users/Logout";      // ��k�� i�lemi URL'si
        options.Cookie.Name = "OyunKedisiAuth";    // �erez ad�
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        // istekler aras�nda sliding expiration yapmak i�in:
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
