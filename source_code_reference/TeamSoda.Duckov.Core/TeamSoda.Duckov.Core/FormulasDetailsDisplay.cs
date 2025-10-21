using System;
using Duckov.Economy;
using Duckov.UI.Animations;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001B2 RID: 434
public class FormulasDetailsDisplay : MonoBehaviour
{
	// Token: 0x06000CDA RID: 3290 RVA: 0x0003588F File Offset: 0x00033A8F
	private void SetupEmpty()
	{
		this.contentFadeGroup.Hide();
		this.placeHolderFadeGroup.Show();
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000358A7 File Offset: 0x00033AA7
	private void SetupFormula(CraftingFormula formula)
	{
		this.formula = formula;
		this.RefreshContent();
		this.contentFadeGroup.Show();
		this.placeHolderFadeGroup.Hide();
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x000358CC File Offset: 0x00033ACC
	private void RefreshContent()
	{
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(this.formula.result.id);
		this.nameText.text = metaData.DisplayName;
		this.descriptionText.text = metaData.Description;
		this.image.sprite = metaData.icon;
		this.costDisplay.Setup(this.formula.cost, 1);
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0003593B File Offset: 0x00033B3B
	public void Setup(CraftingFormula? formula)
	{
		if (formula == null)
		{
			this.SetupEmpty();
			return;
		}
		if (!CraftingManager.IsFormulaUnlocked(formula.Value.id))
		{
			this.SetupUnknown();
			return;
		}
		this.SetupFormula(formula.Value);
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x00035974 File Offset: 0x00033B74
	private void SetupUnknown()
	{
		this.nameText.text = "???";
		this.descriptionText.text = "???";
		this.image.sprite = this.unknownImage;
		this.contentFadeGroup.Show();
		this.placeHolderFadeGroup.Hide();
		this.costDisplay.Setup(default(Cost), 1);
	}

	// Token: 0x04000B14 RID: 2836
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x04000B15 RID: 2837
	[SerializeField]
	private Image image;

	// Token: 0x04000B16 RID: 2838
	[SerializeField]
	private TextMeshProUGUI descriptionText;

	// Token: 0x04000B17 RID: 2839
	[SerializeField]
	private CostDisplay costDisplay;

	// Token: 0x04000B18 RID: 2840
	[SerializeField]
	private FadeGroup contentFadeGroup;

	// Token: 0x04000B19 RID: 2841
	[SerializeField]
	private FadeGroup placeHolderFadeGroup;

	// Token: 0x04000B1A RID: 2842
	[SerializeField]
	private Sprite unknownImage;

	// Token: 0x04000B1B RID: 2843
	private CraftingFormula formula;
}
