using System.Collections.Generic;
using System.Linq;

public static class BuildingQueries
{
    // BuildingDef Queries
    public static IEnumerable<BuildingDef> ByName(this IEnumerable<BuildingDef> BuildingDefs, string name)
    {
        return BuildingDefs.Where(bldg => bldg.name == name);
    }
    public static IEnumerable<BuildingDef> ByCategory(this IEnumerable<BuildingDef> BuildingDefs, string category)
    {
        return BuildingDefs.Where(bldg => bldg.category == category);
    }
    public static IEnumerable<BuildingDef> ByParent(this IEnumerable<BuildingDef> BuildingDefs, string parent)
    {
        return BuildingDefs.Where(bldg => bldg.parentName == parent);
    }
}