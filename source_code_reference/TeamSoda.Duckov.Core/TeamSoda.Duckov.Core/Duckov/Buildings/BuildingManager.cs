using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Saves;
using Sirenix.Utilities;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x02000313 RID: 787
	public class BuildingManager : MonoBehaviour
	{
		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x060019F6 RID: 6646 RVA: 0x0005DA29 File Offset: 0x0005BC29
		// (set) Token: 0x060019F7 RID: 6647 RVA: 0x0005DA30 File Offset: 0x0005BC30
		public static BuildingManager Instance { get; private set; }

		// Token: 0x060019F8 RID: 6648 RVA: 0x0005DA38 File Offset: 0x0005BC38
		private static int GenerateBuildingGUID(string buildingID)
		{
			BuildingManager.<>c__DisplayClass4_0 CS$<>8__locals1 = new BuildingManager.<>c__DisplayClass4_0();
			CS$<>8__locals1.<GenerateBuildingGUID>g__Regenerate|0();
			while (BuildingManager.Any((BuildingManager.BuildingData e) => e != null && e.GUID == CS$<>8__locals1.result))
			{
				CS$<>8__locals1.<GenerateBuildingGUID>g__Regenerate|0();
			}
			return CS$<>8__locals1.result;
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x0005DA74 File Offset: 0x0005BC74
		public int GetTokenAmount(string id)
		{
			BuildingManager.BuildingTokenAmountEntry buildingTokenAmountEntry = this.tokens.Find((BuildingManager.BuildingTokenAmountEntry e) => e.id == id);
			if (buildingTokenAmountEntry != null)
			{
				return buildingTokenAmountEntry.amount;
			}
			return 0;
		}

		// Token: 0x060019FA RID: 6650 RVA: 0x0005DAB4 File Offset: 0x0005BCB4
		private void SetTokenAmount(string id, int amount)
		{
			BuildingManager.BuildingTokenAmountEntry buildingTokenAmountEntry = this.tokens.Find((BuildingManager.BuildingTokenAmountEntry e) => e.id == id);
			if (buildingTokenAmountEntry != null)
			{
				buildingTokenAmountEntry.amount = amount;
				return;
			}
			buildingTokenAmountEntry = new BuildingManager.BuildingTokenAmountEntry
			{
				id = id,
				amount = amount
			};
			this.tokens.Add(buildingTokenAmountEntry);
		}

		// Token: 0x060019FB RID: 6651 RVA: 0x0005DB18 File Offset: 0x0005BD18
		private void AddToken(string id, int amount = 1)
		{
			BuildingManager.BuildingTokenAmountEntry buildingTokenAmountEntry = this.tokens.Find((BuildingManager.BuildingTokenAmountEntry e) => e.id == id);
			if (buildingTokenAmountEntry == null)
			{
				buildingTokenAmountEntry = new BuildingManager.BuildingTokenAmountEntry
				{
					id = id,
					amount = 0
				};
				this.tokens.Add(buildingTokenAmountEntry);
			}
			buildingTokenAmountEntry.amount += amount;
		}

		// Token: 0x060019FC RID: 6652 RVA: 0x0005DB80 File Offset: 0x0005BD80
		private bool PayToken(string id)
		{
			BuildingManager.BuildingTokenAmountEntry buildingTokenAmountEntry = this.tokens.Find((BuildingManager.BuildingTokenAmountEntry e) => e.id == id);
			if (buildingTokenAmountEntry == null)
			{
				return false;
			}
			if (buildingTokenAmountEntry.amount <= 0)
			{
				return false;
			}
			buildingTokenAmountEntry.amount--;
			return true;
		}

		// Token: 0x060019FD RID: 6653 RVA: 0x0005DBD4 File Offset: 0x0005BDD4
		public static Vector2Int[] GetOccupyingCoords(Vector2Int dimensions, BuildingRotation rotations, Vector2Int coord)
		{
			if (rotations % BuildingRotation.Half != BuildingRotation.Zero)
			{
				dimensions = new Vector2Int(dimensions.y, dimensions.x);
			}
			Vector2Int[] array = new Vector2Int[dimensions.x * dimensions.y];
			for (int i = 0; i < dimensions.y; i++)
			{
				for (int j = 0; j < dimensions.x; j++)
				{
					int num = j + dimensions.x * i;
					array[num] = coord + new Vector2Int(j, i);
				}
			}
			return array;
		}

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x060019FE RID: 6654 RVA: 0x0005DC55 File Offset: 0x0005BE55
		public List<BuildingManager.BuildingAreaData> Areas
		{
			get
			{
				return this.areas;
			}
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0005DC60 File Offset: 0x0005BE60
		public BuildingManager.BuildingAreaData GetOrCreateArea(string id)
		{
			BuildingManager.BuildingAreaData buildingAreaData = this.areas.Find((BuildingManager.BuildingAreaData e) => e != null && e.AreaID == id);
			if (buildingAreaData != null)
			{
				return buildingAreaData;
			}
			BuildingManager.BuildingAreaData buildingAreaData2 = new BuildingManager.BuildingAreaData(id);
			this.areas.Add(buildingAreaData2);
			return buildingAreaData2;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x0005DCB0 File Offset: 0x0005BEB0
		public BuildingManager.BuildingAreaData GetArea(string id)
		{
			return this.areas.Find((BuildingManager.BuildingAreaData e) => e != null && e.AreaID == id);
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x0005DCE1 File Offset: 0x0005BEE1
		private void CleanupAndSort()
		{
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x0005DCE3 File Offset: 0x0005BEE3
		public static BuildingInfo GetBuildingInfo(string id)
		{
			return BuildingDataCollection.GetInfo(id);
		}

		// Token: 0x06001A03 RID: 6659 RVA: 0x0005DCEC File Offset: 0x0005BEEC
		public static bool Any(string id, bool includeTokens = false)
		{
			if (BuildingManager.Instance == null)
			{
				return false;
			}
			if (includeTokens && BuildingManager.Instance.GetTokenAmount(id) > 0)
			{
				return true;
			}
			using (List<BuildingManager.BuildingAreaData>.Enumerator enumerator = BuildingManager.Instance.Areas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Any(id))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x0005DD6C File Offset: 0x0005BF6C
		public static bool Any(Func<BuildingManager.BuildingData, bool> predicate)
		{
			if (BuildingManager.Instance == null)
			{
				return false;
			}
			using (List<BuildingManager.BuildingAreaData>.Enumerator enumerator = BuildingManager.Instance.Areas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Any(predicate))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001A05 RID: 6661 RVA: 0x0005DDDC File Offset: 0x0005BFDC
		public static int GetBuildingAmount(string id)
		{
			if (BuildingManager.Instance == null)
			{
				return 0;
			}
			int num = 0;
			foreach (BuildingManager.BuildingAreaData buildingAreaData in BuildingManager.Instance.Areas)
			{
				using (List<BuildingManager.BuildingData>.Enumerator enumerator2 = buildingAreaData.Buildings.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.ID == id)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x06001A06 RID: 6662 RVA: 0x0005DE88 File Offset: 0x0005C088
		// (remove) Token: 0x06001A07 RID: 6663 RVA: 0x0005DEBC File Offset: 0x0005C0BC
		public static event Action OnBuildingListChanged;

		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x06001A08 RID: 6664 RVA: 0x0005DEF0 File Offset: 0x0005C0F0
		// (remove) Token: 0x06001A09 RID: 6665 RVA: 0x0005DF24 File Offset: 0x0005C124
		public static event Action<int> OnBuildingBuilt;

		// Token: 0x140000A7 RID: 167
		// (add) Token: 0x06001A0A RID: 6666 RVA: 0x0005DF58 File Offset: 0x0005C158
		// (remove) Token: 0x06001A0B RID: 6667 RVA: 0x0005DF8C File Offset: 0x0005C18C
		public static event Action<int> OnBuildingDestroyed;

		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x06001A0C RID: 6668 RVA: 0x0005DFC0 File Offset: 0x0005C1C0
		// (remove) Token: 0x06001A0D RID: 6669 RVA: 0x0005DFF4 File Offset: 0x0005C1F4
		public static event Action<int, BuildingInfo> OnBuildingBuiltComplex;

		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x06001A0E RID: 6670 RVA: 0x0005E028 File Offset: 0x0005C228
		// (remove) Token: 0x06001A0F RID: 6671 RVA: 0x0005E05C File Offset: 0x0005C25C
		public static event Action<int, BuildingInfo> OnBuildingDestroyedComplex;

		// Token: 0x06001A10 RID: 6672 RVA: 0x0005E08F File Offset: 0x0005C28F
		private void Awake()
		{
			BuildingManager.Instance = this;
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			this.Load();
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x0005E0AE File Offset: 0x0005C2AE
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x0005E0C1 File Offset: 0x0005C2C1
		private void OnCollectSaveData()
		{
			this.Save();
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0005E0CC File Offset: 0x0005C2CC
		private void Load()
		{
			BuildingManager.SaveData saveData = SavesSystem.Load<BuildingManager.SaveData>("BuildingData");
			this.areas.Clear();
			if (saveData.data != null)
			{
				this.areas.AddRange(saveData.data);
			}
			this.tokens.Clear();
			if (saveData.tokenAmounts != null)
			{
				this.tokens.AddRange(saveData.tokenAmounts);
			}
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x0005E12C File Offset: 0x0005C32C
		private void Save()
		{
			BuildingManager.SaveData saveData = new BuildingManager.SaveData
			{
				data = new List<BuildingManager.BuildingAreaData>(this.areas),
				tokenAmounts = new List<BuildingManager.BuildingTokenAmountEntry>(this.tokens)
			};
			SavesSystem.Save<BuildingManager.SaveData>("BuildingData", saveData);
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x0005E174 File Offset: 0x0005C374
		internal static BuildingManager.BuildingAreaData GetAreaData(string areaID)
		{
			if (BuildingManager.Instance == null)
			{
				return null;
			}
			return BuildingManager.Instance.Areas.Find((BuildingManager.BuildingAreaData e) => e != null && e.AreaID == areaID);
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x0005E1B8 File Offset: 0x0005C3B8
		internal static BuildingManager.BuildingAreaData GetOrCreateAreaData(string areaID)
		{
			if (BuildingManager.Instance == null)
			{
				return null;
			}
			return BuildingManager.Instance.GetOrCreateArea(areaID);
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x0005E1D4 File Offset: 0x0005C3D4
		internal static BuildingManager.BuildingData GetBuildingData(int guid, string areaID = null)
		{
			if (areaID == null)
			{
				using (List<BuildingManager.BuildingAreaData>.Enumerator enumerator = BuildingManager.Instance.Areas.GetEnumerator())
				{
					Predicate<BuildingManager.BuildingData> <>9__0;
					while (enumerator.MoveNext())
					{
						BuildingManager.BuildingAreaData buildingAreaData = enumerator.Current;
						List<BuildingManager.BuildingData> buildings = buildingAreaData.Buildings;
						Predicate<BuildingManager.BuildingData> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = (BuildingManager.BuildingData e) => e != null && e.GUID == guid);
						}
						BuildingManager.BuildingData buildingData = buildings.Find(predicate);
						if (buildingData != null)
						{
							return buildingData;
						}
					}
					goto IL_009B;
				}
				goto IL_0074;
				IL_009B:
				return null;
			}
			IL_0074:
			BuildingManager.BuildingAreaData areaData = BuildingManager.GetAreaData(areaID);
			if (areaData == null)
			{
				return null;
			}
			return areaData.Buildings.Find((BuildingManager.BuildingData e) => e != null && e.GUID == guid);
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0005E290 File Offset: 0x0005C490
		internal static BuildingBuyAndPlaceResults BuyAndPlace(string areaID, string id, Vector2Int coord, BuildingRotation rotation)
		{
			if (BuildingManager.Instance == null)
			{
				return BuildingBuyAndPlaceResults.NoReferences;
			}
			BuildingInfo buildingInfo = BuildingManager.GetBuildingInfo(id);
			if (!buildingInfo.Valid)
			{
				return BuildingBuyAndPlaceResults.InvalidBuildingInfo;
			}
			BuildingManager.GetBuildingAmount(id);
			if (buildingInfo.ReachedAmountLimit)
			{
				return BuildingBuyAndPlaceResults.ReachedAmountLimit;
			}
			BuildingManager.Instance.GetTokenAmount(id);
			if (!BuildingManager.Instance.PayToken(id) && !buildingInfo.cost.Pay(true, true))
			{
				return BuildingBuyAndPlaceResults.PaymentFailure;
			}
			BuildingManager.BuildingAreaData orCreateArea = BuildingManager.Instance.GetOrCreateArea(areaID);
			int num = BuildingManager.GenerateBuildingGUID(id);
			orCreateArea.Add(id, rotation, coord, num);
			Action onBuildingListChanged = BuildingManager.OnBuildingListChanged;
			if (onBuildingListChanged != null)
			{
				onBuildingListChanged();
			}
			Action<int> onBuildingBuilt = BuildingManager.OnBuildingBuilt;
			if (onBuildingBuilt != null)
			{
				onBuildingBuilt(num);
			}
			Action<int, BuildingInfo> onBuildingBuiltComplex = BuildingManager.OnBuildingBuiltComplex;
			if (onBuildingBuiltComplex != null)
			{
				onBuildingBuiltComplex(num, buildingInfo);
			}
			AudioManager.Post("UI/building_up");
			return BuildingBuyAndPlaceResults.Succeed;
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x0005E358 File Offset: 0x0005C558
		internal static bool DestroyBuilding(int guid, string areaID = null)
		{
			BuildingManager.BuildingData buildingData;
			BuildingManager.BuildingAreaData buildingAreaData;
			if (!BuildingManager.TryGetBuildingDataAndAreaData(guid, out buildingData, out buildingAreaData, areaID))
			{
				return false;
			}
			buildingAreaData.Remove(buildingData);
			Action onBuildingListChanged = BuildingManager.OnBuildingListChanged;
			if (onBuildingListChanged != null)
			{
				onBuildingListChanged();
			}
			Action<int> onBuildingDestroyed = BuildingManager.OnBuildingDestroyed;
			if (onBuildingDestroyed != null)
			{
				onBuildingDestroyed(guid);
			}
			Action<int, BuildingInfo> onBuildingDestroyedComplex = BuildingManager.OnBuildingDestroyedComplex;
			if (onBuildingDestroyedComplex != null)
			{
				onBuildingDestroyedComplex(guid, buildingData.Info);
			}
			return true;
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x0005E3B8 File Offset: 0x0005C5B8
		internal static bool TryGetBuildingDataAndAreaData(int guid, out BuildingManager.BuildingData buildingData, out BuildingManager.BuildingAreaData areaData, string areaID = null)
		{
			buildingData = null;
			areaData = null;
			if (BuildingManager.Instance == null)
			{
				return false;
			}
			if (areaID == null)
			{
				using (List<BuildingManager.BuildingAreaData>.Enumerator enumerator = BuildingManager.Instance.areas.GetEnumerator())
				{
					Predicate<BuildingManager.BuildingData> <>9__0;
					while (enumerator.MoveNext())
					{
						BuildingManager.BuildingAreaData buildingAreaData = enumerator.Current;
						List<BuildingManager.BuildingData> buildings = buildingAreaData.Buildings;
						Predicate<BuildingManager.BuildingData> predicate;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = (BuildingManager.BuildingData e) => e != null && e.GUID == guid);
						}
						BuildingManager.BuildingData buildingData2 = buildings.Find(predicate);
						if (buildingData2 != null)
						{
							areaData = buildingAreaData;
							buildingData = buildingData2;
							return true;
						}
					}
					return false;
				}
			}
			BuildingManager.BuildingAreaData area = BuildingManager.Instance.GetArea(areaID);
			if (area == null)
			{
				return false;
			}
			BuildingManager.BuildingData buildingData3 = area.Buildings.Find((BuildingManager.BuildingData e) => e != null && e.GUID == guid);
			if (buildingData3 != null)
			{
				areaData = area;
				buildingData = buildingData3;
			}
			return false;
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x0005E4A8 File Offset: 0x0005C6A8
		internal static async UniTask<bool> ReturnBuilding(int guid, string areaID = null)
		{
			bool flag;
			if (BuildingManager.returningBuilding)
			{
				flag = false;
			}
			else
			{
				BuildingManager.returningBuilding = true;
				BuildingManager.BuildingData buildingData;
				BuildingManager.BuildingAreaData buildingAreaData;
				if (!BuildingManager.TryGetBuildingDataAndAreaData(guid, out buildingData, out buildingAreaData, areaID))
				{
					flag = false;
				}
				else
				{
					BuildingManager.Instance.AddToken(buildingData.ID, 1);
					buildingAreaData.Remove(buildingData);
					Action onBuildingListChanged = BuildingManager.OnBuildingListChanged;
					if (onBuildingListChanged != null)
					{
						onBuildingListChanged();
					}
					Action<int> onBuildingDestroyed = BuildingManager.OnBuildingDestroyed;
					if (onBuildingDestroyed != null)
					{
						onBuildingDestroyed(guid);
					}
					Action<int, BuildingInfo> onBuildingDestroyedComplex = BuildingManager.OnBuildingDestroyedComplex;
					if (onBuildingDestroyedComplex != null)
					{
						onBuildingDestroyedComplex(guid, buildingData.Info);
					}
					BuildingManager.returningBuilding = false;
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x0005E4F4 File Offset: 0x0005C6F4
		internal static async UniTask<int> ReturnBuildings(string areaID = null, params int[] buildings)
		{
			int count = 0;
			int[] array = buildings;
			for (int i = 0; i < array.Length; i++)
			{
				UniTask<bool>.Awaiter awaiter = BuildingManager.ReturnBuilding(array[i], areaID).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					await awaiter;
					UniTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(UniTask<bool>.Awaiter);
				}
				if (awaiter.GetResult())
				{
					count++;
				}
			}
			array = null;
			return count;
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x0005E540 File Offset: 0x0005C740
		internal static async UniTask<int> ReturnBuildingsOfType(string buildingID, string areaID = null)
		{
			int num;
			if (BuildingManager.Instance == null)
			{
				num = 0;
			}
			else
			{
				List<BuildingManager.BuildingAreaData> list = new List<BuildingManager.BuildingAreaData>();
				if (areaID != null)
				{
					BuildingManager.BuildingAreaData area = BuildingManager.Instance.GetArea(areaID);
					if (area == null)
					{
						return 0;
					}
					list.Add(area);
				}
				else
				{
					list.AddRange(BuildingManager.Instance.Areas);
				}
				BuildingManager.returningBuilding = true;
				int num2 = 0;
				Predicate<BuildingManager.BuildingData> <>9__0;
				foreach (BuildingManager.BuildingAreaData buildingAreaData in list)
				{
					List<BuildingManager.BuildingData> buildings = buildingAreaData.Buildings;
					Predicate<BuildingManager.BuildingData> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = (BuildingManager.BuildingData e) => e != null && e.ID == buildingID);
					}
					foreach (BuildingManager.BuildingData buildingData in buildings.FindAll(predicate))
					{
						BuildingManager.Instance.AddToken(buildingData.ID, 1);
						buildingAreaData.Remove(buildingData);
						Action<int> onBuildingDestroyed = BuildingManager.OnBuildingDestroyed;
						if (onBuildingDestroyed != null)
						{
							onBuildingDestroyed(buildingData.GUID);
						}
						Action<int, BuildingInfo> onBuildingDestroyedComplex = BuildingManager.OnBuildingDestroyedComplex;
						if (onBuildingDestroyedComplex != null)
						{
							onBuildingDestroyedComplex(buildingData.GUID, buildingData.Info);
						}
						num2++;
					}
				}
				Action onBuildingListChanged = BuildingManager.OnBuildingListChanged;
				if (onBuildingListChanged != null)
				{
					onBuildingListChanged();
				}
				BuildingManager.returningBuilding = false;
				num = num2;
			}
			return num;
		}

		// Token: 0x040012AE RID: 4782
		private List<BuildingManager.BuildingTokenAmountEntry> tokens = new List<BuildingManager.BuildingTokenAmountEntry>();

		// Token: 0x040012AF RID: 4783
		[SerializeField]
		private List<BuildingManager.BuildingAreaData> areas = new List<BuildingManager.BuildingAreaData>();

		// Token: 0x040012B5 RID: 4789
		private const string SaveKey = "BuildingData";

		// Token: 0x040012B6 RID: 4790
		private static bool returningBuilding;

		// Token: 0x020005A3 RID: 1443
		[Serializable]
		public class BuildingTokenAmountEntry
		{
			// Token: 0x04002023 RID: 8227
			public string id;

			// Token: 0x04002024 RID: 8228
			public int amount;
		}

		// Token: 0x020005A4 RID: 1444
		[Serializable]
		public class BuildingAreaData
		{
			// Token: 0x17000772 RID: 1906
			// (get) Token: 0x0600288A RID: 10378 RVA: 0x000961F7 File Offset: 0x000943F7
			public string AreaID
			{
				get
				{
					return this.areaID;
				}
			}

			// Token: 0x17000773 RID: 1907
			// (get) Token: 0x0600288B RID: 10379 RVA: 0x000961FF File Offset: 0x000943FF
			public List<BuildingManager.BuildingData> Buildings
			{
				get
				{
					return this.buildings;
				}
			}

			// Token: 0x0600288C RID: 10380 RVA: 0x00096208 File Offset: 0x00094408
			public bool Any(string buildingID)
			{
				foreach (BuildingManager.BuildingData buildingData in this.buildings)
				{
					if (buildingData != null)
					{
						if (buildingData.ID == buildingID)
						{
							return true;
						}
						if (buildingData.Info.alternativeFor.Contains(buildingID))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x0600288D RID: 10381 RVA: 0x00096284 File Offset: 0x00094484
			public bool Add(string buildingID, BuildingRotation rotation, Vector2Int coord, int guid = -1)
			{
				BuildingManager.GetBuildingInfo(buildingID);
				if (guid < 0)
				{
					guid = BuildingManager.GenerateBuildingGUID(buildingID);
				}
				this.buildings.Add(new BuildingManager.BuildingData(guid, buildingID, rotation, coord));
				return true;
			}

			// Token: 0x0600288E RID: 10382 RVA: 0x000962B0 File Offset: 0x000944B0
			public bool Remove(int buildingGUID)
			{
				BuildingManager.BuildingData buildingData = this.buildings.Find((BuildingManager.BuildingData e) => e != null && e.GUID == buildingGUID);
				return buildingData != null && this.buildings.Remove(buildingData);
			}

			// Token: 0x0600288F RID: 10383 RVA: 0x000962F3 File Offset: 0x000944F3
			public bool Remove(BuildingManager.BuildingData building)
			{
				return this.buildings.Remove(building);
			}

			// Token: 0x06002890 RID: 10384 RVA: 0x00096304 File Offset: 0x00094504
			public BuildingManager.BuildingData GetBuildingAt(Vector2Int coord)
			{
				foreach (BuildingManager.BuildingData buildingData in this.buildings)
				{
					if (BuildingManager.GetOccupyingCoords(buildingData.Dimensions, buildingData.Rotation, buildingData.Coord).Contains(coord))
					{
						return buildingData;
					}
				}
				return null;
			}

			// Token: 0x06002891 RID: 10385 RVA: 0x00096378 File Offset: 0x00094578
			public HashSet<Vector2Int> GetAllOccupiedCoords()
			{
				HashSet<Vector2Int> hashSet = new HashSet<Vector2Int>();
				foreach (BuildingManager.BuildingData buildingData in this.buildings)
				{
					Vector2Int[] occupyingCoords = BuildingManager.GetOccupyingCoords(buildingData.Dimensions, buildingData.Rotation, buildingData.Coord);
					hashSet.AddRange(occupyingCoords);
				}
				return hashSet;
			}

			// Token: 0x06002892 RID: 10386 RVA: 0x000963EC File Offset: 0x000945EC
			public bool Collide(Vector2Int dimensions, BuildingRotation rotation, Vector2Int coord)
			{
				HashSet<Vector2Int> allOccupiedCoords = this.GetAllOccupiedCoords();
				foreach (Vector2Int vector2Int in BuildingManager.GetOccupyingCoords(dimensions, rotation, coord))
				{
					if (allOccupiedCoords.Contains(vector2Int))
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06002893 RID: 10387 RVA: 0x0009642B File Offset: 0x0009462B
			internal bool Any(Func<BuildingManager.BuildingData, bool> predicate)
			{
				return this.buildings.Any(predicate);
			}

			// Token: 0x06002894 RID: 10388 RVA: 0x00096439 File Offset: 0x00094639
			public BuildingAreaData()
			{
			}

			// Token: 0x06002895 RID: 10389 RVA: 0x0009644C File Offset: 0x0009464C
			public BuildingAreaData(string areaID)
			{
				this.areaID = areaID;
			}

			// Token: 0x04002025 RID: 8229
			[SerializeField]
			private string areaID;

			// Token: 0x04002026 RID: 8230
			[SerializeField]
			private List<BuildingManager.BuildingData> buildings = new List<BuildingManager.BuildingData>();
		}

		// Token: 0x020005A5 RID: 1445
		[Serializable]
		public class BuildingData
		{
			// Token: 0x17000774 RID: 1908
			// (get) Token: 0x06002896 RID: 10390 RVA: 0x00096466 File Offset: 0x00094666
			public int GUID
			{
				get
				{
					return this.guid;
				}
			}

			// Token: 0x17000775 RID: 1909
			// (get) Token: 0x06002897 RID: 10391 RVA: 0x0009646E File Offset: 0x0009466E
			public string ID
			{
				get
				{
					return this.id;
				}
			}

			// Token: 0x17000776 RID: 1910
			// (get) Token: 0x06002898 RID: 10392 RVA: 0x00096478 File Offset: 0x00094678
			public Vector2Int Dimensions
			{
				get
				{
					return this.Info.Dimensions;
				}
			}

			// Token: 0x17000777 RID: 1911
			// (get) Token: 0x06002899 RID: 10393 RVA: 0x00096493 File Offset: 0x00094693
			public Vector2Int Coord
			{
				get
				{
					return this.coord;
				}
			}

			// Token: 0x17000778 RID: 1912
			// (get) Token: 0x0600289A RID: 10394 RVA: 0x0009649B File Offset: 0x0009469B
			public BuildingRotation Rotation
			{
				get
				{
					return this.rotation;
				}
			}

			// Token: 0x17000779 RID: 1913
			// (get) Token: 0x0600289B RID: 10395 RVA: 0x000964A3 File Offset: 0x000946A3
			public BuildingInfo Info
			{
				get
				{
					return BuildingDataCollection.GetInfo(this.id);
				}
			}

			// Token: 0x0600289C RID: 10396 RVA: 0x000964B0 File Offset: 0x000946B0
			public BuildingData(int guid, string id, BuildingRotation rotation, Vector2Int coord)
			{
				this.guid = guid;
				this.id = id;
				this.coord = coord;
				this.rotation = rotation;
			}

			// Token: 0x0600289D RID: 10397 RVA: 0x000964D8 File Offset: 0x000946D8
			internal Vector3 GetTransformPosition()
			{
				Vector2Int dimensions = this.Dimensions;
				if (this.rotation % BuildingRotation.Half > BuildingRotation.Zero)
				{
					dimensions = new Vector2Int(dimensions.y, dimensions.x);
				}
				return new Vector3((float)this.coord.x - 0.5f + (float)dimensions.x / 2f, 0f, (float)this.coord.y - 0.5f + (float)dimensions.y / 2f);
			}

			// Token: 0x04002027 RID: 8231
			[SerializeField]
			private int guid;

			// Token: 0x04002028 RID: 8232
			[SerializeField]
			private string id;

			// Token: 0x04002029 RID: 8233
			[SerializeField]
			private Vector2Int coord;

			// Token: 0x0400202A RID: 8234
			[SerializeField]
			private BuildingRotation rotation;
		}

		// Token: 0x020005A6 RID: 1446
		[Serializable]
		private struct SaveData
		{
			// Token: 0x0400202B RID: 8235
			[SerializeField]
			public List<BuildingManager.BuildingAreaData> data;

			// Token: 0x0400202C RID: 8236
			[SerializeField]
			public List<BuildingManager.BuildingTokenAmountEntry> tokenAmounts;
		}
	}
}
