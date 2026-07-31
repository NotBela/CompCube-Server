using CompCube_Models.Models.Map;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class MapPoolButtonInteractionModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatSaverButton")]
    public async Task SubmitMapFromBeatSaverInteraction()
    {
        var options = Enum.GetNames<VotingMap.Category>().Select(i => new StringMenuSelectOptionProperties(i, i));
        
        var modal = new ModalProperties("submitMapFromBeatsaverModal", "Submit From BeatSaver")
        {
            Components = [
                new LabelProperties("Category", new StringMenuProperties("category", options)),
                new LabelProperties("BSR", new TextInputProperties("bsr", TextInputStyle.Short)),
            ]
        };
        
        
        await Context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal));
    }
}