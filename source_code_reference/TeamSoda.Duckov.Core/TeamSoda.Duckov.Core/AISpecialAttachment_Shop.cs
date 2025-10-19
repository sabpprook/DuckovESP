using System;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class AISpecialAttachment_Shop : AISpecialAttachmentBase
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x000160C2 File Offset: 0x000142C2
	protected override void OnInited()
	{
		base.OnInited();
		this.aiCharacterController.hideIfFoundEnemy = this.shop;
	}

	// Token: 0x0400041A RID: 1050
	public GameObject shop;
}
