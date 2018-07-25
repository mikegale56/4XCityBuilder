public class JobBonusQuality : JobBonus
{
	public float qualityBonus;
	
	public JobBonusQuality(string industry, string skill, float qualityBonus, BonusType type)
	{
		this.industry = industry;
		this.skill = skill;
		this.guid = System.Guid.NewGuid();
		this.qualityBonus = qualityBonus;
		this.type = type;
	}
    
	public override void ApplyBonusToJob(JobObj job)
	{
		switch (this.type)
		{
			case BonusType.building:
                job.buildingBonusQualityMultiplier += this.qualityBonus;
				break;
			case BonusType.leader:
                job.leaderBonusQualityMultiplier += this.qualityBonus;
				break;
		}
	}
}