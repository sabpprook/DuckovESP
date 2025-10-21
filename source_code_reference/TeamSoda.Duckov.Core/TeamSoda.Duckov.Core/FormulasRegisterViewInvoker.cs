using System;
using System.Collections.Generic;
using Duckov.UI;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class FormulasRegisterViewInvoker : InteractableBase
{
	// Token: 0x06000CE0 RID: 3296 RVA: 0x000359E5 File Offset: 0x00033BE5
	protected override void Awake()
	{
		base.Awake();
		this.finishWhenTimeOut = true;
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x000359F4 File Offset: 0x00033BF4
	protected override void OnInteractFinished()
	{
		FormulasRegisterView.Show(this.additionalTags);
	}

	// Token: 0x04000B1C RID: 2844
	[SerializeField]
	private List<Tag> additionalTags;
}
