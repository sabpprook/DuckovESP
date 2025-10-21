using System;
using ItemStatsSystem;

// Token: 0x02000083 RID: 131
public class HealAction : EffectAction
{
	// Token: 0x170000FE RID: 254
	// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00015ACE File Offset: 0x00013CCE
	private CharacterMainControl MainControl
	{
		get
		{
			if (this._mainControl == null)
			{
				Effect master = base.Master;
				CharacterMainControl characterMainControl;
				if (master == null)
				{
					characterMainControl = null;
				}
				else
				{
					Item item = master.Item;
					characterMainControl = ((item != null) ? item.GetCharacterMainControl() : null);
				}
				this._mainControl = characterMainControl;
			}
			return this._mainControl;
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x00015B08 File Offset: 0x00013D08
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl)
		{
			return;
		}
		this.MainControl.Health.AddHealth((float)this.healValue);
	}

	// Token: 0x040003FE RID: 1022
	private CharacterMainControl _mainControl;

	// Token: 0x040003FF RID: 1023
	public int healValue = 10;
}
