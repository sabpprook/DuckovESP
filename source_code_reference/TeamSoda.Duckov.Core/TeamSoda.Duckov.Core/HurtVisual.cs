using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000066 RID: 102
public class HurtVisual : MonoBehaviour
{
	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060003CD RID: 973 RVA: 0x00010D3B File Offset: 0x0000EF3B
	public GameObject HitFx
	{
		get
		{
			if (!GameManager.BloodFxOn && this.hitFX_NoBlood != null)
			{
				return this.hitFX_NoBlood;
			}
			return this.hitFX;
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060003CE RID: 974 RVA: 0x00010D5F File Offset: 0x0000EF5F
	public GameObject DeadFx
	{
		get
		{
			if (!GameManager.BloodFxOn && this.deadFx_NoBlood != null)
			{
				return this.deadFx_NoBlood;
			}
			return this.deadFx;
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x00010D84 File Offset: 0x0000EF84
	public void SetHealth(Health _health)
	{
		if (this.useSimpleHealth)
		{
			return;
		}
		if (this.health != null)
		{
			this.health.OnHurtEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnHurt));
			this.health.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnDead));
		}
		this.health = _health;
		_health.OnHurtEvent.AddListener(new UnityAction<DamageInfo>(this.OnHurt));
		_health.OnDeadEvent.AddListener(new UnityAction<DamageInfo>(this.OnDead));
		this.Init();
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x00010E1C File Offset: 0x0000F01C
	private void Awake()
	{
		if (this.useSimpleHealth && this.simpleHealth != null)
		{
			this.simpleHealth.OnHurtEvent += this.OnHurt;
			this.simpleHealth.OnDeadEvent += this.OnDead;
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00010E6D File Offset: 0x0000F06D
	private void Init()
	{
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00010E70 File Offset: 0x0000F070
	private void Update()
	{
		if (this.hurtValue > 0f)
		{
			this.SetRendererValue(this.hurtValue);
			this.hurtValue -= Time.unscaledDeltaTime * this.hurtCoolSpeed;
			if (this.hurtValue <= 0f)
			{
				this.SetRendererValue(0f);
			}
		}
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00010EC8 File Offset: 0x0000F0C8
	private void OnHurt(DamageInfo dmgInfo)
	{
		bool flag = this.health && this.health.Hidden;
		if (this.HitFx && !flag)
		{
			PlayHurtEventProxy component = global::UnityEngine.Object.Instantiate<GameObject>(this.HitFx, dmgInfo.damagePoint, Quaternion.LookRotation(dmgInfo.damageNormal)).GetComponent<PlayHurtEventProxy>();
			if (component)
			{
				component.Play(dmgInfo.crit > 0);
			}
		}
		this.hurtValue = 1f;
		this.SetRendererValue(this.hurtValue);
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00010F54 File Offset: 0x0000F154
	private void SetRendererValue(float value)
	{
		int count = this.renderers.Count;
		for (int i = 0; i < count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				if (this.materialPropertyBlock == null)
				{
					this.materialPropertyBlock = new MaterialPropertyBlock();
				}
				this.renderers[i].GetPropertyBlock(this.materialPropertyBlock);
				this.materialPropertyBlock.SetFloat(HurtVisual.hurtHash, value * this.hurtValueMultiplier);
				this.renderers[i].SetPropertyBlock(this.materialPropertyBlock);
			}
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00010FE8 File Offset: 0x0000F1E8
	private void OnDead(DamageInfo dmgInfo)
	{
		if (this.DeadFx)
		{
			PlayHurtEventProxy component = global::UnityEngine.Object.Instantiate<GameObject>(this.DeadFx, base.transform.position, base.transform.rotation).GetComponent<PlayHurtEventProxy>();
			if (component)
			{
				component.Play(dmgInfo.crit > 0);
			}
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00011040 File Offset: 0x0000F240
	private void OnDestroy()
	{
		if (this.health)
		{
			this.health.OnHurtEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnHurt));
			this.health.OnDeadEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnDead));
		}
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00011092 File Offset: 0x0000F292
	private void AutoSet()
	{
		this.renderers = base.GetComponentsInChildren<Renderer>(true).ToList<Renderer>();
		this.renderers.RemoveAll((Renderer e) => e == null || e.GetComponent<ParticleSystem>() != null);
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x000110D1 File Offset: 0x0000F2D1
	public void SetRenderers(List<Renderer> _renderers)
	{
		this.renderers = _renderers;
	}

	// Token: 0x040002EA RID: 746
	public bool useSimpleHealth;

	// Token: 0x040002EB RID: 747
	public HealthSimpleBase simpleHealth;

	// Token: 0x040002EC RID: 748
	private Health health;

	// Token: 0x040002ED RID: 749
	[SerializeField]
	private GameObject hitFX;

	// Token: 0x040002EE RID: 750
	[SerializeField]
	private GameObject hitFX_NoBlood;

	// Token: 0x040002EF RID: 751
	[SerializeField]
	private GameObject deadFx;

	// Token: 0x040002F0 RID: 752
	[SerializeField]
	private GameObject deadFx_NoBlood;

	// Token: 0x040002F1 RID: 753
	public List<Renderer> renderers;

	// Token: 0x040002F2 RID: 754
	public static readonly int hurtHash = Shader.PropertyToID("_HurtValue");

	// Token: 0x040002F3 RID: 755
	private MaterialPropertyBlock materialPropertyBlock;

	// Token: 0x040002F4 RID: 756
	public float hurtCoolSpeed = 8f;

	// Token: 0x040002F5 RID: 757
	public float hurtValueMultiplier = 1f;

	// Token: 0x040002F6 RID: 758
	private float hurtValue;
}
