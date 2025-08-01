using LoungeSaber_Server.SQL;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace LoungeSaber_Server.Discord.Commands;

public class MatchCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("matchinfo", "Get info about a match!")]
    public async Task<InteractionMessageProperties> MatchInfo(int matchId)
    {
        var match = MatchLog.Instance.GetMatch(matchId);

        if (match == null)
            return $"Could not find match with id {matchId}";

        var embed = await MatchInfoMessageFormatter.GetEmbed(match, "Match Info:", true);

        return new InteractionMessageProperties()
        {
            Embeds = [embed]
        };
    }
}