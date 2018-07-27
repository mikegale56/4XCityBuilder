using System.Collections.Generic;
using System.Linq;

public static class JobQueries
{
	// JobObj Queries
    public static IEnumerable<JobObj> ByName(this IEnumerable<JobObj> jobObjs, string name)
    {
        return jobObjs.Where(job => job.jobDef.name == name);
    }
	public static IEnumerable<JobObj> ByIndustry(this IEnumerable<JobObj> jobObjs, string industry)
    {
        return jobObjs.Where(job => job.jobDef.industry == industry);
    }
	public static IEnumerable<JobObj> BySkill(this IEnumerable<JobObj> jobObjs, string skill)
    {
        return jobObjs.Where(job => job.jobDef.skill == skill);
    }
	/*public static IEnumerable<JobObj> ByOutput(this IEnumerable<JobObj> jobObjs, string output)
    {
        return jobObjs.Where(jobObjs => jobObjs.jobDef.outputName == output);
    }*/
	public static IEnumerable<JobObj> ByBuildingGuid(this IEnumerable<JobObj> jobObjs, System.Guid guid)
    {
        return jobObjs.Where(job => job.buildingGuid == guid);
    }
	
	// JobDef Queries
	public static IEnumerable<JobDef> ByName(this IEnumerable<JobDef> jobDefs, string name)
    {
        return jobDefs.Where(job => job.name == name);
    }
	public static IEnumerable<JobDef> ByNameAndExactTier(this IEnumerable<JobDef> jobDefs, string name, int exactTier)
    {
        return jobDefs.Where(job => ((job.name == name) && (job.tier == exactTier)));
    }
	public static IEnumerable<JobDef> ByNameAndMaxTier(this IEnumerable<JobDef> jobDefs, string name, int maxTier)
    {
        return jobDefs.Where(job => ((job.name == name) && (job.tier <= maxTier)));
    }
	public static IEnumerable<JobDef> ByIndustry(this IEnumerable<JobDef> jobDefs, string industry)
    {
        return jobDefs.Where(job => job.industry == industry);
    }
    public static IEnumerable<JobDef> BySkill(this IEnumerable<JobDef> jobDefs, string skill)
    {
        return jobDefs.Where(job => job.skill == skill);
    }
}