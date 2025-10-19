using System;
using ItemStatsSystem;

// Token: 0x02000087 RID: 135
[MenuPath("角色/角色正在奔跑")]
public class CharacterIsRunning : EffectFilter
{
	// Token: 0x17000101 RID: 257
	// (get) Token: 0x060004D0 RID: 1232 RVA: 0x00015E64 File Offset: 0x00014064
	public override string DisplayName
	{
		get
		{
			return "角色正在奔跑";
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x060004D1 RID: 1233 RVA: 0x00015E6B File Offset: 0x0001406B
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

	// Token: 0x060004D2 RID: 1234 RVA: 0x00015EA5 File Offset: 0x000140A5
	protected override bool OnEvaluate(EffectTriggerEventContext context)
	{
		return this.MainControl.Running;
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00015EB2 File Offset: 0x000140B2
	private void OnDestroy()
	{
	}

	// Token: 0x0400040F RID: 1039
	private CharacterMainControl _mainControl;
}
