using System;
using UnityEngine;
using UnityEngine.Events;

namespace ItemStatsSystem
{
	// Token: 0x02000227 RID: 551
	[MenuPath("General/On Take Damage")]
	public class OnTakeDamageTrigger : EffectTrigger
	{
		// Token: 0x060010FD RID: 4349 RVA: 0x00041E41 File Offset: 0x00040041
		private void OnEnable()
		{
			this.RegisterEvents();
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00041E49 File Offset: 0x00040049
		protected override void OnDisable()
		{
			base.OnDisable();
			this.UnregisterEvents();
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00041E57 File Offset: 0x00040057
		protected override void OnMasterSetTargetItem(Effect effect, Item item)
		{
			this.RegisterEvents();
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00041E60 File Offset: 0x00040060
		private void RegisterEvents()
		{
			this.UnregisterEvents();
			if (base.Master == null)
			{
				return;
			}
			Item item = base.Master.Item;
			if (item == null)
			{
				return;
			}
			CharacterMainControl characterMainControl = item.GetCharacterMainControl();
			if (characterMainControl == null)
			{
				return;
			}
			this.target = characterMainControl.Health;
			this.target.OnHurtEvent.AddListener(new UnityAction<DamageInfo>(this.OnTookDamage));
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00041ED1 File Offset: 0x000400D1
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.OnHurtEvent.RemoveListener(new UnityAction<DamageInfo>(this.OnTookDamage));
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00041EFE File Offset: 0x000400FE
		private void OnTookDamage(DamageInfo info)
		{
			if (info.damageValue < (float)this.threshold)
			{
				return;
			}
			base.Trigger(true);
		}

		// Token: 0x04000D45 RID: 3397
		[SerializeField]
		public int threshold;

		// Token: 0x04000D46 RID: 3398
		private Health target;
	}
}
