using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkbookApi.DAL;
using WorkbookApi.DAL.Entities;
using WorkbookApi.Dtos;
using WorkbookApi.Security;

namespace WorkbookApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenBuilder _tokenBuilder; 
        private readonly WorkbookDbContext _context;

        public AuthController(WorkbookDbContext context, ITokenBuilder tokenBuilder)
        {
            _tokenBuilder = tokenBuilder;
            _context = context;
        }

        // GET: api/Auth
        [HttpPost("login")]
        public async Task<ActionResult<UserWithTokenDto>> Login(LoginDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                throw new BadHttpRequestException("email/password aren't right");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || !(user.PasswordHash == request.Password))
            {
                throw new BadHttpRequestException("email/password aren't right");
            }

            var expiresIn = DateTime.Now.AddMinutes(45);
            var token = _tokenBuilder.Build(user.Email, expiresIn);

            var dto = new UserWithTokenDto { Email = user.Email, Username = user.Username, Token = token };  

            return Ok(dto);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterDto request)
        {
            var user = _context.Users.Add(new User(request.Email, request.Username, request.Password));
            await _context.SaveChangesAsync();

            return Ok(user);

        }

     
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
