using EventHub.MinimalApi.Models;

namespace EventHub.MinimalApi.Dal
{
    public interface IAuthenticationRepository
    {
        int RegisterUser(User user);
        User? CheckCredentials(User user);
        List<User> GetUsers();

        string GetUserRole(int roleId);
    }
}
