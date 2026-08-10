using Studia.Application.Users;
using Studia.Domain.Users;

namespace Studia.WebApi;

public static class DbSeeder
{
    // Rompe el problema del huevo y la gallina: si crear un Profesor/Administrador
    // requiere estar logueado como Administrador, tiene que existir un Administrador
    // desde antes de que exista cualquier sesión. Se corre una vez por arranque y no
    // duplica si el admin configurado ya existe.
    public static void SeedDefaultAdmin(IServiceProvider services, IConfiguration configuration)
    {
        var userRepository = services.GetRequiredService<IUserRepository>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        var email = configuration["DefaultAdmin:Email"] ?? "admin@studia.local";
        var password = configuration["DefaultAdmin:Password"] ?? "Admin123!";

        var emailVo = Email.Create(email);
        if (userRepository.GetByEmail(emailVo) is not null)
            return;

        var passwordHash = passwordHasher.Hash(password);
        var admin = User.Register(emailVo, passwordHash, Role.Administrador, "Administrador");
        userRepository.Save(admin);

        Console.WriteLine($"[dev] Admin por defecto creado: {email}");
        Console.WriteLine("[dev] Contraseña definida en DefaultAdmin:Password (appsettings.Development.json). Cambiarla antes de cualquier despliegue real.");
    }
}
