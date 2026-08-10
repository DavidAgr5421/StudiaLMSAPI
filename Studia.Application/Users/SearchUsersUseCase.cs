namespace Studia.Application.Users;

public class SearchUsersUseCase(IUserRepository userRepository) : ISearchUsersUseCase
{
    public IReadOnlyCollection<UserResult> Execute(SearchUsersQuery query) =>
        userRepository.Search(query.Query)
            .Select(UserResult.FromDomain)
            .ToList();
}
