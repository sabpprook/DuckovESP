using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class MiningMachineCardDisplay : MonoBehaviour
{
	// Token: 0x06000B86 RID: 2950 RVA: 0x00030C78 File Offset: 0x0002EE78
	public void SetVisualActive(bool active, MiningMachineCardDisplay.CardTypes cardType)
	{
		this.activeVisual.SetActive(active);
		this.deactiveVisual.SetActive(!active);
		if (cardType == MiningMachineCardDisplay.CardTypes.normal)
		{
			this.normalGPU.SetActive(true);
			this.potatoGPU.SetActive(false);
			return;
		}
		if (cardType != MiningMachineCardDisplay.CardTypes.potato)
		{
			throw new ArgumentOutOfRangeException("cardType", cardType, null);
		}
		this.normalGPU.SetActive(false);
		this.potatoGPU.SetActive(true);
	}

	// Token: 0x040009D5 RID: 2517
	public GameObject activeVisual;

	// Token: 0x040009D6 RID: 2518
	public GameObject deactiveVisual;

	// Token: 0x040009D7 RID: 2519
	public GameObject normalGPU;

	// Token: 0x040009D8 RID: 2520
	public GameObject potatoGPU;

	// Token: 0x020004B5 RID: 1205
	public enum CardTypes
	{
		// Token: 0x04001C70 RID: 7280
		normal,
		// Token: 0x04001C71 RID: 7281
		potato
	}
}
