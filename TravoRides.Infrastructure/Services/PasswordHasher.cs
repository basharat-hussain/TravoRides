using Microsoft.AspNetCore.Identity;
using TravoRiders.Application.Interfaces.Services;

namespace AlArwaSolutions.Infrastructure.Services
{
    public class PasswordHasher:IPasswordHasher
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHasher()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(
                new object(),
                password);
        }

        public bool VerifyPassword(
            string password,
            string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
