using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.Buffs;
using Duckov.Scenes;
using Duckov.Utilities;
using Duckov.Weathers;
using FX;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000064 RID: 100
public class Health : MonoBehaviour
{
	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060003A2 RID: 930 RVA: 0x0000FE86 File Offset: 0x0000E086
	// (set) Token: 0x060003A1 RID: 929 RVA: 0x0000FE7D File Offset: 0x0000E07D
	public bool showHealthBar
	{
		get
		{
			return this._showHealthBar;
		}
		set
		{
			this._showHealthBar = value;
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060003A3 RID: 931 RVA: 0x0000FE8E File Offset: 0x0000E08E
	public bool Hidden
	{
		get
		{
			return this.TryGetCharacter() && this.characterCached.Hidden;
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060003A4 RID: 932 RVA: 0x0000FEAC File Offset: 0x0000E0AC
	public float MaxHealth
	{
		get
		{
			float num;
			if (this.item)
			{
				num = this.item.GetStatValue(this.maxHealthHash);
			}
			else
			{
				num = (float)this.defaultMaxHealth;
			}
			if (!Mathf.Approximately(this.lastMaxHealth, num))
			{
				this.lastMaxHealth = num;
				UnityEvent<Health> onMaxHealthChange = this.OnMaxHealthChange;
				if (onMaxHealthChange != null)
				{
					onMaxHealthChange.Invoke(this);
				}
			}
			return num;
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060003A5 RID: 933 RVA: 0x0000FF10 File Offset: 0x0000E110
	public bool IsMainCharacterHealth
	{
		get
		{
			return !(LevelManager.Instance == null) && !(LevelManager.Instance.MainCharacter == null) && !(LevelManager.Instance.MainCharacter != this.TryGetCharacter());
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060003A6 RID: 934 RVA: 0x0000FF4F File Offset: 0x0000E14F
	// (set) Token: 0x060003A7 RID: 935 RVA: 0x0000FF58 File Offset: 0x0000E158
	public float CurrentHealth
	{
		get
		{
			return this._currentHealth;
		}
		set
		{
			float currentHealth = this._currentHealth;
			this._currentHealth = value;
			if (this._currentHealth != currentHealth)
			{
				UnityEvent<Health> onHealthChange = this.OnHealthChange;
				if (onHealthChange == null)
				{
					return;
				}
				onHealthChange.Invoke(this);
			}
		}
	}

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x060003A8 RID: 936 RVA: 0x0000FF90 File Offset: 0x0000E190
	// (remove) Token: 0x060003A9 RID: 937 RVA: 0x0000FFC4 File Offset: 0x0000E1C4
	public static event Action<Health, DamageInfo> OnHurt;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x060003AA RID: 938 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
	// (remove) Token: 0x060003AB RID: 939 RVA: 0x0001002C File Offset: 0x0000E22C
	public static event Action<Health, DamageInfo> OnDead;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x060003AC RID: 940 RVA: 0x00010060 File Offset: 0x0000E260
	// (remove) Token: 0x060003AD RID: 941 RVA: 0x00010094 File Offset: 0x0000E294
	public static event Action<Health> OnRequestHealthBar;

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060003AE RID: 942 RVA: 0x000100C7 File Offset: 0x0000E2C7
	public bool IsDead
	{
		get
		{
			return this.isDead;
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060003AF RID: 943 RVA: 0x000100CF File Offset: 0x0000E2CF
	public bool Invincible
	{
		get
		{
			return this.invincible;
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x000100D8 File Offset: 0x0000E2D8
	public CharacterMainControl TryGetCharacter()
	{
		if (this.characterCached != null)
		{
			return this.characterCached;
		}
		if (!this.hasCharacter)
		{
			return null;
		}
		if (!this.item)
		{
			this.hasCharacter = false;
			return null;
		}
		this.characterCached = this.item.GetCharacterMainControl();
		if (!this.characterCached)
		{
			this.hasCharacter = true;
		}
		return this.characterCached;
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060003B1 RID: 945 RVA: 0x00010145 File Offset: 0x0000E345
	public float BodyArmor
	{
		get
		{
			if (this.item)
			{
				return this.item.GetStatValue(this.bodyArmorHash);
			}
			return 0f;
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060003B2 RID: 946 RVA: 0x0001016B File Offset: 0x0000E36B
	public float HeadArmor
	{
		get
		{
			if (this.item)
			{
				return this.item.GetStatValue(this.headArmorHash);
			}
			return 0f;
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00010194 File Offset: 0x0000E394
	public float ElementFactor(ElementTypes type)
	{
		float num = 1f;
		if (!this.item)
		{
			return num;
		}
		Weather currentWeather = TimeOfDayController.Instance.CurrentWeather;
		bool isBaseLevel = LevelManager.Instance.IsBaseLevel;
		switch (type)
		{
		case ElementTypes.physics:
			num = this.item.GetStat(this.Hash_ElementFactor_Physics).Value;
			break;
		case ElementTypes.fire:
			num = this.item.GetStat(this.Hash_ElementFactor_Fire).Value;
			if (!isBaseLevel && currentWeather == Weather.Rainy)
			{
				num -= 0.15f;
			}
			break;
		case ElementTypes.poison:
			num = this.item.GetStat(this.Hash_ElementFactor_Poison).Value;
			break;
		case ElementTypes.electricity:
			num = this.item.GetStat(this.Hash_ElementFactor_Electricity).Value;
			if (!isBaseLevel && currentWeather == Weather.Rainy)
			{
				num += 0.2f;
			}
			break;
		case ElementTypes.space:
			num = this.item.GetStat(this.Hash_ElementFactor_Space).Value;
			break;
		}
		return num;
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x00010288 File Offset: 0x0000E488
	private void Start()
	{
		if (this.autoInit)
		{
			this.Init();
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00010298 File Offset: 0x0000E498
	public void SetItemAndCharacter(Item _item, CharacterMainControl _character)
	{
		this.item = _item;
		if (_character)
		{
			this.hasCharacter = true;
			this.characterCached = _character;
		}
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x000102B7 File Offset: 0x0000E4B7
	public void Init()
	{
		if (this.CurrentHealth <= 0f)
		{
			this.CurrentHealth = this.MaxHealth;
		}
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x000102D2 File Offset: 0x0000E4D2
	public void AddBuff(Buff buffPfb, CharacterMainControl fromWho, int overrideFromWeaponID = 0)
	{
		CharacterMainControl characterMainControl = this.TryGetCharacter();
		if (characterMainControl == null)
		{
			return;
		}
		characterMainControl.AddBuff(buffPfb, fromWho, overrideFromWeaponID);
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x000102E7 File Offset: 0x0000E4E7
	private void Update()
	{
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x000102EC File Offset: 0x0000E4EC
	public bool Hurt(DamageInfo damageInfo)
	{
		if (MultiSceneCore.Instance != null && MultiSceneCore.Instance.IsLoading)
		{
			return false;
		}
		if (this.invincible)
		{
			return false;
		}
		if (this.isDead)
		{
			return false;
		}
		if (damageInfo.buff != null && global::UnityEngine.Random.Range(0f, 1f) < damageInfo.buffChance)
		{
			this.AddBuff(damageInfo.buff, damageInfo.fromCharacter, damageInfo.fromWeaponItemID);
		}
		bool flag = LevelManager.Rule.AdvancedDebuffMode;
		if (LevelManager.Instance.IsBaseLevel)
		{
			flag = false;
		}
		float num = 0.2f;
		float num2 = 0.12f;
		CharacterMainControl characterMainControl = this.TryGetCharacter();
		if (!this.IsMainCharacterHealth)
		{
			num = 0.1f;
			num2 = 0.1f;
		}
		if (flag && global::UnityEngine.Random.Range(0f, 1f) < damageInfo.bleedChance * num)
		{
			this.AddBuff(GameplayDataSettings.Buffs.BoneCrackBuff, damageInfo.fromCharacter, damageInfo.fromWeaponItemID);
		}
		else if (flag && global::UnityEngine.Random.Range(0f, 1f) < damageInfo.bleedChance * num2)
		{
			this.AddBuff(GameplayDataSettings.Buffs.WoundBuff, damageInfo.fromCharacter, damageInfo.fromWeaponItemID);
		}
		else if (global::UnityEngine.Random.Range(0f, 1f) < damageInfo.bleedChance)
		{
			if (flag)
			{
				this.AddBuff(GameplayDataSettings.Buffs.UnlimitBleedBuff, damageInfo.fromCharacter, damageInfo.fromWeaponItemID);
			}
			else
			{
				this.AddBuff(GameplayDataSettings.Buffs.BleedSBuff, damageInfo.fromCharacter, damageInfo.fromWeaponItemID);
			}
		}
		bool flag2 = global::UnityEngine.Random.Range(0f, 1f) < damageInfo.critRate;
		damageInfo.crit = (flag2 ? 1 : 0);
		if (!damageInfo.ignoreDifficulty && this.team == Teams.player)
		{
			damageInfo.damageValue *= LevelManager.Rule.DamageFactor_ToPlayer;
		}
		float num3 = damageInfo.damageValue * (flag2 ? damageInfo.critDamageFactor : 1f);
		if (damageInfo.damageType != DamageTypes.realDamage && !damageInfo.ignoreArmor)
		{
			float num4 = (flag2 ? this.HeadArmor : this.BodyArmor);
			if (characterMainControl && LevelManager.Instance.IsRaidMap)
			{
				Item item = (flag2 ? characterMainControl.GetHelmatItem() : characterMainControl.GetArmorItem());
				if (item)
				{
					item.Durability = Mathf.Max(0f, item.Durability - damageInfo.armorBreak);
				}
			}
			float num5 = 1f;
			if (num4 > 0f)
			{
				num5 = 2f / (Mathf.Clamp(num4 - damageInfo.armorPiercing, 0f, 999f) + 2f);
			}
			if (characterMainControl && !characterMainControl.IsMainCharacter && damageInfo.fromCharacter && !damageInfo.fromCharacter.IsMainCharacter)
			{
				CharacterRandomPreset characterPreset = damageInfo.fromCharacter.characterPreset;
				CharacterRandomPreset characterPreset2 = characterMainControl.characterPreset;
				if (characterPreset && characterPreset2)
				{
					num5 *= characterPreset.aiCombatFactor / characterPreset2.aiCombatFactor;
				}
			}
			num3 *= num5;
		}
		if (damageInfo.elementFactors.Count <= 0)
		{
			damageInfo.elementFactors.Add(new ElementFactor(ElementTypes.physics, 1f));
		}
		float num6 = 0f;
		foreach (ElementFactor elementFactor in damageInfo.elementFactors)
		{
			float factor = elementFactor.factor;
			float num7 = this.ElementFactor(elementFactor.elementType);
			float num8 = num3 * factor * num7;
			if (num8 < 1f && num8 > 0f && num7 > 0f && factor > 0f)
			{
				num8 = 1f;
			}
			if (num8 > 0f && !this.Hidden && PopText.instance)
			{
				GameplayDataSettings.UIStyleData.DisplayElementDamagePopTextLook elementDamagePopTextLook = GameplayDataSettings.UIStyle.GetElementDamagePopTextLook(elementFactor.elementType);
				float num9 = (flag2 ? elementDamagePopTextLook.critSize : elementDamagePopTextLook.normalSize);
				Color color = elementDamagePopTextLook.color;
				PopText.Pop(num8.ToString("F1"), damageInfo.damagePoint + Vector3.up * 2f, color, num9, flag2 ? GameplayDataSettings.UIStyle.CritPopSprite : null);
			}
			num6 += num8;
		}
		damageInfo.finalDamage = num6;
		if (this.CurrentHealth < damageInfo.finalDamage)
		{
			damageInfo.finalDamage = this.CurrentHealth + 1f;
		}
		this.CurrentHealth -= damageInfo.finalDamage;
		UnityEvent<DamageInfo> onHurtEvent = this.OnHurtEvent;
		if (onHurtEvent != null)
		{
			onHurtEvent.Invoke(damageInfo);
		}
		Action<Health, DamageInfo> onHurt = Health.OnHurt;
		if (onHurt != null)
		{
			onHurt(this, damageInfo);
		}
		if (this.isDead)
		{
			return true;
		}
		if (this.CurrentHealth <= 0f)
		{
			bool flag3 = true;
			if (!LevelManager.Instance.IsRaidMap)
			{
				flag3 = false;
			}
			if (!flag3)
			{
				this.SetHealth(1f);
			}
		}
		if (this.CurrentHealth <= 0f)
		{
			this.CurrentHealth = 0f;
			this.isDead = true;
			if (LevelManager.Instance.MainCharacter != this.TryGetCharacter())
			{
				this.DestroyOnDelay().Forget();
			}
			if (this.item != null && this.team != Teams.player && damageInfo.fromCharacter && damageInfo.fromCharacter.IsMainCharacter)
			{
				EXPManager.AddExp(this.item.GetInt("Exp", 0));
			}
			UnityEvent<DamageInfo> onDeadEvent = this.OnDeadEvent;
			if (onDeadEvent != null)
			{
				onDeadEvent.Invoke(damageInfo);
			}
			Action<Health, DamageInfo> onDead = Health.OnDead;
			if (onDead != null)
			{
				onDead(this, damageInfo);
			}
			base.gameObject.SetActive(false);
			if (damageInfo.fromCharacter && damageInfo.fromCharacter.IsMainCharacter)
			{
				Debug.Log("Killed by maincharacter");
			}
		}
		return true;
	}

	// Token: 0x060003BA RID: 954 RVA: 0x000108D4 File Offset: 0x0000EAD4
	public void RequestHealthBar()
	{
		if (this.showHealthBar && LevelManager.LevelInited)
		{
			Action<Health> onRequestHealthBar = Health.OnRequestHealthBar;
			if (onRequestHealthBar == null)
			{
				return;
			}
			onRequestHealthBar(this);
		}
	}

	// Token: 0x060003BB RID: 955 RVA: 0x000108F8 File Offset: 0x0000EAF8
	public async UniTask DestroyOnDelay()
	{
		await UniTask.WaitForSeconds(this.DeadDestroyDelay, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (base.gameObject != null)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001093B File Offset: 0x0000EB3B
	public void AddHealth(float healthValue)
	{
		this.CurrentHealth = Mathf.Min(this.MaxHealth, this.CurrentHealth + healthValue);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00010956 File Offset: 0x0000EB56
	public void SetHealth(float healthValue)
	{
		this.CurrentHealth = Mathf.Min(this.MaxHealth, healthValue);
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0001096A File Offset: 0x0000EB6A
	public void SetInvincible(bool value)
	{
		this.invincible = value;
	}

	// Token: 0x040002C1 RID: 705
	public Teams team;

	// Token: 0x040002C2 RID: 706
	public bool hasSoul = true;

	// Token: 0x040002C3 RID: 707
	private Item item;

	// Token: 0x040002C4 RID: 708
	private int maxHealthHash = "MaxHealth".GetHashCode();

	// Token: 0x040002C5 RID: 709
	private float lastMaxHealth;

	// Token: 0x040002C6 RID: 710
	private bool _showHealthBar;

	// Token: 0x040002C7 RID: 711
	[SerializeField]
	private int defaultMaxHealth;

	// Token: 0x040002C8 RID: 712
	private float _currentHealth;

	// Token: 0x040002C9 RID: 713
	public UnityEvent<Health> OnHealthChange;

	// Token: 0x040002CA RID: 714
	public UnityEvent<Health> OnMaxHealthChange;

	// Token: 0x040002D0 RID: 720
	public float healthBarHeight = 2f;

	// Token: 0x040002D1 RID: 721
	private bool isDead;

	// Token: 0x040002D2 RID: 722
	public bool autoInit = true;

	// Token: 0x040002D3 RID: 723
	[SerializeField]
	private bool DestroyOnDead = true;

	// Token: 0x040002D4 RID: 724
	[SerializeField]
	private float DeadDestroyDelay = 0.5f;

	// Token: 0x040002D5 RID: 725
	private bool inited;

	// Token: 0x040002D6 RID: 726
	private bool invincible;

	// Token: 0x040002D7 RID: 727
	private bool hasCharacter = true;

	// Token: 0x040002D8 RID: 728
	private CharacterMainControl characterCached;

	// Token: 0x040002D9 RID: 729
	private int bodyArmorHash = "BodyArmor".GetHashCode();

	// Token: 0x040002DA RID: 730
	private int headArmorHash = "HeadArmor".GetHashCode();

	// Token: 0x040002DB RID: 731
	private int Hash_ElementFactor_Physics = "ElementFactor_Physics".GetHashCode();

	// Token: 0x040002DC RID: 732
	private int Hash_ElementFactor_Fire = "ElementFactor_Fire".GetHashCode();

	// Token: 0x040002DD RID: 733
	private int Hash_ElementFactor_Poison = "ElementFactor_Poison".GetHashCode();

	// Token: 0x040002DE RID: 734
	private int Hash_ElementFactor_Electricity = "ElementFactor_Electricity".GetHashCode();

	// Token: 0x040002DF RID: 735
	private int Hash_ElementFactor_Space = "ElementFactor_Space".GetHashCode();
}
