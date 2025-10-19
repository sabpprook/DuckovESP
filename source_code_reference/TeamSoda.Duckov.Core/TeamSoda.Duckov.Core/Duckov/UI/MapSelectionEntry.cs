using System;
using Duckov.Economy;
using Duckov.Quests;
using Duckov.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x020003B4 RID: 948
	public class MapSelectionEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x06002258 RID: 8792 RVA: 0x00078356 File Offset: 0x00076556
		public Cost Cost
		{
			get
			{
				return this.cost;
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06002259 RID: 8793 RVA: 0x0007835E File Offset: 0x0007655E
		public bool ConditionsSatisfied
		{
			get
			{
				return this.conditions == null || this.conditions.Satisfied();
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x0600225A RID: 8794 RVA: 0x00078375 File Offset: 0x00076575
		public string SceneID
		{
			get
			{
				return this.sceneID;
			}
		}

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x0600225B RID: 8795 RVA: 0x0007837D File Offset: 0x0007657D
		public int BeaconIndex
		{
			get
			{
				return this.beaconIndex;
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x0600225C RID: 8796 RVA: 0x00078385 File Offset: 0x00076585
		public Sprite FullScreenImage
		{
			get
			{
				return this.fullScreenImage;
			}
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x0007838D File Offset: 0x0007658D
		public void Setup(MapSelectionView master)
		{
			this.master = master;
			this.Refresh();
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x0007839C File Offset: 0x0007659C
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x000783A4 File Offset: 0x000765A4
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!this.ConditionsSatisfied)
			{
				return;
			}
			this.master.NotifyEntryClicked(this, eventData);
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x000783BC File Offset: 0x000765BC
		private void Refresh()
		{
			SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(this.sceneID);
			this.displayNameText.text = sceneInfo.DisplayName;
			this.lockedIndicator.gameObject.SetActive(!this.ConditionsSatisfied);
			this.costDisplay.Setup(this.cost, 1);
			this.costDisplay.gameObject.SetActive(!this.cost.IsFree);
		}

		// Token: 0x04001752 RID: 5970
		[SerializeField]
		private MapSelectionView master;

		// Token: 0x04001753 RID: 5971
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x04001754 RID: 5972
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x04001755 RID: 5973
		[SerializeField]
		private GameObject lockedIndicator;

		// Token: 0x04001756 RID: 5974
		[SerializeField]
		private Condition[] conditions;

		// Token: 0x04001757 RID: 5975
		[SerializeField]
		private Cost cost;

		// Token: 0x04001758 RID: 5976
		[SerializeField]
		[SceneID]
		private string sceneID;

		// Token: 0x04001759 RID: 5977
		[SerializeField]
		private int beaconIndex;

		// Token: 0x0400175A RID: 5978
		[SerializeField]
		private Sprite fullScreenImage;
	}
}
