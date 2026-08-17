using IbnSina.Application.DTOs;
using IbnSina.Application.Interfaces;
using IbnSina.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnSina.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(string? search)
    {
        var users = await _userRepository.GetAllAsync(search);
        if (!users.Any())
        {
            var message = !string.IsNullOrWhiteSpace(search)
                ? "No users found matching your search. Try different keywords."
                : "No users found.";
            return Ok(new { message });
        }
        var result = users.Select(u => new { u.Id, u.Name, u.Email, u.CreatedAt });
        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var result = new { user.Id, user.Name, user.Email, user.CreatedAt };
        return Ok(result);
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!IsPasswordStrong(dto.Password, out var passwordError))
            return BadRequest(new { message = passwordError });

        var hashedPassword = _passwordHasher.Hash(dto.Password);
        var user = new User(dto.Name, dto.Email, hashedPassword);

        await _userRepository.AddAsync(user);

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt
        });
    }
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found." });
        //if (user.Role == Role.SuperAdmin)
        //    return BadRequest(new { message = "SuperAdmin accounts cannot be deleted." });

        await _userRepository.DeleteAsync(user);
        return Ok(new { message = "User deleted successfully." });
    }
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("{id}/promote")]
    public async Task<IActionResult> PromoteToAdmin(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found." });

        if (user.Role != Role.User)
            return BadRequest(new { message = "Only plain User accounts can be promoted to Admin." });

        user.PromoteToAdmin();
        await _userRepository.UpdateAsync(user);

        return Ok(new { message = $"{user.Email} has been promoted to Admin." });
    }

    private bool IsPasswordStrong(string password, out string error)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            error = "Password must be at least 8 characters long.";
            return false;
        }
        if (!password.Any(char.IsUpper))
        {
            error = "Password must contain at least one uppercase letter.";
            return false;
        }
        if (!password.Any(char.IsLower))
        {
            error = "Password must contain at least one lowercase letter.";
            return false;
        }
        if (!password.Any(char.IsDigit))
        {
            error = "Password must contain at least one digit.";
            return false;
        }
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            error = "Password must contain at least one special character.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}