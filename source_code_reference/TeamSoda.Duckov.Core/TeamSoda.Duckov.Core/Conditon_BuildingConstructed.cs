using System;
using Duckov.Buildings;
using Duckov.Quests;

// Token: 0x02000119 RID: 281
public class Conditon_BuildingConstructed : Condition
{
	// Token: 0x0600096F RID: 2415 RVA: 0x000293CC File Offset: 0x000275CC
	public override bool Evaluate()
	{
		bool flag = BuildingManager.Any(this.buildingID, false);
		if (this.not)
		{
			flag = !flag;
		}
		return flag;
	}

	// Token: 0x04000854 RID: 2132
	public string buildingID;

	// Token: 0x04000855 RID: 2133
	public bool not;
}
