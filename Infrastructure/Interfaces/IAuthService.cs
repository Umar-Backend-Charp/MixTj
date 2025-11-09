using Domain.DTO.Auth;
using Infrastructure.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<Response<string>> RegisterUserAsync(Register registerDto);
    Task<Response<string>> LoginAsync(Login loginDto);
}