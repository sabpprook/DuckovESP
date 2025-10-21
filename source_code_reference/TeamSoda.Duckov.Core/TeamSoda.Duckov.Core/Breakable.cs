using System;
using Saves;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class Breakable : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060001E7 RID: 487 RVA: 0x00009564 File Offset: 0x00007764
	public string SaveKey
	{
		get
		{
			return "Breakable_" + this.saveKey;
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00009578 File Offset: 0x00007778
	private void Awake()
	{
		this.normalVisual.SetActive(true);
		if (this.dangerVisual)
		{
			this.dangerVisual.SetActive(false);
		}
		if (this.breakedVisual)
		{
			this.breakedVisual.SetActive(false);
		}
		this.simpleHealth.OnHurtEvent += this.OnHurt;
		this.simpleHealth.OnDeadEvent += this.OnDead;
		bool flag = false;
		if (this.save)
		{
			flag = SavesSystem.Load<bool>(this.SaveKey);
		}
		if (flag)
		{
			this.breakableState = Breakable.BreakableStates.danger;
			this.normalVisual.SetActive(false);
			if (this.dangerVisual)
			{
				this.dangerVisual.SetActive(false);
			}
			if (this.breakedVisual)
			{
				this.breakedVisual.SetActive(true);
			}
			if (this.simpleHealth && this.simpleHealth.dmgReceiver)
			{
				this.simpleHealth.dmgReceiver.gameObject.SetActive(false);
				return;
			}
		}
		else if (this.mainCollider)
		{
			this.mainCollider.SetActive(true);
		}
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000969E File Offset: 0x0000789E
	private void OnValidate()
	{
	}

	// Token: 0x060001EA RID: 490 RVA: 0x000096A0 File Offset: 0x000078A0
	private void OnHurt(DamageInfo dmgInfo)
	{
		switch (this.breakableState)
		{
		case Breakable.BreakableStates.normal:
			if (this.simpleHealth.HealthValue <= (float)this.dangerHealth)
			{
				this.breakableState = Breakable.BreakableStates.danger;
				if (this.dangerVisual)
				{
					this.normalVisual.SetActive(false);
					this.dangerVisual.SetActive(true);
				}
				if (this.dangerFx)
				{
					global::UnityEngine.Object.Instantiate<GameObject>(this.dangerFx, base.transform.position, base.transform.rotation);
				}
			}
			break;
		case Breakable.BreakableStates.danger:
		case Breakable.BreakableStates.breaked:
			break;
		default:
			return;
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00009738 File Offset: 0x00007938
	private void OnDead(DamageInfo dmgInfo)
	{
		this.explosionDamageInfo.fromCharacter = dmgInfo.fromCharacter;
		this.normalVisual.SetActive(false);
		if (this.dangerVisual)
		{
			this.dangerVisual.SetActive(false);
		}
		if (this.breakedVisual)
		{
			this.breakedVisual.SetActive(true);
		}
		if (this.mainCollider)
		{
			this.mainCollider.SetActive(false);
		}
		this.breakableState = Breakable.BreakableStates.breaked;
		if (this.createExplosion)
		{
			LevelManager.Instance.ExplosionManager.CreateExplosion(base.transform.position, this.explosionRadius, this.explosionDamageInfo, ExplosionFxTypes.normal, 1f, true);
		}
		if (this.save)
		{
			SavesSystem.Save<bool>("Breakable_", this.saveKey, true);
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00009802 File Offset: 0x00007A02
	private void OnDrawGizmosSelected()
	{
		if (this.createExplosion)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, this.explosionRadius);
		}
	}

	// Token: 0x040001A4 RID: 420
	public bool save;

	// Token: 0x040001A5 RID: 421
	public string saveKey;

	// Token: 0x040001A6 RID: 422
	public HealthSimpleBase simpleHealth;

	// Token: 0x040001A7 RID: 423
	public int dangerHealth = 50;

	// Token: 0x040001A8 RID: 424
	public bool createExplosion;

	// Token: 0x040001A9 RID: 425
	public float explosionRadius;

	// Token: 0x040001AA RID: 426
	public DamageInfo explosionDamageInfo;

	// Token: 0x040001AB RID: 427
	private Breakable.BreakableStates breakableState;

	// Token: 0x040001AC RID: 428
	public GameObject normalVisual;

	// Token: 0x040001AD RID: 429
	public GameObject dangerVisual;

	// Token: 0x040001AE RID: 430
	public GameObject breakedVisual;

	// Token: 0x040001AF RID: 431
	public GameObject mainCollider;

	// Token: 0x040001B0 RID: 432
	public GameObject dangerFx;

	// Token: 0x0200042A RID: 1066
	private enum BreakableStates
	{
		// Token: 0x040019F9 RID: 6649
		normal,
		// Token: 0x040019FA RID: 6650
		danger,
		// Token: 0x040019FB RID: 6651
		breaked
	}
}
