using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x02000380 RID: 896
	public class FormulasIndexEntry : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06001EFE RID: 7934 RVA: 0x0006CA44 File Offset: 0x0006AC44
		public CraftingFormula Formula
		{
			get
			{
				return this.formula;
			}
		}

		// Token: 0x170005F6 RID: 1526
		// (get) Token: 0x06001EFF RID: 7935 RVA: 0x0006CA4C File Offset: 0x0006AC4C
		private int ItemID
		{
			get
			{
				return this.formula.result.id;
			}
		}

		// Token: 0x170005F7 RID: 1527
		// (get) Token: 0x06001F00 RID: 7936 RVA: 0x0006CA5E File Offset: 0x0006AC5E
		private ItemMetaData Meta
		{
			get
			{
				return ItemAssetsCollection.GetMetaData(this.ItemID);
			}
		}

		// Token: 0x170005F8 RID: 1528
		// (get) Token: 0x06001F01 RID: 7937 RVA: 0x0006CA6B File Offset: 0x0006AC6B
		private bool Unlocked
		{
			get
			{
				return CraftingManager.IsFormulaUnlocked(this.formula.id);
			}
		}

		// Token: 0x170005F9 RID: 1529
		// (get) Token: 0x06001F02 RID: 7938 RVA: 0x0006CA7D File Offset: 0x0006AC7D
		public bool Valid
		{
			get
			{
				return this.ItemID >= 0;
			}
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x0006CA8B File Offset: 0x0006AC8B
		public void OnPointerClick(PointerEventData eventData)
		{
			this.master.OnEntryClicked(this);
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x0006CA99 File Offset: 0x0006AC99
		internal void Setup(FormulasIndexView master, CraftingFormula formula)
		{
			this.master = master;
			this.formula = formula;
			this.Refresh();
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x0006CAB0 File Offset: 0x0006ACB0
		public void Refresh()
		{
			ItemMetaData meta = this.Meta;
			if (!this.Valid)
			{
				this.displayNameText.text = "! " + this.formula.id + " !";
				this.image.sprite = this.lockedImage;
				return;
			}
			if (this.Unlocked)
			{
				this.displayNameText.text = string.Format("{0} x{1}", meta.DisplayName, this.formula.result.amount);
				this.image.sprite = meta.icon;
				return;
			}
			this.displayNameText.text = this.lockedText;
			this.image.sprite = this.lockedImage;
		}

		// Token: 0x04001535 RID: 5429
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x04001536 RID: 5430
		[SerializeField]
		private Image image;

		// Token: 0x04001537 RID: 5431
		[SerializeField]
		private string lockedText = "???";

		// Token: 0x04001538 RID: 5432
		[SerializeField]
		private Sprite lockedImage;

		// Token: 0x04001539 RID: 5433
		private FormulasIndexView master;

		// Token: 0x0400153A RID: 5434
		private CraftingFormula formula;
	}
}
