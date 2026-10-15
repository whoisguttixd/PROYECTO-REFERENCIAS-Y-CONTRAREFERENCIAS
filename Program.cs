using PROYECTO_REFERENCIAS.Data;
using PROYECTO_REFERENCIAS.Repository;
using PROYECTO_REFERENCIAS.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Patrón Singleton: la configuración global del HRC vive en una única instancia.
builder.Services.AddSingleton<ConfiguracionHospitalariaSingleton>();

// Patrón Unit of Work: registro del contenedor de transacciones.
builder.Services.AddScoped<UnitOfWork>();

// DbContext: en desarrollo usa SQLite local; en producción (EDP) la cadena SQL Server del docente.
var useSqlite = builder.Configuration.GetValue<bool>("UsarSQLite", false);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useSqlite)
        options.UseSqlite(builder.Configuration.GetConnectionString("CadenaSQLite"));
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800;
});

// SESIONES
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//AUTENTICACIONES POR COOKIES
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
).AddCookie(options =>
{
    options.LoginPath = "/Login/Login";
    options.AccessDeniedPath = "/Login/AccesoDenegado";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
//Autenticaciones
app.UseAuthentication();
//evitar cache en paginas autenticadas
app.Use(async (context, next) =>
{
    if(context.User.Identity?.IsAuthenticated == true)
    {
        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        context.Response.Headers.Pragma = "no-cache";
        context.Response.Headers.Expires = "0";
    }
    await next();
});
//Autorizaciones
app.UseAuthorization();
//uso de sesiones
app.UseSession();

app.MapStaticAssets();

// Datos semilla realistas de Cajamarca
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DataSeeder.SeedAsync(db, force: false);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();