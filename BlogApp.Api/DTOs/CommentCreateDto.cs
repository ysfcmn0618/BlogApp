namespace BlogApp.Api.DTOs
{
    public class CommentCreateDto
    {
        public string Content { get; set; }
        public int PostId { get; set; }
    }
}
