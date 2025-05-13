using System.Drawing;
using Newtonsoft.Json;
using LoungeSaber_Server.SkillDivision.TournamentRoom;

namespace LoungeSaber_Server.SkillDivision;

public class Division
{
    public readonly int MinMMR;
    public readonly int MaxMMR;

    public readonly string DivisionName;
    public readonly Color DivisionColor;

    public readonly TournamentRoom.TournamentRoom DivisionRoom;

    private Division(int minMMR, int maxMMR, string divisionName, Color divisionColor)
    {
        MinMMR = minMMR;
        MaxMMR = maxMMR;
        DivisionName = divisionName;
        DivisionColor = divisionColor;

        DivisionRoom = new TournamentRoom.TournamentRoom(this);
    }

    public static Division Parse(string json)
    {
        var division = JsonConvert.DeserializeObject<Division>(json);
        
        if (division == null) 
            throw new Exception("Could not deserialize division from config!");
        
        return division;
    }
}