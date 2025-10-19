using System;

// Token: 0x02000073 RID: 115
public class Team
{
	// Token: 0x0600043E RID: 1086 RVA: 0x000135E3 File Offset: 0x000117E3
	public static bool IsEnemy(Teams selfTeam, Teams targetTeam)
	{
		return selfTeam != Teams.middle && (selfTeam == Teams.all || (targetTeam != Teams.middle && selfTeam != targetTeam));
	}
}
