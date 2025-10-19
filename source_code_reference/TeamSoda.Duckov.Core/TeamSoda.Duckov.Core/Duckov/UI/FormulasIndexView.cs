using System;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x02000381 RID: 897
	public class FormulasIndexView : View, ISingleSelectionMenu<FormulasIndexEntry>
	{
		// Token: 0x170005FA RID: 1530
		// (get) Token: 0x06001F07 RID: 7943 RVA: 0x0006CB84 File Offset: 0x0006AD84
		private PrefabPool<FormulasIndexEntry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<FormulasIndexEntry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x0006CBBD File Offset: 0x0006ADBD
		public FormulasIndexEntry GetSelection()
		{
			return this.selectedEntry;
		}

		// Token: 0x170005FB RID: 1531
		// (get) Token: 0x06001F09 RID: 7945 RVA: 0x0006CBC5 File Offset: 0x0006ADC5
		public static FormulasIndexView Instance
		{
			get
			{
				return View.GetViewInstance<FormulasIndexView>();
			}
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x0006CBCC File Offset: 0x0006ADCC
		protected override void Awake()
		{
			base.Awake();
		}

		// Token: 0x06001F0B RID: 7947 RVA: 0x0006CBD4 File Offset: 0x0006ADD4
		public static void Show()
		{
			if (FormulasIndexView.Instance == null)
			{
				return;
			}
			FormulasIndexView.Instance.Open(null);
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x0006CBEF File Offset: 0x0006ADEF
		public bool SetSelection(FormulasIndexEntry selection)
		{
			this.selectedEntry = selection;
			return true;
		}

		// Token: 0x06001F0D RID: 7949 RVA: 0x0006CBFC File Offset: 0x0006ADFC
		protected override void OnOpen()
		{
			base.OnOpen();
			this.selectedEntry = null;
			this.Pool.ReleaseAll();
			foreach (CraftingFormula craftingFormula in CraftingFormulaCollection.Instance.Entries)
			{
				if (!craftingFormula.hideInIndex && (!GameMetaData.Instance.IsDemo || !craftingFormula.lockInDemo))
				{
					this.Pool.Get(null).Setup(this, craftingFormula);
				}
			}
			this.RefreshDetails();
			this.fadeGroup.Show();
		}

		// Token: 0x06001F0E RID: 7950 RVA: 0x0006CCA0 File Offset: 0x0006AEA0
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06001F0F RID: 7951 RVA: 0x0006CCB4 File Offset: 0x0006AEB4
		internal void OnEntryClicked(FormulasIndexEntry entry)
		{
			FormulasIndexEntry formulasIndexEntry = this.selectedEntry;
			this.selectedEntry = entry;
			this.selectedEntry.Refresh();
			if (formulasIndexEntry)
			{
				formulasIndexEntry.Refresh();
			}
			this.RefreshDetails();
		}

		// Token: 0x06001F10 RID: 7952 RVA: 0x0006CCF0 File Offset: 0x0006AEF0
		private void RefreshDetails()
		{
			if (this.selectedEntry && this.selectedEntry.Valid)
			{
				this.detailsDisplay.Setup(new CraftingFormula?(this.selectedEntry.Formula));
				return;
			}
			this.detailsDisplay.Setup(null);
		}

		// Token: 0x0400153B RID: 5435
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400153C RID: 5436
		[SerializeField]
		private FormulasIndexEntry entryTemplate;

		// Token: 0x0400153D RID: 5437
		[SerializeField]
		private FormulasDetailsDisplay detailsDisplay;

		// Token: 0x0400153E RID: 5438
		private PrefabPool<FormulasIndexEntry> _pool;

		// Token: 0x0400153F RID: 5439
		private FormulasIndexEntry selectedEntry;
	}
}
