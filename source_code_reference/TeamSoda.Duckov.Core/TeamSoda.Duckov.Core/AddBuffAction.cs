using System;
using Duckov.Buffs;
using ItemStatsSystem;

// Token: 0x02000080 RID: 128
public class AddBuffAction : EffectAction
{
	// Token: 0x170000FB RID: 251
	// (get) Token: 0x060004B8 RID: 1208 RVA: 0x0001593A File Offset: 0x00013B3A
	private CharacterMainControl MainControl
	{
		get
		{
			Effect master = base.Master;
			if (master == null)
			{
				return null;
			}
			Item item = master.Item;
			if (item == null)
			{
				return null;
			}
			return item.GetCharacterMainControl();
		}
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00015958 File Offset: 0x00013B58
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl)
		{
			return;
		}
		this.MainControl.AddBuff(this.buffPfb, this.MainControl, 0);
	}

	// Token: 0x040003F9 RID: 1017
	public Buff buffPfb;
}
