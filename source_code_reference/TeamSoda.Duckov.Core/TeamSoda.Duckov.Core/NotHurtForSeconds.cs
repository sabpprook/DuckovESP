using System;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000089 RID: 137
[MenuPath("Health/一段时间没受伤")]
public class NotHurtForSeconds : EffectFilter
{
	// Token: 0x17000105 RID: 261
	// (get) Token: 0x060004DA RID: 1242 RVA: 0x00015FA3 File Offset: 0x000141A3
	public override string DisplayName
	{
		get
		{
			return this.time.ToString() + "秒内没受伤";
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x060004DB RID: 1243 RVA: 0x00015FBA File Offset: 0x000141BA
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

	// Token: 0x060004DC RID: 1244 RVA: 0x00015FF4 File Offset: 0x000141F4
	protected override bool OnEvaluate(EffectTriggerEventContext context)
	{
		if (!this.binded && this.MainControl)
		{
			this.MainControl.Health.OnHurtEvent.AddListener(new UnityAction<DamageInfo>(this.OnHurt));
			this.binded = true;
		}
		return Time.time - this.lastHurtTime > this.time;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x00016052 File Offset: 0x00014252
	private void OnDestroy()
	{
		if (this.MainControl)
		{
			this.MainControl.Health.OnHurtEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnHurt));
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00016082 File Offset: 0x00014282
	private void OnHurt(DamageInfo dmgInfo)
	{
		this.lastHurtTime = Time.time;
	}

	// Token: 0x04000414 RID: 1044
	public float time;

	// Token: 0x04000415 RID: 1045
	private float lastHurtTime = -9999f;

	// Token: 0x04000416 RID: 1046
	private bool binded;

	// Token: 0x04000417 RID: 1047
	private CharacterMainControl _mainControl;
}
