using System.Drawing;
using LoungeSaber_Server.Models.Client;
using LoungeSaber_Server.SQL;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Color = NetCord.Color;

namespace LoungeSaber_Server.Discord.Commands;

public class UserCommands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("link", "Link your discord account to your LoungeSaber profile!")]
    public string Link(string scoresaberId)
    {
        if (UserData.Instance.GetUserByDiscordId(Context.User.Id.ToString()) != null)
            return "This user is already linked to a discord account.";

        var userData = UserData.Instance.GetUserById(scoresaberId);
        
        if (userData == null)
            return "This user does not have a LoungeSaber account yet!";
        
        UserData.Instance.LinkDiscordToUser(userData.UserId, Context.User.Id.ToString());

        return $"Successfully linked discord account {Context.User.Username} to user {userData.Username}";
    }

    [SlashCommand("profile", "View the profile of yourself or another user")]
    public void ProfileByUser(User? user = null, string? id = null)
    {
        if (id != null)
        {
            Context.Interaction.SendResponseAsync(InteractionCallback.Message(GetUserProfileMessage(UserData.Instance.GetUserById(id))));
            return;
        }
        
        if (user == null)
            user = Context.User;
        
        var userProfile = UserData.Instance.GetUserByDiscordId(user.Id.ToString());
        
        Context.Interaction.SendResponseAsync(InteractionCallback.Message(GetUserProfileMessage(userProfile)));
    }

    private InteractionMessageProperties GetUserProfileMessage(UserInfo? userInfo)
    {
        InteractionMessageProperties message = "";
        var embed = new EmbedProperties();
        message.Embeds = [embed];

        if (userInfo == null)
        {
            embed.Description = "This user is not linked to a LoungeSaber profile.";
            return message;
        }

        embed.Title = userInfo.Username;
        
        if (userInfo.Badge != null)
            embed.Color = ParseColor(userInfo.Badge.ColorCode);
        
        embed.Fields = 
        [
            new()
            {
                Name = "MMR",
                Value = userInfo.Mmr.ToString(),
                Inline = true
            },
            new()
            {
                Name = "Rank",
                Value = userInfo.Rank.ToString(),
                Inline = true
            }
        ];

        return message;
    }

    private Color ParseColor(string colorCode)
    {
        var drawingColor = ColorTranslator.FromHtml(colorCode);
        
        return new Color(drawingColor.R, drawingColor.G, drawingColor.B);
    }
}