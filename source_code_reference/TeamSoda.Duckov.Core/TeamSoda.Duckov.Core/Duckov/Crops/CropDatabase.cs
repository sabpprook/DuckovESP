using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Crops
{
	// Token: 0x020002E4 RID: 740
	[CreateAssetMenu]
	public class CropDatabase : ScriptableObject
	{
		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x060017C2 RID: 6082 RVA: 0x00056FB9 File Offset: 0x000551B9
		public static CropDatabase Instance
		{
			get
			{
				return GameplayDataSettings.CropDatabase;
			}
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x00056FC0 File Offset: 0x000551C0
		public static CropInfo? GetCropInfo(string id)
		{
			CropDatabase instance = CropDatabase.Instance;
			for (int i = 0; i < instance.entries.Count; i++)
			{
				CropInfo cropInfo = instance.entries[i];
				if (cropInfo.id == id)
				{
					return new CropInfo?(cropInfo);
				}
			}
			return null;
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00057014 File Offset: 0x00055214
		internal static bool IsIdValid(string id)
		{
			return !(CropDatabase.Instance == null) && CropDatabase.Instance.entries.Any((CropInfo e) => e.id == id);
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00057058 File Offset: 0x00055258
		internal static bool IsSeed(int itemTypeID)
		{
			return !(CropDatabase.Instance == null) && CropDatabase.Instance.seedInfos.Any((SeedInfo e) => e.itemTypeID == itemTypeID);
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x0005709C File Offset: 0x0005529C
		internal static SeedInfo GetSeedInfo(int seedItemTypeID)
		{
			if (CropDatabase.Instance == null)
			{
				return default(SeedInfo);
			}
			return CropDatabase.Instance.seedInfos.FirstOrDefault((SeedInfo e) => e.itemTypeID == seedItemTypeID);
		}

		// Token: 0x04001155 RID: 4437
		[SerializeField]
		public List<CropInfo> entries = new List<CropInfo>();

		// Token: 0x04001156 RID: 4438
		[SerializeField]
		public List<SeedInfo> seedInfos = new List<SeedInfo>();
	}
}
