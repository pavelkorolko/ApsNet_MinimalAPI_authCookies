using Classwork_11._02._24_auth_authorization_role_.Interfaces;
using Classwork_11._02._24_auth_authorization_role_.Models;

namespace Classwork_11._02._24_auth_authorization_role_.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>()
            {
                new User { Id = Guid.NewGuid(), Login = "Pavel", Email = "pavel@gmail.com", Role = Role.Admin },
                new User { Id = Guid.NewGuid(), Login = "Josh", Email = "josh@gmail.com", Role = Role.User },
                new User { Id = Guid.NewGuid(), Login = "John", Email = "john@gmail.com", Role = Role.Manager },
            };
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public Role GetRole(User u)
        {
            return _users.FirstOrDefault(e => e.Email == u.Email && e.Login == u.Login).Role;
        }

        public User GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(e => e.Email.Equals(email));
        }

        public User GetUserById(Guid id)
        {
            return _users.FirstOrDefault(e => e.Id == id);
        }

        public User GetUserByLogin(string login)
        {
            return _users.FirstOrDefault(e => e.Login.Equals(login));
        }

        public User GetUserByLoginAndEmail(string login, string email)
        {
            return _users.FirstOrDefault(e => e.Login.Equals(login) && e.Email.Equals(email));
        }

        public List<User> GetUsers()
        {
            return _users;
        }

        public void UpdateUser(User user)
        {
            var current = _users.FirstOrDefault(e => e.Id == user.Id);
            if(current != null)
            {
                _users.Remove(current);
                _users.Add(user);
            }
        }
    }
}
