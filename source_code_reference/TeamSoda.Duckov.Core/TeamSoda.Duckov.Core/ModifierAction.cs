using System;
using Duckov.Buffs;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class ModifierAction : EffectAction
{
	// Token: 0x060004C4 RID: 1220 RVA: 0x00015B40 File Offset: 0x00013D40
	protected override void Awake()
	{
		base.Awake();
		this.modifier = new Modifier(this.ModifierType, this.modifierValue, this.overrideOrder, this.overrideOrderValue, base.Master);
		this.targetStatHash = this.targetStatKey.GetHashCode();
		if (this.buff)
		{
			this.buff.OnLayerChangedEvent += this.OnBuffLayerChanged;
		}
		this.OnBuffLayerChanged();
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x00015BB7 File Offset: 0x00013DB7
	private void OnBuffLayerChanged()
	{
		if (!this.buff)
		{
			return;
		}
		if (this.modifier == null)
		{
			return;
		}
		this.modifier.Value = this.modifierValue * (float)this.buff.CurrentLayers;
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00015BF0 File Offset: 0x00013DF0
	protected override void OnTriggered(bool positive)
	{
		if (base.Master.Item == null)
		{
			return;
		}
		Item characterItem = base.Master.Item.GetCharacterItem();
		if (characterItem == null)
		{
			return;
		}
		if (positive)
		{
			if (this.targetStat != null)
			{
				this.targetStat.RemoveModifier(this.modifier);
				this.targetStat = null;
			}
			this.targetStat = characterItem.GetStat(this.targetStatHash);
			this.targetStat.AddModifier(this.modifier);
			return;
		}
		if (this.targetStat != null)
		{
			this.targetStat.RemoveModifier(this.modifier);
			this.targetStat = null;
		}
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x00015C98 File Offset: 0x00013E98
	private void OnDestroy()
	{
		if (this.targetStat != null)
		{
			this.targetStat.RemoveModifier(this.modifier);
			this.targetStat = null;
		}
		if (this.buff)
		{
			this.buff.OnLayerChangedEvent -= this.OnBuffLayerChanged;
		}
	}

	// Token: 0x04000400 RID: 1024
	[SerializeField]
	private Buff buff;

	// Token: 0x04000401 RID: 1025
	public string targetStatKey;

	// Token: 0x04000402 RID: 1026
	private int targetStatHash;

	// Token: 0x04000403 RID: 1027
	public ModifierType ModifierType;

	// Token: 0x04000404 RID: 1028
	public float modifierValue;

	// Token: 0x04000405 RID: 1029
	public bool overrideOrder;

	// Token: 0x04000406 RID: 1030
	public int overrideOrderValue;

	// Token: 0x04000407 RID: 1031
	private Modifier modifier;

	// Token: 0x04000408 RID: 1032
	private Stat targetStat;
}
