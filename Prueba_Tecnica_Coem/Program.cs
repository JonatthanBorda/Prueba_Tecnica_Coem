using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Prueba_Tecnica_Coem.Models;

async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = ["Demandante", "Empleador"];
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Crea el rol si no existe
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbPortalCoemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conection")));

// Configuración de ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuración de la política de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    // ... otras opciones ...

    // Configuración de bloqueo
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    // ... otras opciones ...
})
.AddEntityFrameworkStores<DbPortalCoemContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    //options.LoginPath = "/Usuarios/Login"; // o la ruta de tu elección
    //options.LogoutPath = "/Usuarios/Login"; // o la ruta de tu elección
    options.AccessDeniedPath = "/Usuarios/AccesoDenegado"; // o la ruta de tu elección
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Crear roles en el inicio de la aplicación
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await CreateRoles(serviceProvider);
}

app.Run();
