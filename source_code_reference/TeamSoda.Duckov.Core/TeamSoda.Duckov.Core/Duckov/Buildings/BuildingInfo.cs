using System;
using Duckov.Economy;
using Duckov.Quests;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x02000311 RID: 785
	[Serializable]
	public struct BuildingInfo
	{
		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x060019DF RID: 6623 RVA: 0x0005D723 File Offset: 0x0005B923
		public bool Valid
		{
			get
			{
				return !string.IsNullOrEmpty(this.id);
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x060019E0 RID: 6624 RVA: 0x0005D733 File Offset: 0x0005B933
		public Building Prefab
		{
			get
			{
				return BuildingDataCollection.GetPrefab(this.prefabName);
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x060019E1 RID: 6625 RVA: 0x0005D740 File Offset: 0x0005B940
		public Vector2Int Dimensions
		{
			get
			{
				if (!this.Prefab)
				{
					return default(Vector2Int);
				}
				return this.Prefab.Dimensions;
			}
		}

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x060019E2 RID: 6626 RVA: 0x0005D76F File Offset: 0x0005B96F
		[LocalizationKey("Default")]
		public string DisplayNameKey
		{
			get
			{
				return "Building_" + this.id;
			}
		}

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x060019E3 RID: 6627 RVA: 0x0005D781 File Offset: 0x0005B981
		public string DisplayName
		{
			get
			{
				return this.DisplayNameKey.ToPlainText();
			}
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0005D78E File Offset: 0x0005B98E
		public static string GetDisplayName(string id)
		{
			return ("Building_" + id).ToPlainText();
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x0005D7A0 File Offset: 0x0005B9A0
		internal bool RequirementsSatisfied()
		{
			string[] array = this.requireBuildings;
			for (int i = 0; i < array.Length; i++)
			{
				if (!BuildingManager.Any(array[i], false))
				{
					return false;
				}
			}
			return QuestManager.AreQuestFinished(this.requireQuests);
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x060019E6 RID: 6630 RVA: 0x0005D7DF File Offset: 0x0005B9DF
		[LocalizationKey("Default")]
		public string DescriptionKey
		{
			get
			{
				return "Building_" + this.id + "_Desc";
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x060019E7 RID: 6631 RVA: 0x0005D7F6 File Offset: 0x0005B9F6
		public string Description
		{
			get
			{
				return this.DescriptionKey.ToPlainText();
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x060019E8 RID: 6632 RVA: 0x0005D803 File Offset: 0x0005BA03
		public int CurrentAmount
		{
			get
			{
				if (BuildingManager.Instance == null)
				{
					return 0;
				}
				return BuildingManager.GetBuildingAmount(this.id);
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x0005D81F File Offset: 0x0005BA1F
		public bool ReachedAmountLimit
		{
			get
			{
				return this.maxAmount > 0 && this.CurrentAmount >= this.maxAmount;
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x060019EA RID: 6634 RVA: 0x0005D83D File Offset: 0x0005BA3D
		public int TokenAmount
		{
			get
			{
				if (BuildingManager.Instance == null)
				{
					return 0;
				}
				return BuildingManager.Instance.GetTokenAmount(this.id);
			}
		}

		// Token: 0x040012A2 RID: 4770
		public string id;

		// Token: 0x040012A3 RID: 4771
		public string prefabName;

		// Token: 0x040012A4 RID: 4772
		public int maxAmount;

		// Token: 0x040012A5 RID: 4773
		public Cost cost;

		// Token: 0x040012A6 RID: 4774
		public string[] requireBuildings;

		// Token: 0x040012A7 RID: 4775
		public string[] alternativeFor;

		// Token: 0x040012A8 RID: 4776
		public int[] requireQuests;

		// Token: 0x040012A9 RID: 4777
		public Sprite iconReference;
	}
}
