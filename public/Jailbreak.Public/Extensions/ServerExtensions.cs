using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace Jailbreak.Public.Extensions;

public static class ServerExtensions
{
    /// <summary>
    ///   Get the current CCSGameRules for the server
    /// </summary>
    /// <returns></returns>
    public static CCSGameRules? GetGameRules()
    {
        CCSGameRulesProxy? proxy = Utilities
            .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
            .FirstOrDefault();

        if (proxy is null || !proxy.IsValid)
        {
            Console.WriteLine("[Jailbreak.Public.Extensions.ServerExtensions.GetGameRules] Could not find cs_gamerules entity");
        }

        return proxy?.GameRules;
    }

    public static CCSGameRulesProxy? GetGameRulesProxy()
    {
        CCSGameRulesProxy? proxy = Utilities
           .FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
           .FirstOrDefault();

        if (proxy is null || !proxy.IsValid)
        {
            Console.WriteLine("[Jailbreak.Public.Extensions.ServerExtensions.GetGameRulesProxy] Could not find cs_gamerules entity");
        }

        return proxy;
    }
}