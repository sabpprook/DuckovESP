using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Duckov.UI;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;

namespace Duckov
{
	// Token: 0x02000231 RID: 561
	public class EXPManager : MonoBehaviour, ISaveDataProvider
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x0600116C RID: 4460 RVA: 0x000437C9 File Offset: 0x000419C9
		public static EXPManager Instance
		{
			get
			{
				return EXPManager.instance;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x0600116D RID: 4461 RVA: 0x000437D0 File Offset: 0x000419D0
		private string LevelChangeNotificationFormat
		{
			get
			{
				return this.levelChangeNotificationFormatKey.ToPlainText();
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x0600116E RID: 4462 RVA: 0x000437DD File Offset: 0x000419DD
		// (set) Token: 0x0600116F RID: 4463 RVA: 0x000437FC File Offset: 0x000419FC
		public static long EXP
		{
			get
			{
				if (EXPManager.instance == null)
				{
					return 0L;
				}
				return EXPManager.instance.point;
			}
			private set
			{
				if (EXPManager.instance == null)
				{
					return;
				}
				int level = EXPManager.Level;
				EXPManager.instance.point = value;
				Action<long> action = EXPManager.onExpChanged;
				if (action != null)
				{
					action(value);
				}
				int level2 = EXPManager.Level;
				if (level != level2)
				{
					EXPManager.OnLevelChanged(level, level2);
				}
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x0004384A File Offset: 0x00041A4A
		public static int Level
		{
			get
			{
				if (EXPManager.instance == null)
				{
					return 0;
				}
				return EXPManager.instance.LevelFromExp(EXPManager.EXP);
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001171 RID: 4465 RVA: 0x0004386A File Offset: 0x00041A6A
		public static long CachedExp
		{
			get
			{
				if (EXPManager.instance == null)
				{
					return 0L;
				}
				return EXPManager.instance.cachedExp;
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00043886 File Offset: 0x00041A86
		private static void OnLevelChanged(int oldLevel, int newLevel)
		{
			Action<int, int> action = EXPManager.onLevelChanged;
			if (action != null)
			{
				action(oldLevel, newLevel);
			}
			if (EXPManager.Instance == null)
			{
				return;
			}
			NotificationText.Push(EXPManager.Instance.LevelChangeNotificationFormat.Format(new
			{
				level = newLevel
			}));
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x000438C2 File Offset: 0x00041AC2
		public static bool AddExp(int amount)
		{
			if (EXPManager.instance == null)
			{
				return false;
			}
			EXPManager.EXP += (long)amount;
			return true;
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x000438E1 File Offset: 0x00041AE1
		private void CacheExp()
		{
			this.cachedExp = this.point;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x000438EF File Offset: 0x00041AEF
		public object GenerateSaveData()
		{
			return this.point;
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x000438FC File Offset: 0x00041AFC
		public void SetupSaveData(object data)
		{
			if (data is long)
			{
				long num = (long)data;
				this.point = num;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06001177 RID: 4471 RVA: 0x0004391F File Offset: 0x00041B1F
		private string realKey
		{
			get
			{
				return "EXP_Value";
			}
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00043928 File Offset: 0x00041B28
		private void Load()
		{
			if (SavesSystem.KeyExisits(this.realKey))
			{
				long num = SavesSystem.Load<long>(this.realKey);
				this.SetupSaveData(num);
			}
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0004395C File Offset: 0x00041B5C
		private void Save()
		{
			object obj = this.GenerateSaveData();
			SavesSystem.Save<long>(this.realKey, (long)obj);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x00043984 File Offset: 0x00041B84
		private void Awake()
		{
			if (EXPManager.instance == null)
			{
				EXPManager.instance = this;
			}
			else
			{
				Debug.LogWarning("检测到多个ExpManager");
			}
			SavesSystem.OnSetFile += this.Load;
			SavesSystem.OnCollectSaveData += this.Save;
			LevelManager.OnLevelInitialized += this.CacheExp;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x000439E3 File Offset: 0x00041BE3
		private void Start()
		{
			this.Load();
			this.CacheExp();
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x000439F1 File Offset: 0x00041BF1
		private void OnDestroy()
		{
			SavesSystem.OnSetFile -= this.Load;
			SavesSystem.OnCollectSaveData -= this.Save;
			LevelManager.OnLevelInitialized -= this.CacheExp;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00043A28 File Offset: 0x00041C28
		public int LevelFromExp(long exp)
		{
			for (int i = 0; i < this.levelExpDefinition.Count; i++)
			{
				long num = this.levelExpDefinition[i];
				if (exp < num)
				{
					return i - 1;
				}
			}
			return this.levelExpDefinition.Count - 1;
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00043A70 File Offset: 0x00041C70
		[return: TupleElementNames(new string[] { "from", "to" })]
		public ValueTuple<long, long> GetLevelExpRange(int level)
		{
			int num = this.levelExpDefinition.Count - 1;
			if (level >= num)
			{
				List<long> list = this.levelExpDefinition;
				return new ValueTuple<long, long>(list[list.Count - 1], long.MaxValue);
			}
			long num2 = this.levelExpDefinition[level];
			long num3 = this.levelExpDefinition[level + 1];
			return new ValueTuple<long, long>(num2, num3);
		}

		// Token: 0x04000D80 RID: 3456
		private static EXPManager instance;

		// Token: 0x04000D81 RID: 3457
		[SerializeField]
		private string levelChangeNotificationFormatKey = "UI_LevelChangeNotification";

		// Token: 0x04000D82 RID: 3458
		[SerializeField]
		private List<long> levelExpDefinition;

		// Token: 0x04000D83 RID: 3459
		[SerializeField]
		private long point;

		// Token: 0x04000D84 RID: 3460
		public static Action<long> onExpChanged;

		// Token: 0x04000D85 RID: 3461
		public static Action<int, int> onLevelChanged;

		// Token: 0x04000D86 RID: 3462
		private long cachedExp;

		// Token: 0x04000D87 RID: 3463
		private const string prefixKey = "EXP";

		// Token: 0x04000D88 RID: 3464
		private const string key = "Value";
	}
}
