using Domain.Entities;
using Domain.Filters;
using Infrastructure.Data.DataContext;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NewsRepository(AppDbContext context) : INewsRepository
{
    public async Task<int> CreateNewsAsync(News news)
    {
        await context.AddAsync(news);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateNewsAsync(News news)
    {
        context.Update(news);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteNewsAsync(News news)
    {
        news.IsDeleted = true;
        return await context.SaveChangesAsync();
    }

    public async Task<News?> GetNewsByIdAsync(Guid newsId)
    {
        return await context.News.FindAsync(newsId);
    }

    public async Task<List<News>> GetAllNewsAsync()
    {
        return await context.News.ToListAsync();
    }
}