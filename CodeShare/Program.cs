using Blazored.LocalStorage;
using CodeShare.Background.Tasks;
using CodeShare.History.Storage;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
// track
/* Регистрация кастомного сервиса */
builder.Services.AddSingleton<WatchDogService>();
builder.Services.AddSingleton<HistoryManager>();
     
/* Добавление файла с настройками */
builder.Configuration.AddJsonFile("appsettings.json");

/* Настраиваем сервер для доступа во внешнюю сеть,
 * Указываем на каком порте работать из конфиг файла */
builder.WebHost.ConfigureKestrel(
    options => options.ListenAnyIP(builder.Configuration.GetValue<int>("HostPort")));

builder.Services.AddMudServices();

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