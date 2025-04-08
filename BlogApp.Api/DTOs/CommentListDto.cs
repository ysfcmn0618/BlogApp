namespace BlogApp.Api.DTOs
{
    public class CommentListDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string CreatedAt { get; set; }
    }
}
