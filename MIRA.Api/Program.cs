using Dapper;
using Microsoft.OpenApi.Models;
using MIRA.Api.Configuracion;
using MIRA.Api.Repositorios;
using MIRA.Api.Servicios;

// Habilitar mapeo automático de columnas con guiones bajos (snake_case) a propiedades PascalCase en Dapper
DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

// Configuración de controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MIRA API",
        Version = "v0.2",
        Description = "Módulo de Investigación para la Gestión de Proyectos Académicos - USB Medellín (Entrega 2: Entidades Relacionales y Tablas Puente)"
    });
});

// Registro de factoría de conexiones a PostgreSQL
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

// --- Registro de Repositorios (Inyección de Dependencias) ---
// Repositorios v0.1 (Tablas maestras)
builder.Services.AddScoped<IAreaConocimientoRepository, AreaConocimientoRepository>();
builder.Services.AddScoped<IObjetivoDesarrolloSostenibleRepository, ObjetivoDesarrolloSostenibleRepository>();
builder.Services.AddScoped<IAreaAplicacionRepository, AreaAplicacionRepository>();
builder.Services.AddScoped<ITerminoClaveRepository, TerminoClaveRepository>();
builder.Services.AddScoped<IUniversidadRepository, UniversidadRepository>();
builder.Services.AddScoped<ILineaInvestigacionRepository, LineaInvestigacionRepository>();

// Repositorios v0.2 (Entidades relacionales y tablas puente)
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IGrupoInvestigacionRepository, GrupoInvestigacionRepository>();
builder.Services.AddScoped<ISemilleroRepository, SemilleroRepository>();
builder.Services.AddScoped<IRelacionesRepository, RelacionesRepository>();

// --- Registro de Servicios de Lógica de Negocio (Inyección de Dependencias) ---
// Servicios v0.1 (Tablas maestras)
builder.Services.AddScoped<IAreaConocimientoService, AreaConocimientoService>();
builder.Services.AddScoped<IObjetivoDesarrolloSostenibleService, ObjetivoDesarrolloSostenibleService>();
builder.Services.AddScoped<IAreaAplicacionService, AreaAplicacionService>();
builder.Services.AddScoped<ITerminoClaveService, TerminoClaveService>();
builder.Services.AddScoped<IUniversidadService, UniversidadService>();
builder.Services.AddScoped<ILineaInvestigacionService, LineaInvestigacionService>();

// Servicios v0.2 (Validación de integridad referencial)
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<IGrupoInvestigacionService, GrupoInvestigacionService>();
builder.Services.AddScoped<ISemilleroService, SemilleroService>();
builder.Services.AddScoped<IRelacionesService, RelacionesService>();

var app = builder.Build();

// Habilitar Swagger en desarrollo y entorno local
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MIRA API v0.2");
    });
}

app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.Run();