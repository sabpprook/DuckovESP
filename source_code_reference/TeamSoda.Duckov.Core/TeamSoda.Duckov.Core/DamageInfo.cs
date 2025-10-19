using System;
using System.Collections.Generic;
using Duckov.Buffs;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x0200006B RID: 107
[Serializable]
public struct DamageInfo
{
	// Token: 0x0600041C RID: 1052 RVA: 0x000122F0 File Offset: 0x000104F0
	public string GenerateDescription()
	{
		string text = "";
		string text2 = "";
		string text3 = "";
		if (this.fromCharacter != null)
		{
			if (this.fromCharacter.IsMainCharacter)
			{
				text = "DeathReason_Self".ToPlainText();
			}
			else if (this.fromCharacter.characterPreset != null)
			{
				text = this.fromCharacter.characterPreset.DisplayName;
			}
		}
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(this.fromWeaponItemID);
		if (metaData.id > 0)
		{
			text2 = metaData.DisplayName;
		}
		if (this.isExplosion)
		{
			text2 = "DeathReason_Explosion".ToPlainText();
		}
		if (this.crit > 0)
		{
			text3 = "DeathReason_Critical".ToPlainText();
		}
		bool flag = string.IsNullOrEmpty(text);
		bool flag2 = string.IsNullOrEmpty(text2);
		if (flag && flag2)
		{
			return "?";
		}
		if (flag)
		{
			return text2;
		}
		if (flag2)
		{
			return text;
		}
		return string.Concat(new string[] { text, " (", text2, ") ", text3 });
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x000123F0 File Offset: 0x000105F0
	public DamageInfo(CharacterMainControl fromCharacter = null)
	{
		this.damageValue = 0f;
		this.critDamageFactor = 1f;
		this.ignoreArmor = false;
		this.critRate = 0f;
		this.armorBreak = 0f;
		this.armorPiercing = 0f;
		this.fromCharacter = fromCharacter;
		this.toDamageReceiver = null;
		this.damagePoint = Vector3.zero;
		this.damageNormal = Vector3.up;
		this.elementFactors = new List<ElementFactor>();
		this.crit = -1;
		this.damageType = DamageTypes.normal;
		this.buffChance = 0f;
		this.buff = null;
		this.finalDamage = 0f;
		this.isFromBuffOrEffect = false;
		this.fromWeaponItemID = 0;
		this.isExplosion = false;
		this.bleedChance = 0f;
		this.ignoreDifficulty = false;
	}

	// Token: 0x04000314 RID: 788
	public DamageTypes damageType;

	// Token: 0x04000315 RID: 789
	public bool isFromBuffOrEffect;

	// Token: 0x04000316 RID: 790
	public float damageValue;

	// Token: 0x04000317 RID: 791
	public bool ignoreArmor;

	// Token: 0x04000318 RID: 792
	public bool ignoreDifficulty;

	// Token: 0x04000319 RID: 793
	public float critDamageFactor;

	// Token: 0x0400031A RID: 794
	public float critRate;

	// Token: 0x0400031B RID: 795
	public float armorPiercing;

	// Token: 0x0400031C RID: 796
	[SerializeField]
	public List<ElementFactor> elementFactors;

	// Token: 0x0400031D RID: 797
	public bool isExplosion;

	// Token: 0x0400031E RID: 798
	public float armorBreak;

	// Token: 0x0400031F RID: 799
	public float finalDamage;

	// Token: 0x04000320 RID: 800
	public CharacterMainControl fromCharacter;

	// Token: 0x04000321 RID: 801
	public DamageReceiver toDamageReceiver;

	// Token: 0x04000322 RID: 802
	[HideInInspector]
	public Vector3 damagePoint;

	// Token: 0x04000323 RID: 803
	[HideInInspector]
	public Vector3 damageNormal;

	// Token: 0x04000324 RID: 804
	public int crit;

	// Token: 0x04000325 RID: 805
	[ItemTypeID]
	public int fromWeaponItemID;

	// Token: 0x04000326 RID: 806
	public float buffChance;

	// Token: 0x04000327 RID: 807
	public Buff buff;

	// Token: 0x04000328 RID: 808
	public float bleedChance;
}
