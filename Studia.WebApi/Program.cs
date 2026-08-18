using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Studia.Application.Auth;
using Studia.Infrastructure.Auth;
using Studia.Infrastructure.Persistence.EfCore;
using Studia.WebApi;
using Studia.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Serializa enums como texto (p.ej. "Abierta"), no como número -- mucho más cómodo para probar a mano en Postman.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// ---------- Manejo de errores ----------
builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------- CORS ----------
// Sin AllowCredentials(): el front autentica con un Bearer token en el header
// Authorization, no con cookies, así que no hace falta habilitar credenciales
// cross-origin (que además es incompatible con orígenes comodín).
const string frontendCorsPolicy = "StudiaFrontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddPolicy(frontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// ---------- Base de datos ----------
var connectionString = builder.Configuration.GetConnectionString("StudiaDb")
    ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:StudiaDb.");

builder.Services.AddDbContext<StudiaDbContext>(options => options.UseNpgsql(connectionString));

// ---------- Repositorios, puertos técnicos y casos de uso ----------
builder.Services.AddStudiaRepositories();
builder.Services.AddStudiaTechnicalPorts();
builder.Services.AddStudiaUseCases();

// El secreto NUNCA va en appsettings.json. Viene de la variable de entorno Jwt__SigningSecret
// (el doble guion bajo es cómo ASP.NET Core mapea env vars a claves de configuración anidadas,
// "Jwt:SigningSecret") o de "dotnet user-secrets" en desarrollo. Si no está configurado, se
// genera una clave aleatoria válida solo para esta ejecución -- las sesiones no sobreviven un reinicio.
var jwtSigningSecret = builder.Configuration["Jwt:SigningSecret"];
if (string.IsNullOrWhiteSpace(jwtSigningSecret))
{
    jwtSigningSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    Console.WriteLine("[dev] Jwt:SigningSecret no configurado; usando una clave aleatoria válida solo para esta ejecución.");
    Console.WriteLine("[dev] En producción esta clave debe venir de configuración/secrets manager, nunca generarse al vuelo.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Studia.LMS";
builder.Services.AddSingleton<IJwtTokenService>(_ => new JwtTokenService(jwtSigningSecret, jwtIssuer));

// ---------- Autenticación JWT ----------
// MapInboundClaims = false: el mismo fix del bug real que encontramos en la consola.
// Sin esto, "sub"/"email" se remapean a URIs largas de WS-Federation y las búsquedas por claim fallan en silencio.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            // Un JWT válido puede seguir siendo una sesión "cerrada" (logout) -- se verifica
            // contra la misma lista de revocación que usa Studia.Console.
            OnTokenValidated = context =>
            {
                var jti = context.Principal?.FindFirst("jti")?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    context.Fail("El token no tiene jti.");
                    return Task.CompletedTask;
                }

                var revokedTokenRepository = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenRepository>();
                if (revokedTokenRepository.GetByJti(jti) is not null)
                    context.Fail("La sesión fue cerrada. Inicie sesión nuevamente.");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Aplica migraciones pendientes al arrancar -- en Render no hay una forma
    // cómoda de correr "dotnet ef database update" a mano contra la base de
    // datos administrada, así que el propio servicio se encarga al iniciar.
    scope.ServiceProvider.GetRequiredService<StudiaDbContext>().Database.Migrate();
    DbSeeder.SeedDefaultAdmin(scope.ServiceProvider, app.Configuration);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors(frontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapAdminEndpoints();
app.MapCoursesEndpoints();
app.MapCohortsEndpoints();
app.MapUsersEndpoints();
app.MapEnrollmentsEndpoints();
app.MapSectionsEndpoints();
app.MapActivitiesEndpoints();
app.MapSubmissionsEndpoints();
app.MapNotificationsEndpoints();

app.Run();
