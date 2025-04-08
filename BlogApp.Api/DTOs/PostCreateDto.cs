namespace BlogApp.Api.DTOs
{
    public class PostCreateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
    }
}
