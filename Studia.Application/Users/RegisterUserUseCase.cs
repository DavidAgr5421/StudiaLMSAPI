using Studia.Domain.Users;

namespace Studia.Application.Users;

public class RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher) : IRegisterUserUseCase
{
    public UserResult Execute(RegisterUserCommand command)
    {
        var email = Email.Create(command.Email);

        if (userRepository.GetByEmail(email) is not null)
            throw new InvalidOperationException($"Ya existe un usuario registrado con el email '{email}'.");

        var passwordHash = passwordHasher.Hash(command.Password);
        var user = User.Register(email, passwordHash, command.Role, command.Name);

        userRepository.Save(user);

        return UserResult.FromDomain(user);
    }
}
