namespace LoungeSaber_Server.Models;

public class User(string id, int mmr, string? discordId = null)
{
    public string ID { get; private set; } = id;
    public int MMR { get; private set; } = mmr;

    public string? DiscordId { get; private set; } = discordId;
}