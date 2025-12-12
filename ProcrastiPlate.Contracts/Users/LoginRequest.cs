using System.ComponentModel.DataAnnotations;

namespace ProcrastiPlate.Contracts.Users;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
