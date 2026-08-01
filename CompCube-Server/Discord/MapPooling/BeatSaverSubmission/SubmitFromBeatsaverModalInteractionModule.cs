using CompCube_Server.Api.BeatSaver;
using CompCube_Server.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class SubmitFromBeatsaverModalInteractionModule(IConfiguration config, RestClient client, BeatSaverApiWrapper beatSaverApi) : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatsaverModal")]
    public async Task SubmitMapFromBeatsaverModalInteraction()
    {
        var category = Context.Components.OfType<Label>().Select(i => i.Component).OfType<StringMenu>().First().SelectedValues?.FirstOrDefault();
        var bsr = Context.Components.OfType<Label>().Select(i => i.Component).OfType<TextInput>().FirstOrDefault()?.Value;
        var difficulty = Context.Components.OfType<Label>().Select(i => i.Component).OfType<StringMenu>().Last().SelectedValues?.FirstOrDefault();

        if (category is null)
        {
            await RespondUsingDms("Failed to get category from form!");
            return;
        }

        if (bsr is null)
        {
            await RespondUsingDms("Failed to get BSR from form!");
            return;
        }

        if (difficulty is null)
        {
            await RespondUsingDms("Failed to fetch difficulty from form!");
            return;
        }

        var beatSaverBeatmap = await beatSaverApi.GetBeatmapFromKey(bsr);

        if (beatSaverBeatmap is null)
        {
            await RespondUsingDms($"Failed to fetch beatmap {bsr} from beatsaver! (Are you sure this is the right key?)");
            return;
        }
        
        var poolingChannelId = config.GetSection("Discord").GetSection("PoolingChannelIds").GetValue<ulong>(category);

        var forumThread = await client.CreateForumGuildThreadAsync(poolingChannelId, new ForumGuildThreadProperties(beatSaverBeatmap.Name, new ForumGuildThreadMessageProperties
        {
            Embeds = [new EmbedProperties
            {
                Image = new EmbedImageProperties(beatSaverBeatmap.LatestVersion.CoverURL),
                Title = beatSaverBeatmap.Name,
                Description = beatSaverBeatmap.Description,
                Fields = [new EmbedFieldProperties
                    {
                        Name = "Map Link",
                        Value = $"https://beatsaver.com/maps/{bsr}"
                    },
                    new EmbedFieldProperties
                    {
                        Name = "Difficulty",
                        Value = difficulty
                    },
                    new EmbedFieldProperties()
                    {
                        Inline = true,
                        Name = "Category",
                        Value = category
                    }
                ]
            }]
        }));

        await SendEmptyResponse();
    }

    private async Task SendEmptyResponse() => await RespondAsync(InteractionCallback.DeferredModifyMessage);

    private async Task RespondUsingDms(MessageProperties message)
    {
        await RespondAsync(InteractionCallback.DeferredModifyMessage);

        var dmChannel = await Context.User.GetDMChannelAsync();

        await dmChannel.SendMessageAsync(message);
    }
}