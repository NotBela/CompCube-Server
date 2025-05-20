using System.Drawing;
using LoungeSaber_Server.Models.Divisions;
using Color = LoungeSaber_Server.Models.Divisions.Color;

namespace LoungeSaber_Server.MatchRoom;

public static class DivisionManager
{
    public static Division[] Divisions { get; private set; } = [
        new(0, 1000, "Iron", new Color(150, 150, 150), []),
        new(1000, 2000, "Gold", new Color(211, 175, 55), [])
    ];

    public static bool TryGetDivisionFromName(string name, out Division? value)
    {
        value = Divisions.FirstOrDefault(x => x.DivisionName == name);
        return value != null;
    }
    
    // TODO: add division parsing from config
}