using IbnSina.Domain.Entities;

namespace IbnSina.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}