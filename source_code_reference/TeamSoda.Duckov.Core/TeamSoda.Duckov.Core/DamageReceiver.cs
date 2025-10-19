using System;
using Duckov.Buffs;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200006E RID: 110
public class DamageReceiver : MonoBehaviour
{
	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x0600041F RID: 1055 RVA: 0x000124CC File Offset: 0x000106CC
	public Teams Team
	{
		get
		{
			if (!this.useSimpleHealth && this.health)
			{
				return this.health.team;
			}
			if (this.useSimpleHealth && this.simpleHealth)
			{
				return this.simpleHealth.team;
			}
			return Teams.all;
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000420 RID: 1056 RVA: 0x0001251C File Offset: 0x0001071C
	public bool IsMainCharacter
	{
		get
		{
			return !this.useSimpleHealth && this.health && this.health.IsMainCharacterHealth;
		}
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000421 RID: 1057 RVA: 0x00012540 File Offset: 0x00010740
	public bool IsDead
	{
		get
		{
			return this.health && this.health.IsDead;
		}
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x0001255C File Offset: 0x0001075C
	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("DamageReceiver");
		if (this.health)
		{
			this.health.OnDeadEvent.AddListener(new UnityAction<DamageInfo>(this.OnDead));
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0001259C File Offset: 0x0001079C
	private void OnDestroy()
	{
		if (this.health)
		{
			this.health.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnDead));
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x000125C7 File Offset: 0x000107C7
	public bool Hurt(DamageInfo damageInfo)
	{
		damageInfo.toDamageReceiver = this;
		UnityEvent<DamageInfo> onHurtEvent = this.OnHurtEvent;
		if (onHurtEvent != null)
		{
			onHurtEvent.Invoke(damageInfo);
		}
		if (this.health)
		{
			this.health.Hurt(damageInfo);
		}
		return true;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00012600 File Offset: 0x00010800
	public bool AddBuff(Buff buffPfb, CharacterMainControl fromWho)
	{
		if (this.useSimpleHealth)
		{
			return false;
		}
		if (!this.health)
		{
			return false;
		}
		CharacterMainControl characterMainControl = this.health.TryGetCharacter();
		if (!characterMainControl)
		{
			return false;
		}
		characterMainControl.AddBuff(buffPfb, fromWho, 0);
		return true;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00012646 File Offset: 0x00010846
	public void OnDead(DamageInfo dmgInfo)
	{
		base.gameObject.SetActive(false);
		UnityEvent<DamageInfo> onDeadEvent = this.OnDeadEvent;
		if (onDeadEvent == null)
		{
			return;
		}
		onDeadEvent.Invoke(dmgInfo);
	}

	// Token: 0x04000331 RID: 817
	public bool useSimpleHealth;

	// Token: 0x04000332 RID: 818
	public Health health;

	// Token: 0x04000333 RID: 819
	public HealthSimpleBase simpleHealth;

	// Token: 0x04000334 RID: 820
	public bool isHalfObsticle;

	// Token: 0x04000335 RID: 821
	public UnityEvent<DamageInfo> OnHurtEvent;

	// Token: 0x04000336 RID: 822
	public UnityEvent<DamageInfo> OnDeadEvent;
}
