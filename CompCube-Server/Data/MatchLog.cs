using System.Globalization;
using CompCube_Models.Models.Match;
using Newtonsoft.Json;

namespace CompCube_Server.SQL;

public class MatchLog(UserData userData, IConfiguration configuration) : TableManager(configuration)
{
    private readonly Random _random = new();
    
    protected override void CreateInitialTables()
    {
        var command = Connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS matchLog (id INT NOT NULL PRIMARY KEY, winnerIds TEXT NOT NULL, loserIds TEXT NOT NULL, mmrExchange INT NOT NULL, prematureEnd BOOL NOT NULL, time TEXT NOT NULL)";
        command.ExecuteNonQuery();
    }

    public MatchResultsData? GetMatch(int id)
    {
        return null;
    }

    public void AddMatchToTable(MatchResultsData results)
    {
        
    }

    public int GetValidMatchId()
    {
        var idArr = new int[6];

        for (var i = 0; i < idArr.Length; i++)
            idArr[i] = _random.Next(0, 10);

        var id = int.Parse(string.Join("", idArr));

        if (IsMatchIdUsed(id)) 
            return GetValidMatchId();

        return id;
    }

    private bool IsMatchIdUsed(int matchId)
    {
        var match = GetMatch(matchId);

        return match != null;
    }
}