using System.Globalization;
using LoungeSaber_Server.Gameplay.Match;
using LoungeSaber_Server.Gameplay.Matchmaking;
using LoungeSaber_Server.Models.Match;
using LoungeSaber_Server.Models.Packets.ServerPackets;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace LoungeSaber_Server.Discord.Events;

public class MatchCompletedMessageManager
{
    public static readonly MatchCompletedMessageManager Instance = new();

    private TextChannel? _channel;

    public void Init(TextChannel channel)
    {
        if (_channel != null) 
            return;
        
        _channel = channel;
        
        Matchmaker.OnMatchStarted += OnMatchStarted;
    }

    public void Stop()
    {
        Matchmaker.OnMatchStarted -= OnMatchStarted;
    }
    
    private void OnMatchStarted(Match match) => match.OnMatchEnded += OnMatchEnded;

    private async void OnMatchEnded(MatchResultsData results, Match match)
    {
        try
        {
            match.OnMatchEnded -= OnMatchEnded;

            if (results.Premature)
                return;

            if (_channel == null) 
                return;

            MessageProperties message = "";

            var embed = new EmbedProperties
            {
                Title = "Match results:",
                Description = "",
                Fields =
                [
                    new()
                    {
                        Name = "Winner",
                        Value = $"{results.Winner.User.Username} - {(results.Winner.RelativeScore * 100).ToString("F2" , CultureInfo.InvariantCulture)}% {(results.Winner.FC ? "FC" : $"{results.Winner.Misses}x")} ({results.Winner.User.Mmr} -> {results.Winner.User.Mmr + results.MmrChange})",
                        Inline = true
                    },
                    new()
                    {
                        Name = "Loser",
                        Value = $"{results.Loser.User.Username} - {(results.Loser.RelativeScore * 100).ToString("F2" , CultureInfo.InvariantCulture)}% {(results.Loser.FC ? "FC" : $"{results.Loser.Misses}x")} {(results.Loser.ProMode ? "[PM]" : "")} ({results.Loser.User.Mmr} -> {results.Loser.User.Mmr - results.MmrChange})"
                    }
                ],
                Timestamp = DateTime.Now,
            };

            message.Embeds = [embed];

            await _channel.SendMessageAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}