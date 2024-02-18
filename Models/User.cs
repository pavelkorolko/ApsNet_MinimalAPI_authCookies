namespace Classwork_11._02._24_auth_authorization_role_.Models
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Login { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
