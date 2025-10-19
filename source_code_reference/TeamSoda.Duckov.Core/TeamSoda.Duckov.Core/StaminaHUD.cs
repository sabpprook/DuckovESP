using System;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000BD RID: 189
public class StaminaHUD : MonoBehaviour
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000617 RID: 1559 RVA: 0x0001B51F File Offset: 0x0001971F
	private Item item
	{
		get
		{
			return this.characterMainControl.CharacterItem;
		}
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x0001B52C File Offset: 0x0001972C
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
		float num = this.characterMainControl.CurrentStamina / this.characterMainControl.MaxStamina;
		if (!Mathf.Approximately(num, this.percent))
		{
			this.percent = num;
			this.fillImage.fillAmount = this.percent;
			this.SetColor();
			if (Mathf.Approximately(num, 1f))
			{
				this.targetAlpha = 0f;
			}
			else
			{
				this.targetAlpha = 1f;
			}
		}
		this.UpdateAlpha(Time.unscaledDeltaTime);
	}

	// Token: 0x06000619 RID: 1561 RVA: 0x0001B5D8 File Offset: 0x000197D8
	private void SetColor()
	{
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(this.glowColor.Evaluate(this.percent), out num, out num2, out num3);
		num2 = 0.4f;
		num3 = 1f;
		Color color = Color.HSVToRGB(num, num2, num3);
		this.fillImage.color = color;
	}

	// Token: 0x0600061A RID: 1562 RVA: 0x0001B622 File Offset: 0x00019822
	private void UpdateAlpha(float deltaTime)
	{
		if (this.targetAlpha != this.canvasGroup.alpha)
		{
			this.canvasGroup.alpha = Mathf.MoveTowards(this.canvasGroup.alpha, this.targetAlpha, 5f * deltaTime);
		}
	}

	// Token: 0x040005B4 RID: 1460
	private CharacterMainControl characterMainControl;

	// Token: 0x040005B5 RID: 1461
	private float percent;

	// Token: 0x040005B6 RID: 1462
	public CanvasGroup canvasGroup;

	// Token: 0x040005B7 RID: 1463
	private float targetAlpha;

	// Token: 0x040005B8 RID: 1464
	public ProceduralImage fillImage;

	// Token: 0x040005B9 RID: 1465
	public Gradient glowColor;
}
