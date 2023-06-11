﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkbookApi.DAL;
using WorkbookApi.DAL.Entities;
using WorkbookApi.Dtos;

namespace WorkbookApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly WorkbookDbContext _context;

        public AuthController(WorkbookDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth
        [HttpPost("login")]
        public async Task<ActionResult<UserDataDto>> Login(LoginDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                throw new BadHttpRequestException("email/password aren't right");
            }   
            
            // Validate password
            if (string.IsNullOrWhiteSpace(request.Password) || !(user.PasswordHash == request.Password))
            {
                throw new BadHttpRequestException("email/password aren't right");
            }

            var dto = new UserDataDto { Id = user.Id, Email = user.Email, Username = user.Username, ProfileImage = user.ProfileImage };  

            return Ok(dto);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDataDto>> Register(RegisterDto request)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (existingUser != null)
            {
                throw new BadHttpRequestException("Пользователь с такой почтой существует");
            }

            var newUser = new User()
            {
                Email = request.Email,
                Username = request.Username,
                PasswordHash = request.Password,
                ProfileImage = "data:image/jpeg;base64,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",
                IsVerified = false,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok();
        }
    }
}
