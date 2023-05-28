namespace WorkbookApi.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsVerified { get; set; }

        public User(string email, string username, string passwordHash, bool isVerified = false)
        {
            Email = email;
            Username = username;
            PasswordHash = passwordHash;
            IsVerified = isVerified;
        }
    }
}
