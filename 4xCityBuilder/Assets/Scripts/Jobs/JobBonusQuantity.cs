public class JobBonusQuantity : JobBonus
{
	public float quantityBonus;
	
	public JobBonusQuantity(string industry, string skill, float quantityBonus, BonusType type)
	{
		this.industry = industry;
		this.skill = skill;
		this.guid = System.Guid.NewGuid();
		this.quantityBonus = quantityBonus;
		this.type = type;
	}
    
	public override void ApplyBonusToJob(JobObj job)
	{
		switch (this.type)
		{
			case BonusType.building:
                job.buildingBonusQuantityMultiplier += this.quantityBonus;
				break;
			case BonusType.leader:
                job.leaderBonusQuantityMultiplier += this.quantityBonus;
				break;
		}
	}
}