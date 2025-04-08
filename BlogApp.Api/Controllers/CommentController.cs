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
    public class CommentController(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        // Create - Yeni yorum ekleme
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateDto commentDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                Content = commentDto.Content,
                CreatedAt = DateTime.Now,
                PostId = commentDto.PostId,
                UserId = userId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, comment);
        }

        // Read - Tüm yorumları listeleme
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .ToListAsync();

            var commentDtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                UserName = c.User.UserName,
                PostTitle = c.Post.Title
            }).ToList();

            return Ok(commentDtos);
        }

        // Read - ID'ye göre yorum getirme
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetCommentById(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                UserName = comment.User.UserName,
                PostTitle = comment.Post.Title
            };

            return Ok(commentDto);
        }

        // Update - Yorum güncelleme
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentCreateDto commentDto)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            comment.Content = commentDto.Content;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Delete - Yorum silme
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
