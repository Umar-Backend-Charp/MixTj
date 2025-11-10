using Domain.DTO.News;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Responses;

namespace Infrastructure.Interfaces;

public interface INewsRepositoryService
{
    Task<Response<string>> CreateNewsAsync(CreateNewsDto dto);
    Task<Response<string>> UpdateNewsAsync(CreateNewsDto dto);
    Task<Response<string>> DeleteNewsAsync(Guid newsId);
    Task<Response<string>> GetNewsByIdAsync(Guid newsId);
    Task<Response<string>> GetAllNewsAsync(NewsFilter filter);
}