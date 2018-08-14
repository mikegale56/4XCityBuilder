using System.Collections.Generic;
using System.Linq;

public static class JobQueries
{
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
    public static JobDef ByGuid(this IEnumerable<JobDef> jobDefs, System.Guid guid)
    {
        return jobDefs.Where(job => job.guid == guid).ElementAt(0);
    }
}