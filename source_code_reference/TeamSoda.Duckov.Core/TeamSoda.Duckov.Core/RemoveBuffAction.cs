using System;
using ItemStatsSystem;

// Token: 0x02000085 RID: 133
public class RemoveBuffAction : EffectAction
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00015CF2 File Offset: 0x00013EF2
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

	// Token: 0x060004CA RID: 1226 RVA: 0x00015D10 File Offset: 0x00013F10
	protected override void OnTriggered(bool positive)
	{
		if (!this.MainControl)
		{
			return;
		}
		this.MainControl.RemoveBuff(this.buffID, this.removeOneLayer);
	}

	// Token: 0x04000409 RID: 1033
	public int buffID;

	// Token: 0x0400040A RID: 1034
	public bool removeOneLayer;
}
