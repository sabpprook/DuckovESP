using System;
using Duckov.Buffs;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class StormWeather : MonoBehaviour
{
	// Token: 0x06000BC3 RID: 3011 RVA: 0x00031E2C File Offset: 0x0003002C
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		SubSceneEntry subSceneInfo = MultiSceneCore.Instance.GetSubSceneInfo();
		if (this.onlyOutDoor && subSceneInfo.IsInDoor)
		{
			return;
		}
		if (!this.target)
		{
			this.target = CharacterMainControl.Main;
			if (!this.target)
			{
				return;
			}
		}
		this.addBuffTimer -= Time.deltaTime;
		if (this.addBuffTimer <= 0f)
		{
			this.addBuffTimer = this.addBuffTimeSpace;
			if (this.target.StormProtection > this.stormProtectionThreshold)
			{
				return;
			}
			this.target.AddBuff(this.buff, null, 0);
		}
	}

	// Token: 0x04000A19 RID: 2585
	public Buff buff;

	// Token: 0x04000A1A RID: 2586
	public float addBuffTimeSpace = 1f;

	// Token: 0x04000A1B RID: 2587
	private float addBuffTimer;

	// Token: 0x04000A1C RID: 2588
	private CharacterMainControl target;

	// Token: 0x04000A1D RID: 2589
	private bool onlyOutDoor = true;

	// Token: 0x04000A1E RID: 2590
	public float stormProtectionThreshold = 0.9f;
}
