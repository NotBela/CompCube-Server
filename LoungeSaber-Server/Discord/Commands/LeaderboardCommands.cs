using LoungeSaber_Server.SQL;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using RomanNumerals;

namespace LoungeSaber_Server.Discord.Commands;

public class LeaderboardCommands(UserData userData) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("leaderboard", "leaderboard")]
    public InteractionMessageProperties Leaderboard(int page = 0)
    {
        var embed = new EmbedProperties();

        var leaderboard = userData.GetLeaderboardRange(page, page + 10);

        embed.Title = $"Leaderboard (Page {page}): ";
        var leaderboardString = "";

        foreach (var user in leaderboard)
        {
            var mmrString = $"{user.Mmr} MMR ({user.Division.Division} {new RomanNumeral(user.Division.SubDivision)})";

            var lineString = "\n" + user.Username + " " + 
                             string.Concat(Enumerable.Repeat("-", Math.Max(1, 50 - user.Username.Length))) + " " + mmrString;
            leaderboardString += lineString;
        }

        embed.Description = leaderboardString;

        return new InteractionMessageProperties
        {
            Embeds = [embed]
        };
    }
}