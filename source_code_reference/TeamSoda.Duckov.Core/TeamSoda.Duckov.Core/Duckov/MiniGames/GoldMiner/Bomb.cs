using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028A RID: 650
	public class Bomb : MiniGameBehaviour
	{
		// Token: 0x060014FE RID: 5374 RVA: 0x0004DB84 File Offset: 0x0004BD84
		protected override void OnUpdate(float deltaTime)
		{
			base.transform.position += base.transform.up * this.moveSpeed * deltaTime;
			this.hoveringTargets.RemoveAll((GoldMinerEntity e) => e == null);
			if (this.hoveringTargets.Count > 0)
			{
				this.Explode(this.hoveringTargets[0]);
			}
			this.lifeTime += deltaTime;
			if (this.lifeTime > this.maxLifeTime)
			{
				global::UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0004DC35 File Offset: 0x0004BE35
		private void Explode(GoldMinerEntity goldMinerTarget)
		{
			goldMinerTarget.Explode(base.transform.position);
			FXPool.Play(this.explodeFX, base.transform.position, base.transform.rotation);
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x0004DC78 File Offset: 0x0004BE78
		private void OnCollisionEnter2D(Collision2D collision)
		{
			GoldMinerEntity component = collision.gameObject.GetComponent<GoldMinerEntity>();
			if (component != null)
			{
				this.hoveringTargets.Add(component);
			}
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x0004DCA8 File Offset: 0x0004BEA8
		private void OnCollisionExit2D(Collision2D collision)
		{
			GoldMinerEntity component = collision.gameObject.GetComponent<GoldMinerEntity>();
			if (component != null)
			{
				this.hoveringTargets.Remove(component);
			}
		}

		// Token: 0x04000F69 RID: 3945
		[SerializeField]
		private float moveSpeed;

		// Token: 0x04000F6A RID: 3946
		[SerializeField]
		private float maxLifeTime = 10f;

		// Token: 0x04000F6B RID: 3947
		[SerializeField]
		private ParticleSystem explodeFX;

		// Token: 0x04000F6C RID: 3948
		private float lifeTime;

		// Token: 0x04000F6D RID: 3949
		private List<GoldMinerEntity> hoveringTargets = new List<GoldMinerEntity>();
	}
}
