namespace ProcrastiPlate.Api.Models;

public class Author
{
    public int AuthorId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Website { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public byte[]? Headshot { get; set; }
}