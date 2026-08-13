// IbnSina.Infrastructure/Services/BCryptPasswordHasher.cs
using IbnSina.Application.Interfaces;
using BCrypt.Net;

namespace IbnSina.Infrastructure.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool Verify(string plainPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
    }
}