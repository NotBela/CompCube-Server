using CompCube_Models.Models.Map;

namespace CompCube_Server.Discord;

public class DiscordConfigHelper(IConfiguration config)
{
    private readonly Dictionary<string, ulong> _roleIds =
        config.GetSection("Discord").GetSection("RoleIds").AsEnumerable().TakeLast(config.GetSection("Discord").GetSection("RoleIds").AsEnumerable().ToArray().Length - 1).Select(i => new KeyValuePair<string,ulong>(i.Key["Discord:RoleIds:".Length..], ulong.Parse(i.Value ?? "0"))).ToDictionary();
    
    public ulong GetChannelForCategory(VotingMap.Category category)
    {
        return config.GetSection("Discord").GetSection("PoolingChannelIds").GetValue<ulong>(category.ToString());
    }

    public string[] GetCategoryOptionsFromRoles(ulong[]? roleIds)
    {
        if (roleIds == null)
            return [];
        
        if (roleIds.Contains(_roleIds["MasterPooler"]))
            return Enum.GetNames<VotingMap.Category>();

        return _roleIds.Where(i => _roleIds.ContainsValue(i.Value)).Select(i => i.Key).Where(i => i != "MasterPooler" && i != "GeneralPooler").ToArray();
    }
}