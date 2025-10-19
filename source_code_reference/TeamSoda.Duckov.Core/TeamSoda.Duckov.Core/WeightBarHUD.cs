using System;
using ItemStatsSystem;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x020000D1 RID: 209
public class WeightBarHUD : MonoBehaviour
{
	// Token: 0x1700012F RID: 303
	// (get) Token: 0x0600066D RID: 1645 RVA: 0x0001D1B7 File Offset: 0x0001B3B7
	private Item item
	{
		get
		{
			return this.characterMainControl.CharacterItem;
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0001D1C4 File Offset: 0x0001B3C4
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
		float totalWeight = this.characterMainControl.CharacterItem.TotalWeight;
		float num = this.characterMainControl.MaxWeight;
		if (!Mathf.Approximately(totalWeight, this.weight) || !Mathf.Approximately(num, this.maxWeight))
		{
			this.weight = totalWeight;
			this.maxWeight = num;
			this.percent = this.weight / this.maxWeight;
			this.weightText.text = string.Format(this.weightTextFormat, this.weight, this.maxWeight);
			this.fillImage.fillAmount = this.percent;
			this.SetColor();
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0001D29C File Offset: 0x0001B49C
	private void SetColor()
	{
		Color color;
		if (this.percent < 0.25f)
		{
			color = this.lightColor;
		}
		else if (this.percent < 0.75f)
		{
			color = this.normalColor;
		}
		else if (this.percent < 1f)
		{
			color = this.heavyColor;
		}
		else
		{
			color = this.overWeightColor;
		}
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(color, out num, out num2, out num3);
		Color color2 = color;
		if (num2 > 0.4f)
		{
			num2 = 0.4f;
			num3 = 1f;
			color2 = Color.HSVToRGB(num, num2, num3);
		}
		this.glow.Color = color;
		this.fillImage.color = color2;
		this.weightText.color = color;
	}

	// Token: 0x0400063A RID: 1594
	private CharacterMainControl characterMainControl;

	// Token: 0x0400063B RID: 1595
	private float percent;

	// Token: 0x0400063C RID: 1596
	private float weight;

	// Token: 0x0400063D RID: 1597
	private float maxWeight;

	// Token: 0x0400063E RID: 1598
	public ProceduralImage fillImage;

	// Token: 0x0400063F RID: 1599
	public TrueShadow glow;

	// Token: 0x04000640 RID: 1600
	public Color lightColor;

	// Token: 0x04000641 RID: 1601
	public Color normalColor;

	// Token: 0x04000642 RID: 1602
	public Color heavyColor;

	// Token: 0x04000643 RID: 1603
	public Color overWeightColor;

	// Token: 0x04000644 RID: 1604
	public TextMeshProUGUI weightText;

	// Token: 0x04000645 RID: 1605
	public string weightTextFormat = "{0:0.#}/{1:0.#}kg";
}
