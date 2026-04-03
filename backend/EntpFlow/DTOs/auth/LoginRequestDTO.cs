
using System.ComponentModel.DataAnnotations;
namespace EntpFlow.DTOs.auth;

public class LoginRequestDTO
{

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
    
}