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
}