using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.Options.UI
{
	// Token: 0x0200025B RID: 603
	public class OptionsPanel : UIPanel, ISingleSelectionMenu<OptionsPanel_TabButton>
	{
		// Token: 0x060012B7 RID: 4791 RVA: 0x000466A4 File Offset: 0x000448A4
		private void Start()
		{
			this.Setup();
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000466AC File Offset: 0x000448AC
		private void Setup()
		{
			foreach (OptionsPanel_TabButton optionsPanel_TabButton in this.tabButtons)
			{
				optionsPanel_TabButton.onClicked = (Action<OptionsPanel_TabButton, PointerEventData>)Delegate.Combine(optionsPanel_TabButton.onClicked, new Action<OptionsPanel_TabButton, PointerEventData>(this.OnTabButtonClicked));
			}
			if (this.selection == null)
			{
				this.selection = this.tabButtons[0];
			}
			this.SetSelection(this.selection);
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x00046748 File Offset: 0x00044948
		private void OnTabButtonClicked(OptionsPanel_TabButton button, PointerEventData data)
		{
			data.Use();
			this.SetSelection(button);
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x00046758 File Offset: 0x00044958
		protected override void OnOpen()
		{
			base.OnOpen();
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x00046760 File Offset: 0x00044960
		public OptionsPanel_TabButton GetSelection()
		{
			return this.selection;
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x00046768 File Offset: 0x00044968
		public bool SetSelection(OptionsPanel_TabButton selection)
		{
			this.selection = selection;
			foreach (OptionsPanel_TabButton optionsPanel_TabButton in this.tabButtons)
			{
				optionsPanel_TabButton.NotifySelectionChanged(this, selection);
			}
			return true;
		}

		// Token: 0x04000E1E RID: 3614
		[SerializeField]
		private List<OptionsPanel_TabButton> tabButtons;

		// Token: 0x04000E1F RID: 3615
		private OptionsPanel_TabButton selection;
	}
}
