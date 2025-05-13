using LoungeSaber_Server.Models;
using LoungeSaber_Server.Models.Networking;

namespace LoungeSaber_Server.SkillDivision.TournamentRoom;

public class TournamentRoom
{
    public readonly Division Division;
    
    public List<ConnectedUser> ConnectedUsers = [];
    
    public TournamentRoom(Division division)
    {
        Division = division;
    }
    
    public bool CanJoinRoom(int mmr) => mmr >= Division.MinMMR && mmr < Division.MaxMMR;

    public bool JoinRoom(ConnectedUser user)
    {
        if (!CanJoinRoom(user.UserInfo.MMR)) return false;
        
        ConnectedUsers.Add(user);
        return true;
    }
}