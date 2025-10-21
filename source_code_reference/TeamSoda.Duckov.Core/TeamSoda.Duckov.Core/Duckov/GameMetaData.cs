using System;
using UnityEngine;

namespace Duckov
{
	// Token: 0x02000233 RID: 563
	[CreateAssetMenu(menuName = "Settings/MetaData")]
	public class GameMetaData : ScriptableObject
	{
		// Token: 0x1700030E RID: 782
		// (get) Token: 0x0600118A RID: 4490 RVA: 0x00043C5C File Offset: 0x00041E5C
		public VersionData Version
		{
			get
			{
				if (GameMetaData.Instance == null)
				{
					return default(VersionData);
				}
				return GameMetaData.Instance.versionData.versionData;
			}
		}

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x0600118B RID: 4491 RVA: 0x00043C8F File Offset: 0x00041E8F
		public bool IsDemo
		{
			get
			{
				return this.isDemo;
			}
		}

		// Token: 0x17000310 RID: 784
		// (get) Token: 0x0600118C RID: 4492 RVA: 0x00043C97 File Offset: 0x00041E97
		public bool IsTestVersion
		{
			get
			{
				return this.isTestVersion;
			}
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x00043CA4 File Offset: 0x00041EA4
		public static GameMetaData Instance
		{
			get
			{
				if (GameMetaData._instance == null)
				{
					GameMetaData._instance = Resources.Load<GameMetaData>("MetaData");
				}
				return GameMetaData._instance;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x0600118E RID: 4494 RVA: 0x00043CC7 File Offset: 0x00041EC7
		public static bool BloodFxOn
		{
			get
			{
				return GameMetaData.Instance.bloodFxOn;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x0600118F RID: 4495 RVA: 0x00043CD3 File Offset: 0x00041ED3
		// (set) Token: 0x06001190 RID: 4496 RVA: 0x00043CDB File Offset: 0x00041EDB
		public Platform Platform
		{
			get
			{
				return this.platform;
			}
			set
			{
				this.platform = value;
			}
		}

		// Token: 0x04000D8D RID: 3469
		[SerializeField]
		private GameVersionData versionData;

		// Token: 0x04000D8E RID: 3470
		[SerializeField]
		private bool isTestVersion;

		// Token: 0x04000D8F RID: 3471
		[SerializeField]
		private bool isDemo;

		// Token: 0x04000D90 RID: 3472
		[SerializeField]
		private Platform platform;

		// Token: 0x04000D91 RID: 3473
		[SerializeField]
		private bool bloodFxOn = true;

		// Token: 0x04000D92 RID: 3474
		private static GameMetaData _instance;
	}
}
