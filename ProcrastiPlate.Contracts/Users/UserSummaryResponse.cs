namespace ProcrastiPlate.Contracts.Users;

public record UserSummaryResponse(
    int UserId,
    string Username,
    string FullName,
    string? ProfilePictureUrl
);
