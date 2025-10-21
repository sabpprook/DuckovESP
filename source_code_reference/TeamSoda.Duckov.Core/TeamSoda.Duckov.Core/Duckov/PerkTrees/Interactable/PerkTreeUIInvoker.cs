using System;
using Duckov.UI;

namespace Duckov.PerkTrees.Interactable
{
	// Token: 0x02000259 RID: 601
	public class PerkTreeUIInvoker : InteractableBase
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x00046484 File Offset: 0x00044684
		protected override bool ShowUnityEvents
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00046487 File Offset: 0x00044687
		protected override void OnInteractStart(CharacterMainControl interactCharacter)
		{
			PerkTreeView.Show(PerkTreeManager.GetPerkTree(this.perkTreeID));
			base.StopInteract();
		}

		// Token: 0x04000E1A RID: 3610
		public string perkTreeID;
	}
}
