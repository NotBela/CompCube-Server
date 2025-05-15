using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Api.Controllers;

[ApiController]
[Route("playlist")]
public class GetPlaylistController
{
    [HttpGet]
    public IResult GetPlaylist()
    {
        var playlistObject = new JObject
        {
            {"playlistTitle","LoungeSaber Maps"},
            {"", ""}
        };

        return Results.File(Encoding.UTF8.GetBytes(playlistObject.ToString()), "application/json", "loungesaber-playlist.bplist");
    }
}