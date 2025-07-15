using EtkinlikProjem.Interfaces;
using EtkinlikProjem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // For .NET 6+
// or

// Session hizmetini ekleyin
//builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSession(); // .NET 6 ve sonrasý için
builder.Services.AddDistributedMemoryCache(); // Cache'i ekleyin (Session verisini tutacak)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturumun zaman aþýmý süresi
    options.Cookie.HttpOnly = true; // Güvenlik amacýyla sadece HTTP üzerinden eriþilebilir olsun
    options.Cookie.IsEssential = true; // Cookie'yi temel olarak iþaretle
});

var app = builder.Build();
// Session middleware'ini kullan
app.UseSession();
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





