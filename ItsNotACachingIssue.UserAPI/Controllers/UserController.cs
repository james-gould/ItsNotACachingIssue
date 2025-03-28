using ItsNotACachingIssue.Data;
using Microsoft.AspNetCore.Mvc;

namespace ItsNotACachingIssue.UserAPI.Controllers
{
    [Route("users")]
    public class UserController(UserDbContext context) : Controller
    {
        private readonly UserDbContext _context = context;

        [HttpPut("edit/{userId}/{updatedFirstName}")]
        public async Task<IActionResult> ChangeFirstName(
            [FromRoute] int userId,
            [FromRoute] string updatedFirstName)
        {
            ArgumentException.ThrowIfNullOrEmpty(updatedFirstName);

            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

            if(user is null)
                return NotFound($"Failed to find {nameof(User)} with userId: {userId}");

            user.FirstName = updatedFirstName;

            await _context.SaveChangesAsync();

            var contextId = _context.ContextId;

            return NoContent();
        }
    }
}
