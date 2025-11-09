using System.ComponentModel.DataAnnotations;

namespace Domain.DTO.Auth;

public class Login
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }
}