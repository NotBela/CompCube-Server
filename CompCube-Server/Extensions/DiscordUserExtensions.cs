using NetCord;

namespace CompCube_Server.Extensions;

public static class DiscordUserExtensions
{
    public static string GetMention(this User user) => $"<@{user.Id}>";
    
}