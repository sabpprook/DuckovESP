using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000297 RID: 663
	public class PigBehaviour : MiniGameBehaviour
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x0005039C File Offset: 0x0004E59C
		private void Awake()
		{
			if (this.entity == null)
			{
				this.entity = base.GetComponent<GoldMinerEntity>();
			}
			GoldMinerEntity goldMinerEntity = this.entity;
			goldMinerEntity.OnAttached = (Action<GoldMinerEntity, Hook>)Delegate.Combine(goldMinerEntity.OnAttached, new Action<GoldMinerEntity, Hook>(this.OnAttached));
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x000503EC File Offset: 0x0004E5EC
		protected override void OnUpdate(float deltaTime)
		{
			Quaternion quaternion = Quaternion.AngleAxis((float)(this.movingRight ? 0 : 180), Vector3.up);
			base.transform.localRotation = quaternion;
			base.transform.localPosition += (this.movingRight ? Vector3.right : Vector3.left) * this.moveSpeed * this.entity.master.run.GameSpeedFactor * deltaTime;
			if (base.transform.localPosition.x > this.entity.master.Bounds.max.x)
			{
				this.movingRight = false;
				return;
			}
			if (base.transform.localPosition.x < this.entity.master.Bounds.min.x)
			{
				this.movingRight = true;
			}
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x000504E3 File Offset: 0x0004E6E3
		private void OnAttached(GoldMinerEntity entity, Hook hook)
		{
		}

		// Token: 0x04001008 RID: 4104
		[SerializeField]
		private GoldMinerEntity entity;

		// Token: 0x04001009 RID: 4105
		[SerializeField]
		private float moveSpeed = 50f;

		// Token: 0x0400100A RID: 4106
		private bool attached;

		// Token: 0x0400100B RID: 4107
		private bool movingRight;
	}
}
