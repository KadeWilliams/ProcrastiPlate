namespace ProcrastiPlate.Contracts.Users;

public record UserProfileReponse(
    int UserId,
    string Username,
    string Email,
    string Token, // JWT
    DateTime TokenExpires
);