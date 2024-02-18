using Classwork_11._02._24_auth_authorization_role_.Models;

namespace Classwork_11._02._24_auth_authorization_role_.Interfaces
{
    public interface IUserRepository
    {
        User GetUserById(Guid id);
        User GetUserByEmail(string email);
        User GetUserByLogin(string login);
        User GetUserByLoginAndEmail(string login, string email);
        Role GetRole(User user);
        void AddUser(User user);
        List<User> GetUsers();
        void UpdateUser(User user);
    }
}
