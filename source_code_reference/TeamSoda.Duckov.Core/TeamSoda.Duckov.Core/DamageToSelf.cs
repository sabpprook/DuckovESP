using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
public class DamageToSelf : MonoBehaviour
{
	// Token: 0x060009F4 RID: 2548 RVA: 0x0002AB36 File Offset: 0x00028D36
	private void Start()
	{
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x0002AB38 File Offset: 0x00028D38
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			this.dmg.fromCharacter = CharacterMainControl.Main;
			CharacterMainControl.Main.Health.Hurt(this.dmg);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			float value = CharacterMainControl.Main.CharacterItem.GetStat("InventoryCapacity").Value;
			Debug.Log(string.Format("InventorySize:{0}", value));
		}
	}

	// Token: 0x040008BB RID: 2235
	public DamageInfo dmg;
}
