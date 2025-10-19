using System;
using System.Collections.Generic;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.Buildings.UI
{
	// Token: 0x0200031A RID: 794
	public class BuildingSelectionPanel : MonoBehaviour
	{
		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06001A5A RID: 6746 RVA: 0x0005F56C File Offset: 0x0005D76C
		private PrefabPool<BuildingBtnEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<BuildingBtnEntry>(this.buildingBtnTemplate, null, new Action<BuildingBtnEntry>(this.OnGetButtonEntry), new Action<BuildingBtnEntry>(this.OnReleaseButtonEntry), null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x0005F5BB File Offset: 0x0005D7BB
		private void OnGetButtonEntry(BuildingBtnEntry entry)
		{
			entry.onButtonClicked += this.OnButtonSelected;
			entry.onRecycleRequested += this.OnRecycleRequested;
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x0005F5E1 File Offset: 0x0005D7E1
		private void OnReleaseButtonEntry(BuildingBtnEntry entry)
		{
			entry.onButtonClicked -= this.OnButtonSelected;
			entry.onRecycleRequested -= this.OnRecycleRequested;
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x0005F607 File Offset: 0x0005D807
		private void OnRecycleRequested(BuildingBtnEntry entry)
		{
			Action<BuildingBtnEntry> action = this.onRecycleRequested;
			if (action == null)
			{
				return;
			}
			action(entry);
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0005F61A File Offset: 0x0005D81A
		private void OnButtonSelected(BuildingBtnEntry entry)
		{
			Action<BuildingBtnEntry> action = this.onButtonSelected;
			if (action == null)
			{
				return;
			}
			action(entry);
		}

		// Token: 0x140000AD RID: 173
		// (add) Token: 0x06001A5F RID: 6751 RVA: 0x0005F630 File Offset: 0x0005D830
		// (remove) Token: 0x06001A60 RID: 6752 RVA: 0x0005F668 File Offset: 0x0005D868
		public event Action<BuildingBtnEntry> onButtonSelected;

		// Token: 0x140000AE RID: 174
		// (add) Token: 0x06001A61 RID: 6753 RVA: 0x0005F6A0 File Offset: 0x0005D8A0
		// (remove) Token: 0x06001A62 RID: 6754 RVA: 0x0005F6D8 File Offset: 0x0005D8D8
		public event Action<BuildingBtnEntry> onRecycleRequested;

		// Token: 0x06001A63 RID: 6755 RVA: 0x0005F70D File Offset: 0x0005D90D
		public void Show()
		{
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0005F70F File Offset: 0x0005D90F
		internal void Setup(BuildingArea targetArea)
		{
			this.targetArea = targetArea;
			this.Refresh();
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0005F720 File Offset: 0x0005D920
		public void Refresh()
		{
			this.Pool.ReleaseAll();
			foreach (BuildingInfo buildingInfo in BuildingSelectionPanel.GetBuildingsToDisplay())
			{
				BuildingBtnEntry buildingBtnEntry = this.Pool.Get(null);
				buildingBtnEntry.Setup(buildingInfo);
				buildingBtnEntry.transform.SetAsLastSibling();
			}
			foreach (BuildingBtnEntry buildingBtnEntry2 in this.Pool.ActiveEntries)
			{
				if (!buildingBtnEntry2.CostEnough)
				{
					buildingBtnEntry2.transform.SetAsLastSibling();
				}
			}
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0005F7C8 File Offset: 0x0005D9C8
		public static BuildingInfo[] GetBuildingsToDisplay()
		{
			BuildingDataCollection instance = BuildingDataCollection.Instance;
			if (instance == null)
			{
				return new BuildingInfo[0];
			}
			List<BuildingInfo> list = new List<BuildingInfo>();
			foreach (BuildingInfo buildingInfo in instance.Infos)
			{
				if (buildingInfo.CurrentAmount > 0 || buildingInfo.RequirementsSatisfied())
				{
					list.Add(buildingInfo);
				}
			}
			return list.ToArray();
		}

		// Token: 0x040012EE RID: 4846
		[SerializeField]
		private BuildingBtnEntry buildingBtnTemplate;

		// Token: 0x040012EF RID: 4847
		private PrefabPool<BuildingBtnEntry> _pool;

		// Token: 0x040012F0 RID: 4848
		private BuildingArea targetArea;
	}
}
