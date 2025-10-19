using System;
using System.Collections.Generic;
using ItemStatsSystem;
using SodaCraft.Localizations;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.Buffs
{
	// Token: 0x020003F9 RID: 1017
	public class Buff : MonoBehaviour
	{
		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x060024D9 RID: 9433 RVA: 0x0007F5A8 File Offset: 0x0007D7A8
		public Buff.BuffExclusiveTags ExclusiveTag
		{
			get
			{
				return this.exclusiveTag;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x060024DA RID: 9434 RVA: 0x0007F5B0 File Offset: 0x0007D7B0
		public int ExclusiveTagPriority
		{
			get
			{
				return this.exclusiveTagPriority;
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x060024DB RID: 9435 RVA: 0x0007F5B8 File Offset: 0x0007D7B8
		public bool Hide
		{
			get
			{
				return this.hide;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x060024DC RID: 9436 RVA: 0x0007F5C0 File Offset: 0x0007D7C0
		public CharacterMainControl Character
		{
			get
			{
				CharacterBuffManager characterBuffManager = this.master;
				if (characterBuffManager == null)
				{
					return null;
				}
				return characterBuffManager.Master;
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x060024DD RID: 9437 RVA: 0x0007F5D3 File Offset: 0x0007D7D3
		private Item CharacterItem
		{
			get
			{
				CharacterBuffManager characterBuffManager = this.master;
				if (characterBuffManager == null)
				{
					return null;
				}
				CharacterMainControl characterMainControl = characterBuffManager.Master;
				if (characterMainControl == null)
				{
					return null;
				}
				return characterMainControl.CharacterItem;
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x060024DE RID: 9438 RVA: 0x0007F5F1 File Offset: 0x0007D7F1
		// (set) Token: 0x060024DF RID: 9439 RVA: 0x0007F5F9 File Offset: 0x0007D7F9
		public int ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x060024E0 RID: 9440 RVA: 0x0007F602 File Offset: 0x0007D802
		// (set) Token: 0x060024E1 RID: 9441 RVA: 0x0007F60A File Offset: 0x0007D80A
		public int CurrentLayers
		{
			get
			{
				return this.currentLayers;
			}
			set
			{
				this.currentLayers = value;
				Action onLayerChangedEvent = this.OnLayerChangedEvent;
				if (onLayerChangedEvent == null)
				{
					return;
				}
				onLayerChangedEvent();
			}
		}

		// Token: 0x140000F3 RID: 243
		// (add) Token: 0x060024E2 RID: 9442 RVA: 0x0007F624 File Offset: 0x0007D824
		// (remove) Token: 0x060024E3 RID: 9443 RVA: 0x0007F65C File Offset: 0x0007D85C
		public event Action OnLayerChangedEvent;

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x060024E4 RID: 9444 RVA: 0x0007F691 File Offset: 0x0007D891
		public int MaxLayers
		{
			get
			{
				return this.maxLayers;
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x060024E5 RID: 9445 RVA: 0x0007F699 File Offset: 0x0007D899
		public string DisplayName
		{
			get
			{
				return this.displayName.ToPlainText();
			}
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x060024E6 RID: 9446 RVA: 0x0007F6A6 File Offset: 0x0007D8A6
		public string DisplayNameKey
		{
			get
			{
				return this.displayName;
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x060024E7 RID: 9447 RVA: 0x0007F6AE File Offset: 0x0007D8AE
		public string Description
		{
			get
			{
				return this.description.ToPlainText();
			}
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x060024E8 RID: 9448 RVA: 0x0007F6BB File Offset: 0x0007D8BB
		public Sprite Icon
		{
			get
			{
				return this.icon;
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x060024E9 RID: 9449 RVA: 0x0007F6C3 File Offset: 0x0007D8C3
		public bool LimitedLifeTime
		{
			get
			{
				return this.limitedLifeTime;
			}
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x060024EA RID: 9450 RVA: 0x0007F6CB File Offset: 0x0007D8CB
		public float TotalLifeTime
		{
			get
			{
				return this.totalLifeTime;
			}
		}

		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x060024EB RID: 9451 RVA: 0x0007F6D3 File Offset: 0x0007D8D3
		public float CurrentLifeTime
		{
			get
			{
				return Time.time - this.timeWhenStarted;
			}
		}

		// Token: 0x17000723 RID: 1827
		// (get) Token: 0x060024EC RID: 9452 RVA: 0x0007F6E1 File Offset: 0x0007D8E1
		public float RemainingTime
		{
			get
			{
				if (!this.limitedLifeTime)
				{
					return float.PositiveInfinity;
				}
				return this.totalLifeTime - this.CurrentLifeTime;
			}
		}

		// Token: 0x17000724 RID: 1828
		// (get) Token: 0x060024ED RID: 9453 RVA: 0x0007F6FE File Offset: 0x0007D8FE
		public bool IsOutOfTime
		{
			get
			{
				return this.limitedLifeTime && this.CurrentLifeTime >= this.totalLifeTime;
			}
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x0007F71C File Offset: 0x0007D91C
		internal void Setup(CharacterBuffManager manager)
		{
			this.master = manager;
			this.timeWhenStarted = Time.time;
			base.transform.SetParent(this.CharacterItem.transform, false);
			if (this.buffFxInstance)
			{
				global::UnityEngine.Object.Destroy(this.buffFxInstance.gameObject);
			}
			if (this.buffFxPfb && manager.Master && manager.Master.characterModel)
			{
				this.buffFxInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.buffFxPfb);
				Transform transform = manager.Master.characterModel.ArmorSocket;
				if (transform == null)
				{
					transform = manager.Master.transform;
				}
				this.buffFxInstance.transform.SetParent(transform);
				this.buffFxInstance.transform.position = transform.position;
				this.buffFxInstance.transform.localRotation = Quaternion.identity;
			}
			foreach (Effect effect in this.effects)
			{
				effect.SetItem(this.CharacterItem);
			}
			this.OnSetup();
			UnityEvent onSetupEvent = this.OnSetupEvent;
			if (onSetupEvent == null)
			{
				return;
			}
			onSetupEvent.Invoke();
		}

		// Token: 0x060024EF RID: 9455 RVA: 0x0007F874 File Offset: 0x0007DA74
		internal void NotifyUpdate()
		{
			this.OnUpdate();
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0007F87C File Offset: 0x0007DA7C
		internal void NotifyOutOfTime()
		{
			this.OnNotifiedOutOfTime();
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x0007F88F File Offset: 0x0007DA8F
		internal virtual void NotifyIncomingBuffWithSameID(Buff incomingPrefab)
		{
			this.timeWhenStarted = Time.time;
			if (this.CurrentLayers < this.maxLayers)
			{
				this.CurrentLayers += incomingPrefab.CurrentLayers;
			}
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x0007F8BD File Offset: 0x0007DABD
		protected virtual void OnSetup()
		{
		}

		// Token: 0x060024F3 RID: 9459 RVA: 0x0007F8BF File Offset: 0x0007DABF
		protected virtual void OnUpdate()
		{
		}

		// Token: 0x060024F4 RID: 9460 RVA: 0x0007F8C1 File Offset: 0x0007DAC1
		protected virtual void OnNotifiedOutOfTime()
		{
		}

		// Token: 0x060024F5 RID: 9461 RVA: 0x0007F8C3 File Offset: 0x0007DAC3
		private void OnDestroy()
		{
			if (this.buffFxInstance)
			{
				global::UnityEngine.Object.Destroy(this.buffFxInstance.gameObject);
			}
		}

		// Token: 0x04001925 RID: 6437
		[SerializeField]
		private int id;

		// Token: 0x04001926 RID: 6438
		[SerializeField]
		private int maxLayers = 1;

		// Token: 0x04001927 RID: 6439
		[SerializeField]
		private Buff.BuffExclusiveTags exclusiveTag;

		// Token: 0x04001928 RID: 6440
		[Tooltip("优先级高的代替优先级低的。同优先级，选剩余时间长的。如果一方不限制时长，选后来的")]
		[SerializeField]
		private int exclusiveTagPriority;

		// Token: 0x04001929 RID: 6441
		[LocalizationKey("Buffs")]
		[SerializeField]
		private string displayName;

		// Token: 0x0400192A RID: 6442
		[LocalizationKey("Buffs")]
		[SerializeField]
		private string description;

		// Token: 0x0400192B RID: 6443
		[SerializeField]
		private Sprite icon;

		// Token: 0x0400192C RID: 6444
		[SerializeField]
		private bool limitedLifeTime;

		// Token: 0x0400192D RID: 6445
		[SerializeField]
		private float totalLifeTime;

		// Token: 0x0400192E RID: 6446
		[SerializeField]
		private List<Effect> effects = new List<Effect>();

		// Token: 0x0400192F RID: 6447
		[SerializeField]
		private bool hide;

		// Token: 0x04001930 RID: 6448
		[SerializeField]
		private int currentLayers = 1;

		// Token: 0x04001931 RID: 6449
		private CharacterBuffManager master;

		// Token: 0x04001932 RID: 6450
		public UnityEvent OnSetupEvent;

		// Token: 0x04001934 RID: 6452
		[SerializeField]
		private GameObject buffFxPfb;

		// Token: 0x04001935 RID: 6453
		private GameObject buffFxInstance;

		// Token: 0x04001936 RID: 6454
		[HideInInspector]
		public CharacterMainControl fromWho;

		// Token: 0x04001937 RID: 6455
		public int fromWeaponID;

		// Token: 0x04001938 RID: 6456
		private float timeWhenStarted;

		// Token: 0x02000662 RID: 1634
		public enum BuffExclusiveTags
		{
			// Token: 0x040022FB RID: 8955
			NotExclusive,
			// Token: 0x040022FC RID: 8956
			Bleeding,
			// Token: 0x040022FD RID: 8957
			Starve,
			// Token: 0x040022FE RID: 8958
			Thirsty,
			// Token: 0x040022FF RID: 8959
			Weight,
			// Token: 0x04002300 RID: 8960
			Poison,
			// Token: 0x04002301 RID: 8961
			Pain,
			// Token: 0x04002302 RID: 8962
			Electric,
			// Token: 0x04002303 RID: 8963
			Burning,
			// Token: 0x04002304 RID: 8964
			Space,
			// Token: 0x04002305 RID: 8965
			StormProtection,
			// Token: 0x04002306 RID: 8966
			Nauseous,
			// Token: 0x04002307 RID: 8967
			Stun
		}
	}
}
