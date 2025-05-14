namespace LoungeSaber_Server.Models;

public class Map
{
    public Difficulties Difficulty { get; private set; }
    public string SongName { get; private set; }
    public string SongArtist { get; private set; }
    public string Mapper { get; private set; }
    public string Hash { get; private set; }

    private Map(Difficulties difficulty, string songName, string songArtist, string mapper)
    {
        Difficulty = difficulty;
        SongName = songName;
        SongArtist = songArtist;
        Mapper = mapper;
    }

    //TODO: add map info parsing
    public static Map Parse(string json)
    {
        return new Map(Difficulties.ExpertPlus, "testName", "testArtist", "testMapper");
    }
    
    
    public enum Difficulties
    {
        Easy,
        Normal,
        Hard,
        Expert,
        ExpertPlus
    }
    
    
    public enum MapTypes
    {
        Acc,
        Speed,
        Tech,
        Midspeed,
        Balanced,
        Extreme
    }
}