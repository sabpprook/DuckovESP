using System;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class ItemTest : MonoBehaviour
{
	// Token: 0x06000CAB RID: 3243 RVA: 0x00035205 File Offset: 0x00033405
	public void DoInstantiate()
	{
		this.characterInstance = this.characterTemplate.CreateInstance();
		this.swordInstance = this.swordTemplate.CreateInstance();
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0003522C File Offset: 0x0003342C
	public void EquipSword()
	{
		Item item;
		this.characterInstance.Slots["Weapon"].Plug(this.swordInstance, out item);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0003525C File Offset: 0x0003345C
	public void UequipSword()
	{
		this.characterInstance.Slots["Weapon"].Unplug();
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00035279 File Offset: 0x00033479
	public void DestroyInstances()
	{
		if (this.characterInstance)
		{
			this.characterInstance.DestroyTreeImmediate();
		}
		if (this.swordInstance)
		{
			this.swordInstance.DestroyTreeImmediate();
		}
	}

	// Token: 0x04000AFA RID: 2810
	public Item characterTemplate;

	// Token: 0x04000AFB RID: 2811
	public Item swordTemplate;

	// Token: 0x04000AFC RID: 2812
	public Item characterInstance;

	// Token: 0x04000AFD RID: 2813
	public Item swordInstance;
}
