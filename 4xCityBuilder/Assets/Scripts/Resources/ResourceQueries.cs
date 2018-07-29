using System.Collections.Generic;
using System.Linq;

public static class ResourceQueries
{
    // ResourceDef Queries
    public static IEnumerable<ResourceDef> ByName(this IEnumerable<ResourceDef> ResourceDefs, string name)
    {
        return ResourceDefs.Where(res => res.name == name);
    }
    public static IEnumerable<ResourceDef> ByIndustry(this IEnumerable<ResourceDef> ResourceDefs, string industry)
    {
        return ResourceDefs.Where(res => res.industry == industry);
    }
    public static IEnumerable<ResourceDef> BySkill(this IEnumerable<ResourceDef> ResourceDefs, string skill)
    {
        return ResourceDefs.Where(res => res.skill == skill);
    }
    public static IEnumerable<ResourceDef> ByType(this IEnumerable<ResourceDef> ResourceDefs, string type)
    {
        return ResourceDefs.Where(res => res.types.Contains(type));
    }
    public static IEnumerable<ResourceDef> ByTypeSortedByTier(this IEnumerable<ResourceDef> ResourceDefs, string type)
    {
        IEnumerable < ResourceDef > theseDefs = ResourceDefs.Where(res => res.types.Contains(type));
        return theseDefs.OrderByDescending(x => x.tier).Reverse<ResourceDef>();
    }
    public static IEnumerable<ResourceDef> ByTypeSortedByTierMinTier(this IEnumerable<ResourceDef> ResourceDefs, string type, int minTier)
    {
        IEnumerable<ResourceDef> theseDefs = ResourceDefs.Where(res => (res.types.Contains(type)) && (res.tier >= minTier));
        return theseDefs.OrderByDescending(x => x.tier).Reverse<ResourceDef>();
    }
}