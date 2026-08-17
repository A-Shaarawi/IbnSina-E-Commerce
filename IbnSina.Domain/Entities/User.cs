namespace IbnSina.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public Role Role { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string HashedPassword { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutEnd { get; private set; }
        public bool IsBlocked { get; private set; }
        public DateTime CreatedAt { get; private set; }
        private User() { }
        public User(string name, string email, string hashedPassword)
        {
            SetName(name);
            Role = Role.User;
            SetEmail(email);
            SetHashedPassword(hashedPassword);
            CreatedAt = DateTime.UtcNow;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            Name = name;
        }
        public void PromoteToAdmin()
        {
            Role = Role.Admin;
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            var atIndex = email.IndexOf('@');
            var dotIndex = email.LastIndexOf('.');

            if (atIndex <= 0 || dotIndex < atIndex + 2 || dotIndex == email.Length - 1)
                throw new ArgumentException("Email is not a valid email address.", nameof(email));

            Email = email;
        }

        private void SetHashedPassword(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));
            HashedPassword = hashedPassword;
        }
        public void RegisterFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 6)
            {
                IsBlocked = true;
                LockoutEnd = null;
            }
            else if (FailedLoginAttempts >= 5)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(3);
            }
            else if (FailedLoginAttempts >= 3)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(1);
            }
        }
        public void RegisterSuccessfulLogin()
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            IsBlocked = false;
        }

        public bool IsCurrentlyLockedOut()
        {
            return IsBlocked || (LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow);
        }

        public void Unblock()
        {
            IsBlocked = false;
            LockoutEnd = null;
            FailedLoginAttempts = 0;
        }
    }
}
