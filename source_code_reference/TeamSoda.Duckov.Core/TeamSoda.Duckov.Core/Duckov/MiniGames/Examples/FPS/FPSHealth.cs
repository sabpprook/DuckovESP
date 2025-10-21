using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002D4 RID: 724
	public class FPSHealth : MiniGameBehaviour
	{
		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x060016BD RID: 5821 RVA: 0x0005335D File Offset: 0x0005155D
		public int HP
		{
			get
			{
				return this.hp;
			}
		}

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x060016BE RID: 5822 RVA: 0x00053365 File Offset: 0x00051565
		public bool Dead
		{
			get
			{
				return this.dead;
			}
		}

		// Token: 0x14000094 RID: 148
		// (add) Token: 0x060016BF RID: 5823 RVA: 0x00053370 File Offset: 0x00051570
		// (remove) Token: 0x060016C0 RID: 5824 RVA: 0x000533A8 File Offset: 0x000515A8
		public event Action<FPSHealth> onDead;

		// Token: 0x060016C1 RID: 5825 RVA: 0x000533E0 File Offset: 0x000515E0
		protected override void Start()
		{
			base.Start();
			this.hp = this.maxHp;
			this.materialPropertyBlock = new MaterialPropertyBlock();
			foreach (FPSDamageReceiver fpsdamageReceiver in this.damageReceivers)
			{
				fpsdamageReceiver.onReceiveDamage += this.OnReceiverReceiveDamage;
			}
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0005345C File Offset: 0x0005165C
		protected override void OnUpdate(float deltaTime)
		{
			if (this.hurtValue > 0f)
			{
				this.hurtValue = Mathf.MoveTowards(this.hurtValue, 0f, deltaTime * this.hurtValueDropRate);
			}
			this.materialPropertyBlock.SetFloat("_HurtValue", this.hurtValue);
			this.meshRenderer.SetPropertyBlock(this.materialPropertyBlock, 0);
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x000534BC File Offset: 0x000516BC
		private void OnReceiverReceiveDamage(FPSDamageReceiver receiver, FPSDamageInfo info)
		{
			this.ReceiveDamage(info);
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x000534C8 File Offset: 0x000516C8
		private void ReceiveDamage(FPSDamageInfo info)
		{
			if (this.dead)
			{
				return;
			}
			this.hurtValue = 1f;
			this.hp -= Mathf.FloorToInt(info.amount);
			if (this.hp <= 0)
			{
				this.hp = 0;
				this.Die();
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00053517 File Offset: 0x00051717
		private void Die()
		{
			this.dead = true;
			Action<FPSHealth> action = this.onDead;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x040010AA RID: 4266
		[SerializeField]
		private int maxHp;

		// Token: 0x040010AB RID: 4267
		[SerializeField]
		private List<FPSDamageReceiver> damageReceivers;

		// Token: 0x040010AC RID: 4268
		[SerializeField]
		private MeshRenderer meshRenderer;

		// Token: 0x040010AD RID: 4269
		[SerializeField]
		private float hurtValueDropRate = 1f;

		// Token: 0x040010AE RID: 4270
		private int hp;

		// Token: 0x040010AF RID: 4271
		private bool dead;

		// Token: 0x040010B0 RID: 4272
		private float hurtValue;

		// Token: 0x040010B2 RID: 4274
		private MaterialPropertyBlock materialPropertyBlock;
	}
}
