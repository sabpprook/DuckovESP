using System;
using Saves;
using UnityEngine;

namespace Duckov.Achievements
{
	// Token: 0x02000320 RID: 800
	public class StatisticsManager : MonoBehaviour
	{
		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06001AA0 RID: 6816 RVA: 0x0006055C File Offset: 0x0005E75C
		// (remove) Token: 0x06001AA1 RID: 6817 RVA: 0x00060590 File Offset: 0x0005E790
		public static event Action<string, long, long> OnStatisticsChanged;

		// Token: 0x06001AA2 RID: 6818 RVA: 0x000605C3 File Offset: 0x0005E7C3
		private static string GetSaveKey(string statisticsKey)
		{
			return "Statistics/" + statisticsKey;
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x000605D0 File Offset: 0x0005E7D0
		private static long Get(string key)
		{
			StatisticsManager.GetSaveKey(key);
			if (!SavesSystem.KeyExisits(key))
			{
				return 0L;
			}
			return SavesSystem.Load<long>(key);
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x000605EC File Offset: 0x0005E7EC
		private static void Set(string key, long value)
		{
			long num = StatisticsManager.Get(key);
			StatisticsManager.GetSaveKey(key);
			SavesSystem.Save<long>(key, value);
			Action<string, long, long> onStatisticsChanged = StatisticsManager.OnStatisticsChanged;
			if (onStatisticsChanged == null)
			{
				return;
			}
			onStatisticsChanged(key, num, value);
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x00060620 File Offset: 0x0005E820
		public static void Add(string key, long value = 1L)
		{
			long num = StatisticsManager.Get(key);
			checked
			{
				try
				{
					num += value;
				}
				catch (OverflowException ex)
				{
					Debug.LogException(ex);
					Debug.Log("Failed changing statistics of " + key + ". Overflow detected.");
					return;
				}
				StatisticsManager.Set(key, num);
			}
		}

		// Token: 0x06001AA6 RID: 6822 RVA: 0x00060670 File Offset: 0x0005E870
		private void Awake()
		{
			this.RegisterEvents();
		}

		// Token: 0x06001AA7 RID: 6823 RVA: 0x00060678 File Offset: 0x0005E878
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x00060680 File Offset: 0x0005E880
		private void RegisterEvents()
		{
		}

		// Token: 0x06001AA9 RID: 6825 RVA: 0x00060682 File Offset: 0x0005E882
		private void UnregisterEvents()
		{
		}
	}
}
