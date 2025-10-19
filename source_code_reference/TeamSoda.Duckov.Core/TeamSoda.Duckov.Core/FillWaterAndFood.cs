using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class FillWaterAndFood : MonoBehaviour
{
	// Token: 0x0600054F RID: 1359 RVA: 0x00017C50 File Offset: 0x00015E50
	public void Fill()
	{
		CharacterMainControl main = CharacterMainControl.Main;
		if (!main)
		{
			return;
		}
		main.AddWater(this.water);
		main.AddEnergy(this.food);
	}

	// Token: 0x040004C5 RID: 1221
	public float water;

	// Token: 0x040004C6 RID: 1222
	public float food;
}
