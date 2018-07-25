using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JobManager : MonoBehaviour {

	public Dictionary<string, JobDef> jobDefsByName;
    private List<JobObj> activeJobList;
	private List<JobBonus> activeLeaderBonusList;

    // Initialization
    void Awake()
    {

        jobDefsByName = new Dictionary<string, JobDef>();
		activeJobList  = new List<JobObj>();
        activeLeaderBonusList = new List<JobBonus>();
		
		/*string m_Path = Application.dataPath;
        using (var reader = new StreamReader(m_Path + "/Definitions/Jobs.csv"))
        {
            // Parse the header
            Dictionary<string, List<int>> column = ParseHeader(reader.ReadLine());
            while (!reader.EndOfStream)
			{
                JobDef newDef = new JobDef(reader.ReadLine(), column);
				jobDefsByName.Add(newDef.name, newDef);
			}			
        }*/
    }

    // Use this for initialization
    void Start()
    {    }

    // Update is called once per frame
    void Update() 
	{	
		// Updates the jobs list by adding PDU
	}
	
	public void AddBonus(JobBonus newBonus)
	{
		
	}
	
	public void RemoveBonus(System.Guid bonusGuid)
	{
		
	}
	
	public void AddJob(JobDef newJobDef)
	{
		
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
	
}