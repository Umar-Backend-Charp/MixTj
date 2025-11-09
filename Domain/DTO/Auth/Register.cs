using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Auth;

public class Register
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Nickname { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    [Required]
    public required string Password { get; set; }
    
}