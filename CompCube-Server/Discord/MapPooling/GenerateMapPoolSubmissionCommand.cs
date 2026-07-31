using NetCord;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class GenerateMapPoolSubmissionCommand() : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("createmapsubmissionmessage", "Create a new map submission message", DefaultGuildPermissions = Permissions.Administrator)]
    public InteractionMessageProperties CreateMapSubmissionMessage()
    {
        return new InteractionMessageProperties()
        {
            Embeds =
            [
                new EmbedProperties
                {
                    Title = "Pool Submission",
                    Description = "test",
                }
            ],
            Components =
            [
                new ActionRowProperties()
                {
                    new ButtonProperties("submitMapFromBeatSaverButton", "Submit from BeatSaver", ButtonStyle.Primary)
                }
            ]
        };
    }
}