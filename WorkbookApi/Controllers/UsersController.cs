using AspNetCore;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using WorkbookApi.DAL;
using WorkbookApi.Dtos;

[Route("api/users")]
[ApiController]

public class UsersController : ControllerBase
{
    private readonly WorkbookDbContext _context;

    public UsersController(WorkbookDbContext context)
    {
        _context = context;
    }

    [HttpPost("update")]
    public async Task<ActionResult<UserDataDto>> Update(UpdateDto request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == request.Id);

        if (user == null)
        {
            throw new BadHttpRequestException($"User with Id: {request.Id} not found");
        }

        user.Username = request.Username;
        if (!String.IsNullOrEmpty(request.ProfileImage))
        {
            user.ProfileImage = request.ProfileImage;
        }

        _context.SaveChanges();

        var dto = new UserDataDto { Id = user.Id, Email = user.Email, Username = user.Username, ProfileImage = user.ProfileImage };

        return Ok(dto);
    }
}