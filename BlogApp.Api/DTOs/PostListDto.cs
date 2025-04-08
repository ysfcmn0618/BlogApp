namespace BlogApp.Api.DTOs
{
    public class PostListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
        public string UserName { get; set; }
        public string CategoryName { get; set; }
    }
}
