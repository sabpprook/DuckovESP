using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.Rules.UI
{
	// Token: 0x020003F1 RID: 1009
	public class DifficultySelection_Entry : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
	{
		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x0600246E RID: 9326 RVA: 0x0007E647 File Offset: 0x0007C847
		// (set) Token: 0x0600246F RID: 9327 RVA: 0x0007E64F File Offset: 0x0007C84F
		public DifficultySelection Master { get; private set; }

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06002470 RID: 9328 RVA: 0x0007E658 File Offset: 0x0007C858
		// (set) Token: 0x06002471 RID: 9329 RVA: 0x0007E660 File Offset: 0x0007C860
		public DifficultySelection.SettingEntry Setting { get; private set; }

		// Token: 0x06002472 RID: 9330 RVA: 0x0007E669 File Offset: 0x0007C869
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.locked)
			{
				return;
			}
			this.Master.NotifySelected(this);
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x0007E680 File Offset: 0x0007C880
		public void OnPointerEnter(PointerEventData eventData)
		{
			DifficultySelection master = this.Master;
			if (master != null)
			{
				master.NotifyEntryPointerEnter(this);
			}
			Action<DifficultySelection_Entry> action = this.onPointerEnter;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x0007E6A5 File Offset: 0x0007C8A5
		public void OnPointerExit(PointerEventData eventData)
		{
			DifficultySelection master = this.Master;
			if (master != null)
			{
				master.NotifyEntryPointerExit(this);
			}
			Action<DifficultySelection_Entry> action = this.onPointerExit;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x0007E6CA File Offset: 0x0007C8CA
		internal void Refresh()
		{
			if (this.Master == null)
			{
				return;
			}
			this.selectedIndicator.SetActive(this.Master.SelectedRuleIndex == this.Setting.ruleIndex);
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x0007E700 File Offset: 0x0007C900
		internal void Setup(DifficultySelection master, DifficultySelection.SettingEntry setting, bool locked)
		{
			this.Master = master;
			this.Setting = setting;
			this.title.text = setting.Title;
			this.icon.sprite = setting.icon;
			this.recommendationIndicator.SetActive(setting.recommended);
			this.locked = locked;
			this.lockedIndicator.SetActive(locked);
			this.Refresh();
		}

		// Token: 0x040018D1 RID: 6353
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040018D2 RID: 6354
		[SerializeField]
		private Image icon;

		// Token: 0x040018D3 RID: 6355
		[SerializeField]
		private GameObject recommendationIndicator;

		// Token: 0x040018D4 RID: 6356
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x040018D5 RID: 6357
		[SerializeField]
		private GameObject lockedIndicator;

		// Token: 0x040018D6 RID: 6358
		internal Action<DifficultySelection_Entry> onPointerEnter;

		// Token: 0x040018D7 RID: 6359
		internal Action<DifficultySelection_Entry> onPointerExit;

		// Token: 0x040018DA RID: 6362
		private bool locked;
	}
}
