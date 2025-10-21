using System;
using Duckov.Utilities;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.Quests.UI
{
	// Token: 0x02000342 RID: 834
	public class QuestEntry : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x06001CAE RID: 7342 RVA: 0x000670FC File Offset: 0x000652FC
		public Quest Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x140000CB RID: 203
		// (add) Token: 0x06001CAF RID: 7343 RVA: 0x00067104 File Offset: 0x00065304
		// (remove) Token: 0x06001CB0 RID: 7344 RVA: 0x0006713C File Offset: 0x0006533C
		public event Action<QuestEntry, PointerEventData> onClick;

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x06001CB1 RID: 7345 RVA: 0x00067171 File Offset: 0x00065371
		public bool Selected
		{
			get
			{
				return this.menu.GetSelection() == this;
			}
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x00067184 File Offset: 0x00065384
		public void NotifyPooled()
		{
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x00067186 File Offset: 0x00065386
		public void NotifyReleased()
		{
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x00067195 File Offset: 0x00065395
		private void OnDestroy()
		{
			this.UnregisterEvents();
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x0006719D File Offset: 0x0006539D
		internal void Setup(Quest quest)
		{
			this.UnregisterEvents();
			this.target = quest;
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x000671B8 File Offset: 0x000653B8
		internal void SetMenu(ISingleSelectionMenu<QuestEntry> menu)
		{
			this.menu = menu;
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x000671C1 File Offset: 0x000653C1
		private void RegisterEvents()
		{
			if (this.target != null)
			{
				this.target.onStatusChanged += this.OnTargetStatusChanged;
				this.target.onNeedInspectionChanged += this.OnNeedInspectionChanged;
			}
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x000671FF File Offset: 0x000653FF
		private void UnregisterEvents()
		{
			if (this.target != null)
			{
				this.target.onStatusChanged -= this.OnTargetStatusChanged;
				this.target.onNeedInspectionChanged -= this.OnNeedInspectionChanged;
			}
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x0006723D File Offset: 0x0006543D
		private void OnNeedInspectionChanged(Quest obj)
		{
			this.Refresh();
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x00067245 File Offset: 0x00065445
		private void OnTargetStatusChanged(Quest quest)
		{
			this.Refresh();
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x0006724D File Offset: 0x0006544D
		private void OnMasterSelectionChanged(QuestView view, Quest oldSelection, Quest newSelection)
		{
			this.Refresh();
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x00067258 File Offset: 0x00065458
		private void Refresh()
		{
			this.selectionIndicator.SetActive(this.Selected);
			this.displayName.text = this.target.DisplayName;
			this.questIDDisplay.text = string.Format("{0:0000}", this.target.ID);
			SceneInfoEntry requireSceneInfo = this.target.RequireSceneInfo;
			if (requireSceneInfo == null)
			{
				this.locationName.text = this.anyLocationKey.ToPlainText();
			}
			else
			{
				this.locationName.text = requireSceneInfo.DisplayName;
			}
			this.redDot.SetActive(this.target.NeedInspection);
			this.claimableIndicator.SetActive(this.target.Complete || this.target.AreTasksFinished());
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x00067325 File Offset: 0x00065525
		public void OnPointerClick(PointerEventData eventData)
		{
			Action<QuestEntry, PointerEventData> action = this.onClick;
			if (action != null)
			{
				action(this, eventData);
			}
			this.menu.SetSelection(this);
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x00067347 File Offset: 0x00065547
		public void NotifyRefresh()
		{
			this.Refresh();
		}

		// Token: 0x040013EF RID: 5103
		private ISingleSelectionMenu<QuestEntry> menu;

		// Token: 0x040013F0 RID: 5104
		private Quest target;

		// Token: 0x040013F1 RID: 5105
		[SerializeField]
		private GameObject selectionIndicator;

		// Token: 0x040013F2 RID: 5106
		[SerializeField]
		private TextMeshProUGUI displayName;

		// Token: 0x040013F3 RID: 5107
		[SerializeField]
		private TextMeshProUGUI locationName;

		// Token: 0x040013F4 RID: 5108
		[SerializeField]
		[LocalizationKey("Default")]
		private string anyLocationKey;

		// Token: 0x040013F5 RID: 5109
		[SerializeField]
		private GameObject redDot;

		// Token: 0x040013F6 RID: 5110
		[SerializeField]
		private GameObject claimableIndicator;

		// Token: 0x040013F7 RID: 5111
		[SerializeField]
		private TextMeshProUGUI questIDDisplay;
	}
}
