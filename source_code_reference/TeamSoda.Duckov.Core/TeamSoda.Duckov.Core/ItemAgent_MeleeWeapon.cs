using System;
using Duckov.Utilities;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class ItemAgent_MeleeWeapon : DuckovItemAgent
{
	// Token: 0x17000194 RID: 404
	// (get) Token: 0x0600079A RID: 1946 RVA: 0x00022296 File Offset: 0x00020496
	public float Damage
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.DamageHash);
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x0600079B RID: 1947 RVA: 0x000222A8 File Offset: 0x000204A8
	public float CritRate
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.CritRateHash);
		}
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x0600079C RID: 1948 RVA: 0x000222BA File Offset: 0x000204BA
	public float CritDamageFactor
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.CritDamageFactorHash);
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x0600079D RID: 1949 RVA: 0x000222CC File Offset: 0x000204CC
	public float ArmorPiercing
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.ArmorPiercingHash);
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x0600079E RID: 1950 RVA: 0x000222DE File Offset: 0x000204DE
	public float AttackSpeed
	{
		get
		{
			return Mathf.Max(0.1f, base.Item.GetStatValue(ItemAgent_MeleeWeapon.AttackSpeedHash));
		}
	}

	// Token: 0x17000199 RID: 409
	// (get) Token: 0x0600079F RID: 1951 RVA: 0x000222FA File Offset: 0x000204FA
	public float AttackRange
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.AttackRangeHash);
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x060007A0 RID: 1952 RVA: 0x0002230C File Offset: 0x0002050C
	public float DealDamageTime
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.DealDamageTimeHash);
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x060007A1 RID: 1953 RVA: 0x0002231E File Offset: 0x0002051E
	public float StaminaCost
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.StaminaCostHash);
		}
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x060007A2 RID: 1954 RVA: 0x00022330 File Offset: 0x00020530
	public float BleedChance
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.BleedChanceHash);
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x060007A3 RID: 1955 RVA: 0x00022342 File Offset: 0x00020542
	public float MoveSpeedMultiplier
	{
		get
		{
			return base.Item.GetStatValue(ItemAgent_MeleeWeapon.MoveSpeedMultiplierHash);
		}
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x060007A4 RID: 1956 RVA: 0x00022354 File Offset: 0x00020554
	public float CharacterDamageMultiplier
	{
		get
		{
			if (!base.Holder)
			{
				return 1f;
			}
			return base.Holder.MeleeDamageMultiplier;
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x060007A5 RID: 1957 RVA: 0x00022374 File Offset: 0x00020574
	public float CharacterCritRateGain
	{
		get
		{
			if (!base.Holder)
			{
				return 0f;
			}
			return base.Holder.MeleeCritRateGain;
		}
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x060007A6 RID: 1958 RVA: 0x00022394 File Offset: 0x00020594
	public float CharacterCritDamageGain
	{
		get
		{
			if (!base.Holder)
			{
				return 0f;
			}
			return base.Holder.MeleeCritDamageGain;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x060007A7 RID: 1959 RVA: 0x000223B4 File Offset: 0x000205B4
	public string SoundKey
	{
		get
		{
			if (string.IsNullOrWhiteSpace(this.soundKey))
			{
				return "Default";
			}
			return this.soundKey;
		}
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x000223D0 File Offset: 0x000205D0
	private int UpdateColliders()
	{
		if (this.colliders == null)
		{
			this.colliders = new Collider[6];
		}
		return Physics.OverlapSphereNonAlloc(base.Holder.transform.position, this.AttackRange, this.colliders, GameplayDataSettings.Layers.damageReceiverLayerMask);
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00022421 File Offset: 0x00020621
	public void CheckAndDealDamage()
	{
		this.CheckCollidersInRange(true);
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0002242B File Offset: 0x0002062B
	public bool AttackableTargetInRange()
	{
		return this.CheckCollidersInRange(false) > 0;
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x00022438 File Offset: 0x00020638
	private int CheckCollidersInRange(bool dealDamage)
	{
		if (this.colliders == null)
		{
			this.colliders = new Collider[6];
		}
		int num = this.UpdateColliders();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			Collider collider = this.colliders[i];
			DamageReceiver component = collider.GetComponent<DamageReceiver>();
			if (!(component == null) && Team.IsEnemy(component.Team, base.Holder.Team))
			{
				Health health = component.health;
				if (health)
				{
					CharacterMainControl characterMainControl = health.TryGetCharacter();
					if (characterMainControl == base.Holder || (characterMainControl && characterMainControl.Dashing))
					{
						goto IL_02B3;
					}
				}
				Vector3 vector = collider.transform.position - base.Holder.transform.position;
				vector.y = 0f;
				vector.Normalize();
				if (Vector3.Angle(vector, base.Holder.CurrentAimDirection) < 90f)
				{
					num2++;
					if (dealDamage)
					{
						DamageInfo damageInfo = new DamageInfo(base.Holder);
						damageInfo.damageValue = this.Damage * this.CharacterDamageMultiplier;
						damageInfo.armorPiercing = this.ArmorPiercing;
						damageInfo.critDamageFactor = this.CritDamageFactor * (1f + this.CharacterCritDamageGain);
						damageInfo.critRate = this.CritRate * (1f + this.CharacterCritRateGain);
						damageInfo.crit = -1;
						damageInfo.damageNormal = -base.Holder.modelRoot.right;
						damageInfo.damagePoint = collider.transform.position - vector * 0.2f;
						damageInfo.damagePoint.y = base.transform.position.y;
						damageInfo.fromWeaponItemID = base.Item.TypeID;
						damageInfo.bleedChance = this.BleedChance;
						if (this.setting)
						{
							damageInfo.isExplosion = this.setting.dealExplosionDamage;
						}
						component.Hurt(damageInfo);
						component.AddBuff(GameplayDataSettings.Buffs.Pain, base.Holder);
						if (this.hitFx)
						{
							global::UnityEngine.Object.Instantiate<GameObject>(this.hitFx, damageInfo.damagePoint, Quaternion.LookRotation(damageInfo.damageNormal, Vector3.up));
						}
						if (base.Holder && base.Holder == CharacterMainControl.Main)
						{
							Vector3 vector2 = base.Holder.modelRoot.right;
							vector2 += global::UnityEngine.Random.insideUnitSphere * 0.3f;
							vector2.Normalize();
							CameraShaker.Shake(vector2 * 0.05f, CameraShaker.CameraShakeTypes.meleeAttackHit);
						}
					}
				}
			}
			IL_02B3:;
		}
		return num2;
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x00022704 File Offset: 0x00020904
	private void Update()
	{
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x00022706 File Offset: 0x00020906
	protected override void OnInitialize()
	{
		base.OnInitialize();
		this.setting = base.Item.GetComponent<ItemSetting_MeleeWeapon>();
	}

	// Token: 0x0400072D RID: 1837
	public GameObject hitFx;

	// Token: 0x0400072E RID: 1838
	public GameObject slashFx;

	// Token: 0x0400072F RID: 1839
	public float slashFxDelayTime = 0.05f;

	// Token: 0x04000730 RID: 1840
	[SerializeField]
	private string soundKey = "Default";

	// Token: 0x04000731 RID: 1841
	private Collider[] colliders;

	// Token: 0x04000732 RID: 1842
	private ItemSetting_MeleeWeapon setting;

	// Token: 0x04000733 RID: 1843
	private static int DamageHash = "Damage".GetHashCode();

	// Token: 0x04000734 RID: 1844
	private static int CritRateHash = "CritRate".GetHashCode();

	// Token: 0x04000735 RID: 1845
	private static int CritDamageFactorHash = "CritDamageFactor".GetHashCode();

	// Token: 0x04000736 RID: 1846
	private static int ArmorPiercingHash = "ArmorPiercing".GetHashCode();

	// Token: 0x04000737 RID: 1847
	private static int AttackSpeedHash = "AttackSpeed".GetHashCode();

	// Token: 0x04000738 RID: 1848
	private static int AttackRangeHash = "AttackRange".GetHashCode();

	// Token: 0x04000739 RID: 1849
	private static int DealDamageTimeHash = "DealDamageTime".GetHashCode();

	// Token: 0x0400073A RID: 1850
	private static int StaminaCostHash = "StaminaCost".GetHashCode();

	// Token: 0x0400073B RID: 1851
	private static int BleedChanceHash = "BleedChance".GetHashCode();

	// Token: 0x0400073C RID: 1852
	private static int MoveSpeedMultiplierHash = "MoveSpeedMultiplier".GetHashCode();
}
