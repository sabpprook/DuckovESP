using System;
using UnityEngine;

namespace ItemStatsSystem
{
	// Token: 0x02000226 RID: 550
	[MenuPath("General/On Shoot&Attack")]
	public class OnShootAttackTrigger : EffectTrigger
	{
		// Token: 0x060010F6 RID: 4342 RVA: 0x00041D13 File Offset: 0x0003FF13
		private void OnEnable()
		{
			this.RegisterEvents();
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x00041D1B File Offset: 0x0003FF1B
		protected override void OnDisable()
		{
			base.OnDisable();
			this.UnregisterEvents();
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00041D29 File Offset: 0x0003FF29
		protected override void OnMasterSetTargetItem(Effect effect, Item item)
		{
			this.RegisterEvents();
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00041D34 File Offset: 0x0003FF34
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
			this.target = item.GetCharacterMainControl();
			if (this.target == null)
			{
				return;
			}
			if (this.onShoot)
			{
				this.target.OnShootEvent += this.OnShootAttack;
			}
			if (this.onAttack)
			{
				this.target.OnAttackEvent += this.OnShootAttack;
			}
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00041DC8 File Offset: 0x0003FFC8
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			if (this.onShoot)
			{
				this.target.OnShootEvent -= this.OnShootAttack;
			}
			if (this.onAttack)
			{
				this.target.OnAttackEvent -= this.OnShootAttack;
			}
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00041E22 File Offset: 0x00040022
		private void OnShootAttack(DuckovItemAgent agent)
		{
			base.Trigger(true);
		}

		// Token: 0x04000D42 RID: 3394
		[SerializeField]
		private bool onShoot = true;

		// Token: 0x04000D43 RID: 3395
		[SerializeField]
		private bool onAttack = true;

		// Token: 0x04000D44 RID: 3396
		private CharacterMainControl target;
	}
}
