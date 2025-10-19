using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.MasterKeys.UI
{
	// Token: 0x020002DC RID: 732
	public class MasterKeysIndexEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x0600175B RID: 5979 RVA: 0x00055C08 File Offset: 0x00053E08
		public int ItemID
		{
			get
			{
				return this.itemID;
			}
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x0600175C RID: 5980 RVA: 0x00055C10 File Offset: 0x00053E10
		public string DisplayName
		{
			get
			{
				if (this.status == null)
				{
					return "???";
				}
				if (!this.status.active)
				{
					return "???";
				}
				return this.metaData.DisplayName;
			}
		}

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x0600175D RID: 5981 RVA: 0x00055C3E File Offset: 0x00053E3E
		public Sprite Icon
		{
			get
			{
				if (this.status == null)
				{
					return this.undiscoveredIcon;
				}
				if (!this.status.active)
				{
					return this.undiscoveredIcon;
				}
				return this.metaData.icon;
			}
		}

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x0600175E RID: 5982 RVA: 0x00055C6E File Offset: 0x00053E6E
		public string Description
		{
			get
			{
				if (this.status == null)
				{
					return "???";
				}
				if (!this.status.active)
				{
					return "???";
				}
				return this.metaData.Description;
			}
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x00055C9C File Offset: 0x00053E9C
		public bool Active
		{
			get
			{
				return this.status != null && this.status.active;
			}
		}

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x06001760 RID: 5984 RVA: 0x00055CB4 File Offset: 0x00053EB4
		// (remove) Token: 0x06001761 RID: 5985 RVA: 0x00055CEC File Offset: 0x00053EEC
		internal event Action<MasterKeysIndexEntry> onPointerClicked;

		// Token: 0x06001762 RID: 5986 RVA: 0x00055D21 File Offset: 0x00053F21
		public void Setup(int itemID, ISingleSelectionMenu<MasterKeysIndexEntry> menu)
		{
			this.itemID = itemID;
			this.metaData = ItemAssetsCollection.GetMetaData(itemID);
			this.menu = menu;
			this.Refresh();
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x00055D44 File Offset: 0x00053F44
		private void SetupNotDiscovered()
		{
			this.icon.sprite = (this.undiscoveredIcon ? this.undiscoveredIcon : this.metaData.icon);
			this.notDiscoveredLook.ApplyTo(this.icon);
			this.nameText.text = "???";
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x00055D9D File Offset: 0x00053F9D
		private void SetupActive()
		{
			this.icon.sprite = this.metaData.icon;
			this.activeLook.ApplyTo(this.icon);
			this.nameText.text = this.metaData.DisplayName;
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x00055DDC File Offset: 0x00053FDC
		private void Refresh()
		{
			this.status = MasterKeysManager.GetStatus(this.itemID);
			if (this.status != null)
			{
				if (this.status.active)
				{
					this.SetupActive();
					return;
				}
			}
			else
			{
				this.SetupNotDiscovered();
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x00055E11 File Offset: 0x00054011
		public void OnPointerClick(PointerEventData eventData)
		{
			this.Refresh();
			ISingleSelectionMenu<MasterKeysIndexEntry> singleSelectionMenu = this.menu;
			if (singleSelectionMenu != null)
			{
				singleSelectionMenu.SetSelection(this);
			}
			Action<MasterKeysIndexEntry> action = this.onPointerClicked;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x04001114 RID: 4372
		[SerializeField]
		private Image icon;

		// Token: 0x04001115 RID: 4373
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04001116 RID: 4374
		[SerializeField]
		private MasterKeysIndexEntry.Look notDiscoveredLook;

		// Token: 0x04001117 RID: 4375
		[SerializeField]
		private MasterKeysIndexEntry.Look activeLook;

		// Token: 0x04001118 RID: 4376
		[SerializeField]
		private Sprite undiscoveredIcon;

		// Token: 0x04001119 RID: 4377
		[ItemTypeID]
		private int itemID;

		// Token: 0x0400111A RID: 4378
		private ItemMetaData metaData;

		// Token: 0x0400111B RID: 4379
		private MasterKeysManager.Status status;

		// Token: 0x0400111D RID: 4381
		private ISingleSelectionMenu<MasterKeysIndexEntry> menu;

		// Token: 0x02000577 RID: 1399
		[Serializable]
		public struct Look
		{
			// Token: 0x06002833 RID: 10291 RVA: 0x000945A1 File Offset: 0x000927A1
			public void ApplyTo(Graphic graphic)
			{
				graphic.material = this.material;
				graphic.color = this.color;
			}

			// Token: 0x04001F8B RID: 8075
			public Color color;

			// Token: 0x04001F8C RID: 8076
			public Material material;
		}
	}
}
