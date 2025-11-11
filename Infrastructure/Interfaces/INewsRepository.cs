using Domain.Entities;
using Domain.Filters;

namespace Infrastructure.Interfaces;

public interface INewsRepository
{
    Task<int> CreateNewsAsync(News news);
    Task<int> UpdateNewsAsync(News news);
    Task<int> DeleteNewsAsync(News news);
    Task<News?> GetNewsByIdAsync(Guid newsId);
    Task<List<News>> GetAllNewsAsync();
}