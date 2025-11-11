using System.Net;
using System.Text.Json;
using AutoMapper;
using Domain.DTO.News;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Caching;
using Infrastructure.Interfaces;
using Infrastructure.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;

namespace Infrastructure.Services;

public class NewsService(INewsRepository newsRepository, IMapper mapper, CacheService cacheService, IHttpContextAccessor accessor) : INewsService
{
    private async Task RefreshCache()
    {
        var allNews = await newsRepository.GetAllNewsAsync();
        var mapped = mapper.Map<List<GetNewsDto>>(allNews);
        await cacheService.AddAsync(CacheKeys.News, mapped, DateTimeOffset.Now.AddMinutes(5));
        Log.Information("Refreshed cache with key {k}", CacheKeys.News);
    }
    public async Task<Response<string>> CreateNewsAsync(CreateNewsDto dto)
    {
        Log.Information("Author with {id} tries to create a news", dto.AuthorId);
        var mappedNews = mapper.Map<News>(dto);
        var result = await newsRepository.CreateNewsAsync(mappedNews);

        if (result == 0)
        {
            Log.Warning("Author with {id} failed to created a news", dto.AuthorId);
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to create a news");
        }
        
        await RefreshCache();
        
        Log.Information("Author with id {id} successfully created a news", dto.AuthorId);
        return new Response<string>(HttpStatusCode.Created, "Created News Successfully");
    }

    public async Task<Response<string>> UpdateNewsAsync(UpdateNewsDto dto)
    {
        var userId = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        Log.Information("User with id {userId} tries to update the news with id {id}", userId, dto.Id);
        var theNews = await newsRepository.GetNewsByIdAsync(dto.Id);
        
        if (theNews is null)
        {
            Log.Warning("Not found the news");
            return new Response<string>(HttpStatusCode.NotFound, "News not found");
        }
        
        var mappedNews = mapper.Map<News>(dto);
        var result = await newsRepository.UpdateNewsAsync(mappedNews);

        if (result == 0)
        {
            Log.Warning("Failed to update the news");
            return new Response<string>(HttpStatusCode.NotFound, "Failed to update the news");
        }
        
        await RefreshCache();
        
        Log.Information("User with id {userId} updated the news with {id} successfully", userId, dto.Id);
        return new Response<string>(HttpStatusCode.OK, "Updated the news");
    }

    public async Task<Response<string>> DeleteNewsAsync(Guid newsId)
    {
        var userId = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        Log.Information("User with id {userID} tries to delete the news with id {id}", userId, newsId);
        var theNews = await newsRepository.GetNewsByIdAsync(newsId);
        if (theNews is null)
        {
            Log.Warning("Not found the news with id {id}", newsId);
            return new Response<string>(HttpStatusCode.NotFound, "News not found");
        }
        
        var result = await newsRepository.DeleteNewsAsync(theNews);

        if (result == 0)
        {
            Log.Warning("Failed to delete the news with id {id}", newsId);
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to delete news");
        }
        
        await RefreshCache();
        
        Log.Information("User with id {userId}deleted the news with id {id} successfully", userId, newsId);
        return new Response<string>(HttpStatusCode.OK, "Deleted News Successfully");
    }

    public async Task<Response<GetNewsDto>> GetNewsByIdAsync(Guid newsId)
    {
        var userId = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        Log.Information("User with id {userId} tries to get the news with id {id}", userId, newsId);
        var theNews = await newsRepository.GetNewsByIdAsync(newsId);
        if (theNews is null)
        {
            Log.Warning("Not found the news with id {id}", newsId);
            return new Response<GetNewsDto>(HttpStatusCode.NotFound, "News not found");
        }
        
        var mappedNews = mapper.Map<GetNewsDto>(theNews);
        
        Log.Information("Got the news with id {id} successfully", newsId);
        return new Response<GetNewsDto>(mappedNews);
    }

    public async Task<PaginationResponse<List<GetNewsDto>>> GetAllNewsAsync(NewsFilter filter)
    {
        var userId = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        Log.Information("User {userId} tries to get all the news", userId);
        
        var newsInCache = await cacheService.GetAsync<List<GetNewsDto>>(CacheKeys.News);

        List<News> newsList;
        
        if (newsInCache is not null)
        {
            newsList = mapper.Map<List<News>>(newsInCache);
            Log.Information("Retrieved news with key {k} from cache", CacheKeys.News);
        }
        else
        {
            Log.Information("Not found data in the cache with key {k}", CacheKeys.News);
            newsList = await newsRepository.GetAllNewsAsync();
            var mappedNewsList = mapper.Map<List<GetNewsDto>>(newsList);
            
            var expirationTime = DateTimeOffset.Now.AddMinutes(5);
            await cacheService.AddAsync(CacheKeys.News, mappedNewsList, expirationTime);
            Log.Information("Added to cache the news with key {key}", CacheKeys.News);
        }
        
        if (!string.IsNullOrEmpty(filter.Title))
        {
            newsList = newsList.Where(n => n.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(filter.AuthorId))
        {
            newsList = newsList.Where(n => n.AuthorId == filter.AuthorId).ToList();
        }

        if (filter.Category.HasValue)
        {
            newsList = newsList.Where(n => n.Category == filter.Category).ToList();
        }

        if (filter.Tags != null && filter.Tags.Length > 0)
        {
            newsList = newsList
                .Where(n => n.Tags != null && n.Tags.Any(t => filter.Tags.Contains(t)))
                .ToList();
        }

        var mappedFiltered = mapper.Map<List<GetNewsDto>>(newsList);
        
        
        await Console.Out.WriteLineAsync(new string('-', 50));
        Log.Information("Retrieved data with key {k} from database", CacheKeys.News);
        await Console.Out.WriteLineAsync(new string('-', 50));
        
        return new PaginationResponse<List<GetNewsDto>>(mappedFiltered, newsList.Count, filter.PageNumber, filter.PageSize);
    }
}