using LoungeSaber_Server.Models;
using LoungeSaber_Server.SQL;
using Microsoft.AspNetCore.Mvc;

namespace LoungeSaber_Server.Api.Controllers;

[ApiController]
[Route("api/user/")]
public class GetUserController : ControllerBase
{
    [HttpGet("id/{userId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetUserFromId(string userId)
    {
        var user = Database.GetUserById(userId);

        if (user == null) return NotFound();
        return user;
    }

    [HttpGet("discord/{discordId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetUserFromDiscordId(string discordId)
    {
        var user = Database.GetUserByDiscordId(discordId);
        
        if (user == null) 
            return NotFound();
        
        return user;
    }
}