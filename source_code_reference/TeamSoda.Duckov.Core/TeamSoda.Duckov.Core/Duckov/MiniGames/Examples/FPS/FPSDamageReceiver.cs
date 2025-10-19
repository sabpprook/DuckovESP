using System;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002D0 RID: 720
	public class FPSDamageReceiver : MonoBehaviour
	{
		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x060016A5 RID: 5797 RVA: 0x00052D36 File Offset: 0x00050F36
		public ParticleSystem DamageFX
		{
			get
			{
				if (GameManager.BloodFxOn)
				{
					return this.damageEffectPrefab;
				}
				return this.damageEffectPrefab_Censored;
			}
		}

		// Token: 0x14000093 RID: 147
		// (add) Token: 0x060016A6 RID: 5798 RVA: 0x00052D4C File Offset: 0x00050F4C
		// (remove) Token: 0x060016A7 RID: 5799 RVA: 0x00052D84 File Offset: 0x00050F84
		public event Action<FPSDamageReceiver, FPSDamageInfo> onReceiveDamage;

		// Token: 0x060016A8 RID: 5800 RVA: 0x00052DBC File Offset: 0x00050FBC
		internal void CastDamage(FPSDamageInfo damage)
		{
			if (this.DamageFX == null)
			{
				return;
			}
			FXPool.Play(this.DamageFX, damage.point, Quaternion.FromToRotation(Vector3.forward, damage.normal));
			Action<FPSDamageReceiver, FPSDamageInfo> action = this.onReceiveDamage;
			if (action == null)
			{
				return;
			}
			action(this, damage);
		}

		// Token: 0x0400108D RID: 4237
		[SerializeField]
		private ParticleSystem damageEffectPrefab;

		// Token: 0x0400108E RID: 4238
		[SerializeField]
		private ParticleSystem damageEffectPrefab_Censored;
	}
}
