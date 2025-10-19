using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000296 RID: 662
	public class ExplodeOnAttach : MiniGameBehaviour
	{
		// Token: 0x060015A6 RID: 5542 RVA: 0x00050260 File Offset: 0x0004E460
		private void Awake()
		{
			if (this.master == null)
			{
				this.master = base.GetComponent<GoldMinerEntity>();
			}
			if (this.goldMiner == null)
			{
				this.goldMiner = base.GetComponentInParent<GoldMiner>();
			}
			GoldMinerEntity goldMinerEntity = this.master;
			goldMinerEntity.OnAttached = (Action<GoldMinerEntity, Hook>)Delegate.Combine(goldMinerEntity.OnAttached, new Action<GoldMinerEntity, Hook>(this.OnAttached));
		}

		// Token: 0x060015A7 RID: 5543 RVA: 0x000502C8 File Offset: 0x0004E4C8
		private void OnAttached(GoldMinerEntity target, Hook hook)
		{
			if (this.goldMiner == null)
			{
				return;
			}
			if (this.goldMiner.run == null)
			{
				return;
			}
			if (this.goldMiner.run.defuse.Value > 0.1f)
			{
				return;
			}
			Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, this.explodeRange);
			for (int i = 0; i < array.Length; i++)
			{
				GoldMinerEntity component = array[i].GetComponent<GoldMinerEntity>();
				if (!(component == null))
				{
					component.Explode(base.transform.position);
				}
			}
			this.master.Explode(base.transform.position);
		}

		// Token: 0x060015A8 RID: 5544 RVA: 0x00050372 File Offset: 0x0004E572
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.explodeRange);
		}

		// Token: 0x04001005 RID: 4101
		[SerializeField]
		private GoldMiner goldMiner;

		// Token: 0x04001006 RID: 4102
		[SerializeField]
		private GoldMinerEntity master;

		// Token: 0x04001007 RID: 4103
		[SerializeField]
		private float explodeRange;
	}
}
