using CodeVijetWeb.Services;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

/* Регистрация кастомного сервиса */
builder.Services.AddSingleton<WatchDogService>();
/* Добавление файла с настройками */
builder.Configuration.AddJsonFile("appsettings.json");

string s = builder.Configuration.GetValue<string>("Host");

 builder.WebHost.UseUrls(new string[] { s });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
