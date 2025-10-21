using System;
using Duckov.Buffs;
using ItemStatsSystem;
using UnityEngine;

namespace Duckov.Effects
{
	// Token: 0x020003EB RID: 1003
	public class DamageAction : EffectAction
	{
		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06002432 RID: 9266 RVA: 0x0007DDEC File Offset: 0x0007BFEC
		private CharacterMainControl MainControl
		{
			get
			{
				Effect master = base.Master;
				if (master == null)
				{
					return null;
				}
				Item item = master.Item;
				if (item == null)
				{
					return null;
				}
				return item.GetCharacterMainControl();
			}
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x0007DE0C File Offset: 0x0007C00C
		protected override void OnTriggeredPositive()
		{
			if (this.MainControl == null)
			{
				return;
			}
			if (this.MainControl.Health == null)
			{
				return;
			}
			this.damageInfo.isFromBuffOrEffect = true;
			if (this.buff != null)
			{
				this.damageInfo.fromCharacter = this.buff.fromWho;
				this.damageInfo.fromWeaponItemID = this.buff.fromWeaponID;
			}
			this.damageInfo.damagePoint = this.MainControl.transform.position + Vector3.up * 0.8f;
			this.damageInfo.damageNormal = Vector3.up;
			if (this.percentDamage && this.MainControl.Health != null)
			{
				this.damageInfo.damageValue = this.percentDamageValue * this.MainControl.Health.MaxHealth * ((this.buff == null) ? 1f : ((float)this.buff.CurrentLayers));
			}
			else
			{
				this.damageInfo.damageValue = this.damageValue * ((this.buff == null) ? 1f : ((float)this.buff.CurrentLayers));
			}
			this.MainControl.Health.Hurt(this.damageInfo);
			if (this.fx)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(this.fx, this.damageInfo.damagePoint, Quaternion.identity);
			}
		}

		// Token: 0x040018A4 RID: 6308
		[SerializeField]
		private Buff buff;

		// Token: 0x040018A5 RID: 6309
		[SerializeField]
		private bool percentDamage;

		// Token: 0x040018A6 RID: 6310
		[SerializeField]
		private float damageValue = 1f;

		// Token: 0x040018A7 RID: 6311
		[SerializeField]
		private float percentDamageValue;

		// Token: 0x040018A8 RID: 6312
		[SerializeField]
		private DamageInfo damageInfo = new DamageInfo(null);

		// Token: 0x040018A9 RID: 6313
		[SerializeField]
		private GameObject fx;
	}
}
