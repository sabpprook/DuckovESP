using System;
using ItemStatsSystem;

// Token: 0x02000081 RID: 129
public class CostStaminaAction : EffectAction
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x060004BB RID: 1211 RVA: 0x00015988 File Offset: 0x00013B88
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

	// Token: 0x060004BC RID: 1212 RVA: 0x000159A6 File Offset: 0x00013BA6
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl)
		{
			return;
		}
		this.MainControl.UseStamina(this.staminaCost);
	}

	// Token: 0x040003FA RID: 1018
	public float staminaCost;
}
