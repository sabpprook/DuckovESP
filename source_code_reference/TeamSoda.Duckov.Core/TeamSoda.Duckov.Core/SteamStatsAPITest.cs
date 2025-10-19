using System;
using Steamworks;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class SteamStatsAPITest : MonoBehaviour
{
	// Token: 0x06000E55 RID: 3669 RVA: 0x00039C55 File Offset: 0x00037E55
	private void Awake()
	{
		this.onStatsReceivedCallback = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatReceived));
		this.onStatsStoredCallback = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate(this.OnUserStatStored));
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x00039C85 File Offset: 0x00037E85
	private void OnUserStatStored(UserStatsStored_t param)
	{
		Debug.Log("Stat Stored!");
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00039C94 File Offset: 0x00037E94
	private void OnUserStatReceived(UserStatsReceived_t param)
	{
		string text = "Stat Fetched:";
		CSteamID steamIDUser = param.m_steamIDUser;
		Debug.Log(text + steamIDUser.ToString() + " " + param.m_nGameID.ToString());
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00039CD5 File Offset: 0x00037ED5
	private void Start()
	{
		SteamUserStats.RequestGlobalStats(60);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x00039CE0 File Offset: 0x00037EE0
	private void Test()
	{
		int num;
		Debug.Log(SteamUserStats.GetStat("game_finished", out num).ToString() + " " + num.ToString());
		bool flag = SteamUserStats.SetStat("game_finished", num + 1);
		Debug.Log(string.Format("Set: {0}", flag));
		SteamUserStats.StoreStats();
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00039D40 File Offset: 0x00037F40
	private void GetGlobalStat()
	{
		long num;
		if (SteamUserStats.GetGlobalStat("game_finished", out num))
		{
			Debug.Log(string.Format("game finished: {0}", num));
			return;
		}
		Debug.Log("Failed");
	}

	// Token: 0x04000BE5 RID: 3045
	private Callback<UserStatsReceived_t> onStatsReceivedCallback;

	// Token: 0x04000BE6 RID: 3046
	private Callback<UserStatsStored_t> onStatsStoredCallback;
}
