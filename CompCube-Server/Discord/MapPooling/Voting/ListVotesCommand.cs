using CompCube_Server.Extensions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class ListVotesCommand(VoteCalculator voteCalculator) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("votes", "List the votes on a map thread")]
    public async Task<InteractionMessageProperties> ListVotes()
    {
        var votes = await voteCalculator.GetUpvotesFromThread(Context.Channel.Id);
        
        return new InteractionMessageProperties()
        {
            
            Embeds = [
                new EmbedProperties()
                {
                    Fields = [
                        new EmbedFieldProperties()
                        {
                            Name = "🗣️ Submission Owner",
                            Value = Context.User.GetMention()
                        },
                        new EmbedFieldProperties()
                        {
                            Name = "👍 Upvotes",
                            Value = $"{string.Join(", ", votes.Upvotes.Select(i => i.GetMention()))}\n"
                        },
                        new EmbedFieldProperties()
                        {
                            Name = "👎 Downvotes",
                            Value = $"{string.Join(", ", votes.Downvotes.Select(i => i.GetMention()))}\n"
                        }
                    ]
                }
            ]
        };
    }
}