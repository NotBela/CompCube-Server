using CompCube_Models.Models.Map;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class MapPoolButtonInteractionModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatsaverButton")]
    public async Task SubmitMapFromBeatSaverInteraction()
    {
        var enumValues = Enum.GetValues<VotingMap.Category>().Select(i => new StringMenuSelectOptionProperties(i.ToString(), i.ToString()));
        
        var modal = new ModalProperties("submitMapFromBeatsaverModal", "Submit From BeatSaver")
        {
            Components = [new StringMenuProperties("categoryMenu", enumValues)]
        };
        
        await Context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal));
    }
}