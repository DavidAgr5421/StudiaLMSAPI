namespace Studia.Application.Users;

public interface ISearchUsersUseCase
{
    IReadOnlyCollection<UserResult> Execute(SearchUsersQuery query);
}
