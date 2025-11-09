using Domain.DTO.Auth;
using Infrastructure.Interfaces;
using Infrastructure.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<Response<string>> Register(Register model)
        => await authService.RegisterUserAsync(model);
    
    [HttpPost("login")]
    public async Task<Response<string>> Login(Login model)
        => await authService.LoginAsync(model);
}