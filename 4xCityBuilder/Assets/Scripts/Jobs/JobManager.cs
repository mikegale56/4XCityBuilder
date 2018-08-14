using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class JobManager : ManagerBase
{

    private List<JobObj> jobList;
    private List<JobBonus> jobBonusList;
    private float lastGameTime = 0;

    // Initialization
    void Awake()
    {

        ManagerBase.jobDefinitions = new List<JobDef>();

        string m_Path = Application.dataPath;
        using (var reader = new StreamReader(m_Path + "/Definitions/Jobs.csv"))
        {
            // Parse the header
            Dictionary<string, List<int>> column = ParseHeader(reader.ReadLine());
            while (!reader.EndOfStream)
                ManagerBase.jobDefinitions.Add(new JobDef(reader.ReadLine(), column));
        }
        jobList = new List<JobObj>();
        jobBonusList = new List<JobBonus>();
}

    // Use this for initialization
    void Start()
    {    }

    // Late Update is called once per frame after update
    void LateUpdate() 
	{

        // Remove completed and canceled jobs
        jobList.RemoveAll(job => job.toDelete == true);

        // Updates the jobs list by adding PDU
        float currentGameTimeSeconds = GameRunner.gameTime;
        float deltaGameTimeSeconds = currentGameTimeSeconds - lastGameTime;
        //Debug.Log("JobManager: updating " + jobList.Count + " jobs with delta time = " + deltaGameTimeSeconds.ToString());
        foreach (JobObj job in jobList)
            job.UpdateJob(deltaGameTimeSeconds);
        lastGameTime = currentGameTimeSeconds;
    }
	
	public void AddBonus(JobBonus newBonus)
	{
        jobBonusList.Add(newBonus);
        foreach (JobObj job in jobList)
            job.UpdateBonuses(jobBonusList);
	}
	
	public void RemoveBonus(System.Guid bonusGuid)
	{
        jobBonusList.RemoveAll(x => x.guid == bonusGuid);
        foreach (JobObj job in jobList)
            job.UpdateBonuses(jobBonusList);
    }
	
	public ResourceJobObj AddJob(JobDef newJobDef, BuildingObj bldg)
	{
        ResourceJobObj newJobObj = new ResourceJobObj(newJobDef, bldg);
        newJobObj.UpdateBonuses(jobBonusList);
        jobList.Add(newJobObj);
        return newJobObj;
    }

    public BuildingJobObj AddConstructionJob(JobDef newJobDef, int iLoc, int jLoc)
    {
        BuildingJobObj newJobObj = new BuildingJobObj(newJobDef, iLoc, jLoc);
        newJobObj.UpdateBonuses(jobBonusList);
        jobList.Add(newJobObj);
        return newJobObj;
    }

    public DemoJobObj AddDemolitionJob(JobDef newJobDef, int iLoc, int jLoc)
    {
        DemoJobObj newJobObj = new DemoJobObj(newJobDef, iLoc, jLoc);
        newJobObj.UpdateBonuses(jobBonusList);
        jobList.Add(newJobObj);
        return newJobObj;
    }
    
    public void CancelJob(System.Guid jobGuid)
	{
		
	}
	
	Dictionary<string, List<int>> ParseHeader(string header)
    {
        Dictionary<string, List<int>> column = new Dictionary<string, List<int>>();

        string[] values = header.Split(',');
        int ind = 0;
        foreach (string val in values)
		{
            if (column.ContainsKey(val))
				column[val].Add(ind++);
			else
			{
				column[val] = new List<int>();
				column[val].Add(ind++);
			}
		}

        return column;
    }

    // JobObj Queries
    public IEnumerable<JobObj> ByName(string name)
    {
        return jobList.Where(job => job.jobDef.name == name);
    }
    public IEnumerable<JobObj> ByIndustry(string industry)
    {
        return jobList.Where(job => job.jobDef.industry == industry);
    }
    public IEnumerable<JobObj> BySkill(string skill)
    {
        return jobList.Where(job => job.jobDef.skill == skill);
    }
    public IEnumerable<JobObj> ByBuildingGuid(System.Guid guid)
    {
        return jobList.Where(job => job.buildingGuid == guid);
    }
    public IEnumerable<JobObj> ByLocation(int iLoc, int jLoc)
    {
        return jobList.Where(job => ((job.iLoc == iLoc) && (job.jLoc == jLoc)));
    }

}