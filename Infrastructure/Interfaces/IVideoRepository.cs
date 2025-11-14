using Domain.DTO.Video;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IVideoService
{
    Task<int> CreateVideoAsync(Video dto);
    Task<int> UpdateVideoAsync(Video dto);
    Task<int> DeleteVideoAsync(Video dto);
    Task<Video> GetVideoByIdAsync(Guid videoId);
    Task<List<Video>> GetAllVideosAsync();
}