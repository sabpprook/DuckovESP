using System;
using Duckov.Quests;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x02000118 RID: 280
public class Condition_XiaoHeiZi : Condition
{
	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x0600096C RID: 2412 RVA: 0x0002932D File Offset: 0x0002752D
	public override string DisplayText
	{
		get
		{
			return "看看你是不是小黑子";
		}
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x00029334 File Offset: 0x00027534
	public override bool Evaluate()
	{
		if (CharacterMainControl.Main == null)
		{
			return false;
		}
		CharacterMainControl main = CharacterMainControl.Main;
		CharacterModel characterModel = main.characterModel;
		if (!characterModel)
		{
			return false;
		}
		CustomFaceInstance customFace = characterModel.CustomFace;
		if (!customFace)
		{
			return false;
		}
		if (customFace.ConvertToSaveData().hairID != this.hairID)
		{
			return false;
		}
		Item armorItem = main.GetArmorItem();
		return !(armorItem == null) && armorItem.TypeID == this.armorID;
	}

	// Token: 0x04000852 RID: 2130
	[SerializeField]
	private int hairID = 6;

	// Token: 0x04000853 RID: 2131
	[ItemTypeID]
	[SerializeField]
	private int armorID = 379;
}
