using System;
using Duckov.Quests;

// Token: 0x02000116 RID: 278
public class Condition_RaidDead : Condition
{
	// Token: 0x06000968 RID: 2408 RVA: 0x000292B2 File Offset: 0x000274B2
	public override bool Evaluate()
	{
		return RaidUtilities.CurrentRaid.dead;
	}
}
