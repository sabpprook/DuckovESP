using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cysharp.Threading.Tasks;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class MultiInteraction : MonoBehaviour
{
	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x0003ABF2 File Offset: 0x00038DF2
	public ReadOnlyCollection<InteractableBase> Interactables
	{
		get
		{
			return this.interactables.AsReadOnly();
		}
	}

	// Token: 0x06000EBA RID: 3770 RVA: 0x0003ABFF File Offset: 0x00038DFF
	private void OnTriggerEnter(Collider other)
	{
		if (CharacterMainControl.Main.gameObject == other.gameObject)
		{
			MultiInteractionMenu instance = MultiInteractionMenu.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SetupAndShow(this).Forget();
		}
	}

	// Token: 0x06000EBB RID: 3771 RVA: 0x0003AC2D File Offset: 0x00038E2D
	private void OnTriggerExit(Collider other)
	{
		if (CharacterMainControl.Main.gameObject == other.gameObject)
		{
			MultiInteractionMenu instance = MultiInteractionMenu.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Hide().Forget();
		}
	}

	// Token: 0x04000C2F RID: 3119
	[SerializeField]
	private List<InteractableBase> interactables;
}
