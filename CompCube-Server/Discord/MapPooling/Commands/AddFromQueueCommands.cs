using CompCube_Server.Data;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace CompCube_Server.Discord.MapPooling.Commands;

public class AddFromQueueCommands(MapData mapData, MapQueue queue) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("movefromqueue", "Add maps to the pool from the queue", Contexts = [InteractionContextType.Guild], DefaultGuildPermissions = Permissions.Administrator)]
    public async Task<InteractionMessageProperties> MoveFromQueue()
    {
        return "";
    }
}