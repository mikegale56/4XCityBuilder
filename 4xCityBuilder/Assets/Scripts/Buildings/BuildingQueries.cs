using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public static IEnumerable<BuildingDef> ByCategoryNoParent(this IEnumerable<BuildingDef> BuildingDefs, string category)
    {
        return BuildingDefs.Where(bldg => ((bldg.category == category) && (string.IsNullOrEmpty(bldg.parentName))));
    }
    public static IEnumerable<BuildingDef> ByParent(this IEnumerable<BuildingDef> BuildingDefs, string parent)
    {
        return BuildingDefs.Where(bldg => bldg.parentName == parent);
    }
    public static BuildingObj ByLocation(this IEnumerable<BuildingObj> BuildingObjs, Vector2Int ijLocation)
    {
        return BuildingObjs.Where(bldg => bldg.ijLocation == ijLocation).ElementAt(0);
    }
}