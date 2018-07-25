public class JobBonusSpeed : JobBonus
{
	public float speedBonus;
	
	public JobBonusSpeed(string industry, string skill, float speedBonus, BonusType type)
	{
		this.industry = industry;
		this.skill = skill;
		this.guid = System.Guid.NewGuid();
		this.speedBonus = speedBonus;
		this.type = type;
	}
    
	public override void ApplyBonusToJob(JobObj job)
	{
		switch (this.type)
		{
			case BonusType.building:
                job.buildingBonusSpeedMultiplier += this.speedBonus;
				break;
			case BonusType.leader:
                job.leaderBonusSpeedMultiplier += this.speedBonus;
				break;
		}
	}
}