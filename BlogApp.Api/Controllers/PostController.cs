using BlogApp.Api.Data;
using BlogApp.Api.DTOs;
using BlogApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateDto post)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
            {
                return Unauthorized();
            }
            var newPost = new Post
            {
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostById), new { id = newPost.Id }, newPost);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostListDto>>> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .ToListAsync();

            var postDtos = posts.Select(p => new PostListDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                UserName = p.User.UserName,
                CategoryName = p.Category.Name
            }).ToList();
            return Ok(postDtos);
        }

        [HttpGet("{int:id}")]
        public async Task<ActionResult<PostListDto>> GetPostById(int id)
        {

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post is null) return NotFound();

            var postDto = new PostListDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                UserName = post.User.UserName,
                CategoryName = post.Category.Name
            };
            return Ok(postDto);
        }

        [HttpPut("{int:id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateDto post)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
            {
                return Unauthorized();
            }
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost is null)
            {
                return NotFound();
            }
            if (existingPost.UserId != userId)
            {
                return Forbid();
            }
            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.CategoryId = post.CategoryId;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{int:id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null)
            {
                return Unauthorized();
            }
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost is null)
            {
                return NotFound();
            }
            if (existingPost.UserId != userId)
            {
                return Forbid();
            }
            _context.Posts.Remove(existingPost);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
