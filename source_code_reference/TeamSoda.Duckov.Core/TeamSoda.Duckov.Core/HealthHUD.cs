using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000BB RID: 187
public class HealthHUD : MonoBehaviour
{
	// Token: 0x17000128 RID: 296
	// (get) Token: 0x0600060F RID: 1551 RVA: 0x0001B349 File Offset: 0x00019549
	private Item item
	{
		get
		{
			return this.characterMainControl.CharacterItem;
		}
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0001B358 File Offset: 0x00019558
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
		float num = this.characterMainControl.Health.MaxHealth;
		float currentHealth = this.characterMainControl.Health.CurrentHealth;
		float num2 = currentHealth / num;
		if (!Mathf.Approximately(num2, this.percent))
		{
			this.percent = num2;
			this.fillImage.fillAmount = this.percent;
			if (this.percent <= 0f)
			{
				this.backgroundImage.color = this.emptyBackgroundColor;
			}
			else
			{
				this.backgroundImage.color = this.backgroundColor;
			}
		}
		if (num != this.maxHealth || currentHealth != this.currenthealth)
		{
			this.maxHealth = num;
			this.currenthealth = currentHealth;
			this.text.text = this.currenthealth.ToString("0.#") + " / " + this.maxHealth.ToString("0.#");
		}
	}

	// Token: 0x040005A8 RID: 1448
	private CharacterMainControl characterMainControl;

	// Token: 0x040005A9 RID: 1449
	private float percent = -1f;

	// Token: 0x040005AA RID: 1450
	private float maxHealth;

	// Token: 0x040005AB RID: 1451
	private float currenthealth;

	// Token: 0x040005AC RID: 1452
	public ProceduralImage fillImage;

	// Token: 0x040005AD RID: 1453
	public ProceduralImage backgroundImage;

	// Token: 0x040005AE RID: 1454
	public Color backgroundColor;

	// Token: 0x040005AF RID: 1455
	public Color emptyBackgroundColor;

	// Token: 0x040005B0 RID: 1456
	public TextMeshProUGUI text;
}
