using Microsoft.EntityFrameworkCore;
using VetSync.Application;
using VetSync.Infrastructure;
using VetSync.Infrastructure.Data.Context;
using VetSync.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de capas de Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Servicios de presentacion
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Aplicar migraciones pendientes al iniciar la aplicacion
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<VetSyncDbContext>();
    dbContext.Database.Migrate();
}

// Middleware global de manejo de excepciones
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorPages().WithStaticAssets();

app.Run();
