using System;
using Saves;

// Token: 0x02000124 RID: 292
public class SavesCounter
{
	// Token: 0x0600098F RID: 2447 RVA: 0x00029804 File Offset: 0x00027A04
	public static int AddCount(string countKey)
	{
		int num = SavesSystem.Load<int>("Count/" + countKey);
		num++;
		SavesSystem.Save<int>("Count/" + countKey, num);
		return num;
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00029838 File Offset: 0x00027A38
	public static int GetCount(string countKey)
	{
		return SavesSystem.Load<int>("Count/" + countKey);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x0002984C File Offset: 0x00027A4C
	public static int AddKillCount(string key)
	{
		int num = SavesCounter.AddCount("Kills/" + key);
		Action<string, int> onKillCountChanged = SavesCounter.OnKillCountChanged;
		if (onKillCountChanged != null)
		{
			onKillCountChanged(key, num);
		}
		return num;
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x0002987D File Offset: 0x00027A7D
	public static int GetKillCount(string key)
	{
		return SavesCounter.GetCount("Kills/" + key);
	}

	// Token: 0x04000869 RID: 2153
	public static Action<string, int> OnKillCountChanged;
}
