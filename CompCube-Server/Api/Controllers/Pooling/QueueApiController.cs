using CompCube_Models.Models.Map;
using CompCube_Server.Config;
using CompCube_Server.Data;
using CompCube_Server.Discord;
using Microsoft.AspNetCore.Mvc;

namespace CompCube_Server.Api.Controllers.Pooling;

[ApiController]
public class QueueApiController(ConfigHelper helper, MapQueue queue) : ControllerBase
{
    [HttpGet("/api/queue")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<VotingMap[]> GetQueue(string secret)
    {
        if (secret != helper.Secret)
            return Unauthorized();

        return queue.GetMaps().ToArray();
    }
}