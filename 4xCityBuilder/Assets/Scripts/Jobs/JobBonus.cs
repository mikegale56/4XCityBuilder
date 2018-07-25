public abstract class JobBonus
{
    public System.Guid guid;
	public string industry;
	public string skill;
	public BonusType type; 
    
	abstract public void ApplyBonusToJob(JobObj job);
}

public enum BonusType { building, leader };