using Domain.DTO.News;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Responses;

namespace Infrastructure.Interfaces;

public interface INewsService
{
    Task<Response<string>> CreateNewsAsync(CreateNewsDto dto);
    Task<Response<string>> UpdateNewsAsync(UpdateNewsDto dto);
    Task<Response<string>> DeleteNewsAsync(Guid newsId);
    Task<Response<GetNewsDto>> GetNewsByIdAsync(Guid newsId);
    Task<PaginationResponse<List<GetNewsDto>>> GetAllNewsAsync(NewsFilter filter);
}