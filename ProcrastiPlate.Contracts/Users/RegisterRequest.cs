using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Users;

public record RegisterRequest(
    [Required, MaxLength(100)] string FirstName,
    [Required, MaxLength(100)] string LastName,
    [Required, MaxLength(50)] string Username,
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MaxLength(8)] string Password
);
