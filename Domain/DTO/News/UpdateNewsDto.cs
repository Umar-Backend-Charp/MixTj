using Domain.Enums;

namespace Domain.DTO.News;

public class UpdateNewsDto
{
    public Guid Id { get; set; }
    public required string AuthorId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public Category Category { get; set; }
    public string[]? Tags { get; set; }
}