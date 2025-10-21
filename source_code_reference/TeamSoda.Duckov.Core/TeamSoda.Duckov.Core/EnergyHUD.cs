using System;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000BA RID: 186
public class EnergyHUD : MonoBehaviour
{
	// Token: 0x17000127 RID: 295
	// (get) Token: 0x0600060C RID: 1548 RVA: 0x0001B282 File Offset: 0x00019482
	private Item item
	{
		get
		{
			return this.characterMainControl.CharacterItem;
		}
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0001B290 File Offset: 0x00019490
	private void Update()
	{
		if (!this.characterMainControl)
		{
			this.characterMainControl = LevelManager.Instance.MainCharacter;
			if (!this.characterMainControl)
			{
				return;
			}
		}
		float num = this.characterMainControl.CurrentEnergy / this.characterMainControl.MaxEnergy;
		if (!Mathf.Approximately(num, this.percent))
		{
			this.percent = num;
			this.fillImage.fillAmount = this.percent;
			if (this.percent <= 0f)
			{
				this.backgroundImage.color = this.emptyBackgroundColor;
				return;
			}
			this.backgroundImage.color = this.backgroundColor;
		}
	}

	// Token: 0x040005A2 RID: 1442
	private CharacterMainControl characterMainControl;

	// Token: 0x040005A3 RID: 1443
	private float percent = -1f;

	// Token: 0x040005A4 RID: 1444
	public ProceduralImage fillImage;

	// Token: 0x040005A5 RID: 1445
	public ProceduralImage backgroundImage;

	// Token: 0x040005A6 RID: 1446
	public Color backgroundColor;

	// Token: 0x040005A7 RID: 1447
	public Color emptyBackgroundColor;
}
