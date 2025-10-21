using System;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class LevelConfig : MonoBehaviour
{
	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x060008BF RID: 2239 RVA: 0x00027696 File Offset: 0x00025896
	public static LevelConfig Instance
	{
		get
		{
			if (!LevelConfig.instance)
			{
				LevelConfig.SetInstance();
			}
			return LevelConfig.instance;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x060008C0 RID: 2240 RVA: 0x000276AE File Offset: 0x000258AE
	public float LootBoxQualityLowPercent
	{
		get
		{
			return 1f - 1f / this.lootBoxHighQualityChanceMultiplier;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x060008C1 RID: 2241 RVA: 0x000276C2 File Offset: 0x000258C2
	public float LootboxItemCountMultiplier
	{
		get
		{
			return this.lootboxItemCountMultiplier;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x060008C2 RID: 2242 RVA: 0x000276CA File Offset: 0x000258CA
	public static bool IsBaseLevel
	{
		get
		{
			return LevelConfig.Instance && LevelConfig.Instance.isBaseLevel;
		}
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x060008C3 RID: 2243 RVA: 0x000276E4 File Offset: 0x000258E4
	public static bool IsRaidMap
	{
		get
		{
			return LevelConfig.Instance && LevelConfig.Instance.isRaidMap;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x060008C4 RID: 2244 RVA: 0x000276FE File Offset: 0x000258FE
	public static int MinExitCount
	{
		get
		{
			if (!LevelConfig.Instance)
			{
				return 0;
			}
			return LevelConfig.Instance.minExitCount;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x060008C5 RID: 2245 RVA: 0x00027718 File Offset: 0x00025918
	public static bool SpawnTomb
	{
		get
		{
			return !LevelConfig.Instance || LevelConfig.Instance.spawnTomb;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x00027732 File Offset: 0x00025932
	public static int MaxExitCount
	{
		get
		{
			if (!LevelConfig.Instance)
			{
				return 0;
			}
			return LevelConfig.Instance.maxExitCount;
		}
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0002774C File Offset: 0x0002594C
	private void Awake()
	{
		global::UnityEngine.Object.Instantiate<LevelManager>(GameplayDataSettings.Prefabs.LevelManagerPrefab).transform.SetParent(base.transform);
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0002776D File Offset: 0x0002596D
	private static void SetInstance()
	{
		if (LevelConfig.instance)
		{
			return;
		}
		LevelConfig.instance = global::UnityEngine.Object.FindFirstObjectByType<LevelConfig>();
		LevelConfig.instance;
	}

	// Token: 0x040007F2 RID: 2034
	private static LevelConfig instance;

	// Token: 0x040007F3 RID: 2035
	[SerializeField]
	private bool isBaseLevel;

	// Token: 0x040007F4 RID: 2036
	[SerializeField]
	private bool isRaidMap = true;

	// Token: 0x040007F5 RID: 2037
	[SerializeField]
	private bool spawnTomb = true;

	// Token: 0x040007F6 RID: 2038
	[SerializeField]
	private int minExitCount;

	// Token: 0x040007F7 RID: 2039
	[SerializeField]
	private int maxExitCount;

	// Token: 0x040007F8 RID: 2040
	public TimeOfDayConfig timeOfDayConfig;

	// Token: 0x040007F9 RID: 2041
	[SerializeField]
	[Min(1f)]
	private float lootBoxHighQualityChanceMultiplier = 1f;

	// Token: 0x040007FA RID: 2042
	[SerializeField]
	[Range(0.1f, 10f)]
	private float lootboxItemCountMultiplier = 1f;
}
