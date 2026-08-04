using CompCube_Models.Models.Map;

namespace CompCube_Server.Discord;

public class DiscordConfigHelper(IConfiguration config)
{
    private Dictionary<VotingMap.Category, ulong> _roleIds => Enum.GetValues<VotingMap.Category>().Where(i => i != VotingMap.Category.Special).Select(i => new KeyValuePair<VotingMap.Category, ulong>(i, config.GetSection("Discord").GetSection("RoleIds").GetValue<ulong>(i.ToString()))).ToDictionary();
    public Dictionary<VotingMap.Category, ulong> ForumChannels => Enum.GetValues<VotingMap.Category>().Select(i => new KeyValuePair<VotingMap.Category,ulong>(i, config.GetSection("Discord").GetSection("ForumChannels").GetValue<ulong>(i.ToString()))).ToDictionary();

    public ulong MasterPoolerRoleId => config.GetSection("Discord").GetSection("RoleIds").GetValue<ulong>("MasterPooler");
    
    public ulong PoolerRoleId => config.GetSection("Discord").GetSection("RoleIds").GetValue<ulong>("Pooler");
    
    public ulong GuildId => config.GetSection("Discord").GetValue<ulong>("GuildId");

    public VotingMap.Category[] GetCategoriesFromRoles(ulong[]? roleIds)
    {
        if (roleIds == null)
            return [];

        if (roleIds.Contains(MasterPoolerRoleId))
            return Enum.GetValues<VotingMap.Category>();

        return _roleIds.Where(i => roleIds.Contains(i.Value)).Select(i => i.Key).ToArray();
    }
}