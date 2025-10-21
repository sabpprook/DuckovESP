using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Buildings
{
	// Token: 0x02000310 RID: 784
	[CreateAssetMenu]
	public class BuildingDataCollection : ScriptableObject
	{
		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x060019DA RID: 6618 RVA: 0x0005D658 File Offset: 0x0005B858
		public static BuildingDataCollection Instance
		{
			get
			{
				return GameplayDataSettings.BuildingDataCollection;
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x060019DB RID: 6619 RVA: 0x0005D65F File Offset: 0x0005B85F
		public ReadOnlyCollection<BuildingInfo> Infos
		{
			get
			{
				if (this.readonlyInfos == null)
				{
					this.readonlyInfos = new ReadOnlyCollection<BuildingInfo>(this.infos);
				}
				return this.readonlyInfos;
			}
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x0005D680 File Offset: 0x0005B880
		internal static BuildingInfo GetInfo(string id)
		{
			if (BuildingDataCollection.Instance == null)
			{
				return default(BuildingInfo);
			}
			return BuildingDataCollection.Instance.infos.FirstOrDefault((BuildingInfo e) => e.id == id);
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x0005D6CC File Offset: 0x0005B8CC
		internal static Building GetPrefab(string prefabName)
		{
			if (BuildingDataCollection.Instance == null)
			{
				return null;
			}
			return BuildingDataCollection.Instance.prefabs.FirstOrDefault((Building e) => e != null && e.name == prefabName);
		}

		// Token: 0x0400129F RID: 4767
		[SerializeField]
		private List<BuildingInfo> infos = new List<BuildingInfo>();

		// Token: 0x040012A0 RID: 4768
		[SerializeField]
		private List<Building> prefabs;

		// Token: 0x040012A1 RID: 4769
		public ReadOnlyCollection<BuildingInfo> readonlyInfos;
	}
}
