using System;
using Duckov;
using Saves;
using UnityEngine;

// Token: 0x02000172 RID: 370
public static class RaidUtilities
{
	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06000B49 RID: 2889 RVA: 0x0002FDD0 File Offset: 0x0002DFD0
	// (set) Token: 0x06000B4A RID: 2890 RVA: 0x0002FE1C File Offset: 0x0002E01C
	public static RaidUtilities.RaidInfo CurrentRaid
	{
		get
		{
			RaidUtilities.RaidInfo raidInfo = SavesSystem.Load<RaidUtilities.RaidInfo>("RaidInfo");
			raidInfo.totalTime = Time.unscaledTime - raidInfo.raidBeginTime;
			raidInfo.expOnEnd = EXPManager.EXP;
			raidInfo.expGained = raidInfo.expOnEnd - raidInfo.expOnBegan;
			return raidInfo;
		}
		private set
		{
			SavesSystem.Save<RaidUtilities.RaidInfo>("RaidInfo", value);
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x0002FE2C File Offset: 0x0002E02C
	public static void NewRaid()
	{
		RaidUtilities.RaidInfo currentRaid = RaidUtilities.CurrentRaid;
		RaidUtilities.RaidInfo raidInfo = new RaidUtilities.RaidInfo
		{
			valid = true,
			ID = currentRaid.ID + 1U,
			dead = false,
			ended = false,
			raidBeginTime = Time.unscaledTime,
			raidEndTime = 0f,
			expOnBegan = EXPManager.EXP
		};
		RaidUtilities.CurrentRaid = raidInfo;
		Action<RaidUtilities.RaidInfo> onNewRaid = RaidUtilities.OnNewRaid;
		if (onNewRaid == null)
		{
			return;
		}
		onNewRaid(raidInfo);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x0002FEAC File Offset: 0x0002E0AC
	public static void NotifyDead()
	{
		RaidUtilities.RaidInfo currentRaid = RaidUtilities.CurrentRaid;
		currentRaid.dead = true;
		currentRaid.ended = true;
		currentRaid.raidEndTime = Time.unscaledTime;
		currentRaid.totalTime = currentRaid.raidEndTime - currentRaid.raidBeginTime;
		currentRaid.expOnEnd = EXPManager.EXP;
		currentRaid.expGained = currentRaid.expOnEnd - currentRaid.expOnBegan;
		RaidUtilities.CurrentRaid = currentRaid;
		Action<RaidUtilities.RaidInfo> onRaidEnd = RaidUtilities.OnRaidEnd;
		if (onRaidEnd != null)
		{
			onRaidEnd(currentRaid);
		}
		Action<RaidUtilities.RaidInfo> onRaidDead = RaidUtilities.OnRaidDead;
		if (onRaidDead == null)
		{
			return;
		}
		onRaidDead(currentRaid);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0002FF38 File Offset: 0x0002E138
	public static void NotifyEnd()
	{
		RaidUtilities.RaidInfo currentRaid = RaidUtilities.CurrentRaid;
		currentRaid.ended = true;
		currentRaid.raidEndTime = Time.unscaledTime;
		currentRaid.totalTime = currentRaid.raidEndTime - currentRaid.raidBeginTime;
		currentRaid.expOnEnd = EXPManager.EXP;
		currentRaid.expGained = currentRaid.expOnEnd - currentRaid.expOnBegan;
		RaidUtilities.CurrentRaid = currentRaid;
		Action<RaidUtilities.RaidInfo> onRaidEnd = RaidUtilities.OnRaidEnd;
		if (onRaidEnd == null)
		{
			return;
		}
		onRaidEnd(currentRaid);
	}

	// Token: 0x0400099D RID: 2461
	public static Action<RaidUtilities.RaidInfo> OnNewRaid;

	// Token: 0x0400099E RID: 2462
	public static Action<RaidUtilities.RaidInfo> OnRaidDead;

	// Token: 0x0400099F RID: 2463
	public static Action<RaidUtilities.RaidInfo> OnRaidEnd;

	// Token: 0x040009A0 RID: 2464
	private const string SaveID = "RaidInfo";

	// Token: 0x020004B1 RID: 1201
	[Serializable]
	public struct RaidInfo
	{
		// Token: 0x04001C56 RID: 7254
		public bool valid;

		// Token: 0x04001C57 RID: 7255
		public uint ID;

		// Token: 0x04001C58 RID: 7256
		public bool dead;

		// Token: 0x04001C59 RID: 7257
		public bool ended;

		// Token: 0x04001C5A RID: 7258
		public float raidBeginTime;

		// Token: 0x04001C5B RID: 7259
		public float raidEndTime;

		// Token: 0x04001C5C RID: 7260
		public float totalTime;

		// Token: 0x04001C5D RID: 7261
		public long expOnBegan;

		// Token: 0x04001C5E RID: 7262
		public long expOnEnd;

		// Token: 0x04001C5F RID: 7263
		public long expGained;
	}
}
