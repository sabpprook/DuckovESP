using System;
using System.Collections.Generic;
using System.Linq;
using Duckov.Utilities;
using ItemStatsSystem;
using Saves;
using UnityEngine;

namespace Duckov.MasterKeys
{
	// Token: 0x020002DB RID: 731
	public class MasterKeysManager : MonoBehaviour
	{
		// Token: 0x14000096 RID: 150
		// (add) Token: 0x06001746 RID: 5958 RVA: 0x00055854 File Offset: 0x00053A54
		// (remove) Token: 0x06001747 RID: 5959 RVA: 0x00055888 File Offset: 0x00053A88
		public static event Action<int> OnMasterKeyUnlocked;

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06001748 RID: 5960 RVA: 0x000558BB File Offset: 0x00053ABB
		// (set) Token: 0x06001749 RID: 5961 RVA: 0x000558C2 File Offset: 0x00053AC2
		public static MasterKeysManager Instance { get; private set; }

		// Token: 0x0600174A RID: 5962 RVA: 0x000558CC File Offset: 0x00053ACC
		public static bool SubmitAndActivate(Item item)
		{
			if (MasterKeysManager.Instance == null)
			{
				return false;
			}
			if (item == null)
			{
				return false;
			}
			int typeID = item.TypeID;
			if (MasterKeysManager.IsActive(typeID))
			{
				return false;
			}
			if (item.StackCount > 1)
			{
				int stackCount = item.StackCount;
				item.StackCount = stackCount - 1;
			}
			else
			{
				item.Detach();
				item.DestroyTree();
			}
			MasterKeysManager.Activate(typeID);
			return true;
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x00055932 File Offset: 0x00053B32
		public static bool IsActive(int id)
		{
			return !(MasterKeysManager.Instance == null) && MasterKeysManager.Instance.IsActive_Local(id);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0005594E File Offset: 0x00053B4E
		internal static void Activate(int id)
		{
			if (MasterKeysManager.Instance == null)
			{
				return;
			}
			MasterKeysManager.Instance.Activate_Local(id);
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x00055969 File Offset: 0x00053B69
		internal static MasterKeysManager.Status GetStatus(int id)
		{
			if (MasterKeysManager.Instance == null)
			{
				return null;
			}
			return MasterKeysManager.Instance.GetStatus_Local(id);
		}

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x00055985 File Offset: 0x00053B85
		public int Count
		{
			get
			{
				return this.keysStatus.Count;
			}
		}

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x0600174F RID: 5967 RVA: 0x00055994 File Offset: 0x00053B94
		public static List<int> AllPossibleKeys
		{
			get
			{
				if (MasterKeysManager._cachedKeyItemIds == null)
				{
					MasterKeysManager._cachedKeyItemIds = new List<int>();
					foreach (ItemAssetsCollection.Entry entry in ItemAssetsCollection.Instance.entries)
					{
						Tag[] tags = entry.metaData.tags;
						if (tags.Any((Tag e) => Tag.Match(e, "Key")))
						{
							if (GameMetaData.Instance.IsDemo)
							{
								if (tags.Any((Tag e) => e.name == GameplayDataSettings.Tags.LockInDemoTag.name))
								{
									continue;
								}
							}
							if (!tags.Any((Tag e) => MasterKeysManager.excludeTags.Contains(e.name)))
							{
								MasterKeysManager._cachedKeyItemIds.Add(entry.typeID);
							}
						}
					}
				}
				return MasterKeysManager._cachedKeyItemIds;
			}
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x00055AA0 File Offset: 0x00053CA0
		private void Awake()
		{
			if (MasterKeysManager.Instance == null)
			{
				MasterKeysManager.Instance = this;
			}
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			this.Load();
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x00055ACC File Offset: 0x00053CCC
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x00055ADF File Offset: 0x00053CDF
		private void OnCollectSaveData()
		{
			this.Save();
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x00055AE8 File Offset: 0x00053CE8
		public bool IsActive_Local(int id)
		{
			MasterKeysManager.Status status = MasterKeysManager.GetStatus(id);
			return status != null && status.active;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x00055B07 File Offset: 0x00053D07
		private void Activate_Local(int id)
		{
			if (id < 0)
			{
				return;
			}
			if (!MasterKeysManager.AllPossibleKeys.Contains(id))
			{
				return;
			}
			this.GetOrCreateStatus(id).active = true;
			Action<int> onMasterKeyUnlocked = MasterKeysManager.OnMasterKeyUnlocked;
			if (onMasterKeyUnlocked == null)
			{
				return;
			}
			onMasterKeyUnlocked(id);
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x00055B3C File Offset: 0x00053D3C
		public MasterKeysManager.Status GetStatus_Local(int id)
		{
			return this.keysStatus.Find((MasterKeysManager.Status e) => e.id == id);
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x00055B70 File Offset: 0x00053D70
		public MasterKeysManager.Status GetOrCreateStatus(int id)
		{
			MasterKeysManager.Status status_Local = this.GetStatus_Local(id);
			if (status_Local != null)
			{
				return status_Local;
			}
			MasterKeysManager.Status status = new MasterKeysManager.Status();
			status.id = id;
			this.keysStatus.Add(status);
			return status;
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x00055BA4 File Offset: 0x00053DA4
		private void Save()
		{
			SavesSystem.Save<List<MasterKeysManager.Status>>("MasterKeys", this.keysStatus);
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x00055BB6 File Offset: 0x00053DB6
		private void Load()
		{
			if (SavesSystem.KeyExisits("MasterKeys"))
			{
				this.keysStatus = SavesSystem.Load<List<MasterKeysManager.Status>>("MasterKeys");
				return;
			}
			this.keysStatus = new List<MasterKeysManager.Status>();
		}

		// Token: 0x04001110 RID: 4368
		[SerializeField]
		private List<MasterKeysManager.Status> keysStatus = new List<MasterKeysManager.Status>();

		// Token: 0x04001111 RID: 4369
		private static List<int> _cachedKeyItemIds;

		// Token: 0x04001112 RID: 4370
		private static string[] excludeTags = new string[] { "SpecialKey" };

		// Token: 0x04001113 RID: 4371
		private const string SaveKey = "MasterKeys";

		// Token: 0x02000574 RID: 1396
		[Serializable]
		public class Status
		{
			// Token: 0x04001F84 RID: 8068
			[ItemTypeID]
			public int id;

			// Token: 0x04001F85 RID: 8069
			public bool active;
		}
	}
}
