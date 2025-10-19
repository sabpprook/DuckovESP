using System;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000BE RID: 190
public class WaterHUD : MonoBehaviour
{
	// Token: 0x1700012A RID: 298
	// (get) Token: 0x0600061C RID: 1564 RVA: 0x0001B667 File Offset: 0x00019867
	private Item item
	{
		get
		{
			return this.characterMainControl.CharacterItem;
		}
	}

	// Token: 0x0600061D RID: 1565 RVA: 0x0001B674 File Offset: 0x00019874
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
		float num = this.characterMainControl.CurrentWater / this.characterMainControl.MaxWater;
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

	// Token: 0x040005BA RID: 1466
	private CharacterMainControl characterMainControl;

	// Token: 0x040005BB RID: 1467
	private float percent = -1f;

	// Token: 0x040005BC RID: 1468
	public ProceduralImage fillImage;

	// Token: 0x040005BD RID: 1469
	public ProceduralImage backgroundImage;

	// Token: 0x040005BE RID: 1470
	public Color backgroundColor;

	// Token: 0x040005BF RID: 1471
	public Color emptyBackgroundColor;
}
