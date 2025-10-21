using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000065 RID: 101
public class HealthSimpleBase : MonoBehaviour
{
	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060003C0 RID: 960 RVA: 0x00010A39 File Offset: 0x0000EC39
	public float HealthValue
	{
		get
		{
			return this.healthValue;
		}
	}

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x060003C1 RID: 961 RVA: 0x00010A44 File Offset: 0x0000EC44
	// (remove) Token: 0x060003C2 RID: 962 RVA: 0x00010A7C File Offset: 0x0000EC7C
	public event Action<DamageInfo> OnHurtEvent;

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x060003C3 RID: 963 RVA: 0x00010AB4 File Offset: 0x0000ECB4
	// (remove) Token: 0x060003C4 RID: 964 RVA: 0x00010AE8 File Offset: 0x0000ECE8
	public static event Action<HealthSimpleBase, DamageInfo> OnSimpleHealthHit;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x060003C5 RID: 965 RVA: 0x00010B1C File Offset: 0x0000ED1C
	// (remove) Token: 0x060003C6 RID: 966 RVA: 0x00010B54 File Offset: 0x0000ED54
	public event Action<DamageInfo> OnDeadEvent;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x060003C7 RID: 967 RVA: 0x00010B8C File Offset: 0x0000ED8C
	// (remove) Token: 0x060003C8 RID: 968 RVA: 0x00010BC0 File Offset: 0x0000EDC0
	public static event Action<HealthSimpleBase, DamageInfo> OnSimpleHealthDead;

	// Token: 0x060003C9 RID: 969 RVA: 0x00010BF3 File Offset: 0x0000EDF3
	private void Awake()
	{
		this.healthValue = this.maxHealthValue;
		this.dmgReceiver.OnHurtEvent.AddListener(new UnityAction<DamageInfo>(this.OnHurt));
	}

	// Token: 0x060003CA RID: 970 RVA: 0x00010C20 File Offset: 0x0000EE20
	private void OnHurt(DamageInfo dmgInfo)
	{
		if (this.onlyReceiveExplosion && !dmgInfo.isExplosion)
		{
			return;
		}
		float num = 1f;
		bool flag = global::UnityEngine.Random.Range(0f, 1f) <= dmgInfo.critRate;
		dmgInfo.crit = (flag ? 1 : 0);
		if (!dmgInfo.fromCharacter || !dmgInfo.fromCharacter.IsMainCharacter)
		{
			num = this.damageMultiplierIfNotMainCharacter;
		}
		this.healthValue -= (flag ? dmgInfo.critDamageFactor : 1f) * dmgInfo.damageValue * num;
		Action<DamageInfo> onHurtEvent = this.OnHurtEvent;
		if (onHurtEvent != null)
		{
			onHurtEvent(dmgInfo);
		}
		Action<HealthSimpleBase, DamageInfo> onSimpleHealthHit = HealthSimpleBase.OnSimpleHealthHit;
		if (onSimpleHealthHit != null)
		{
			onSimpleHealthHit(this, dmgInfo);
		}
		if (this.healthValue <= 0f)
		{
			this.Dead(dmgInfo);
		}
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00010CEC File Offset: 0x0000EEEC
	private void Dead(DamageInfo dmgInfo)
	{
		this.dmgReceiver.OnDead(dmgInfo);
		Action<DamageInfo> onDeadEvent = this.OnDeadEvent;
		if (onDeadEvent != null)
		{
			onDeadEvent(dmgInfo);
		}
		Action<HealthSimpleBase, DamageInfo> onSimpleHealthDead = HealthSimpleBase.OnSimpleHealthDead;
		if (onSimpleHealthDead == null)
		{
			return;
		}
		onSimpleHealthDead(this, dmgInfo);
	}

	// Token: 0x040002E0 RID: 736
	public Teams team;

	// Token: 0x040002E1 RID: 737
	public bool onlyReceiveExplosion;

	// Token: 0x040002E2 RID: 738
	public float maxHealthValue = 250f;

	// Token: 0x040002E3 RID: 739
	private float healthValue;

	// Token: 0x040002E4 RID: 740
	public DamageReceiver dmgReceiver;

	// Token: 0x040002E8 RID: 744
	public float damageMultiplierIfNotMainCharacter = 1f;
}
