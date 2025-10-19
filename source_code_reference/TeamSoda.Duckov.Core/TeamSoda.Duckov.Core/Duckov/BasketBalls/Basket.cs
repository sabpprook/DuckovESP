using System;
using System.Collections.Generic;
using Duckov.Quests;
using UnityEngine;
using UnityEngine.Events;

namespace Duckov.BasketBalls
{
	// Token: 0x0200030B RID: 779
	public class Basket : MonoBehaviour
	{
		// Token: 0x060019A7 RID: 6567 RVA: 0x0005CACE File Offset: 0x0005ACCE
		private void Awake()
		{
			this.trigger.onGoal.AddListener(new UnityAction<BasketBall>(this.OnGoal));
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x0005CAEC File Offset: 0x0005ACEC
		private void OnGoal(BasketBall ball)
		{
			if (!this.conditions.Satisfied())
			{
				return;
			}
			this.onGoal.Invoke(ball);
			this.netAnimator.SetTrigger("Goal");
		}

		// Token: 0x0400128D RID: 4749
		[SerializeField]
		private Animator netAnimator;

		// Token: 0x0400128E RID: 4750
		[SerializeField]
		private List<Condition> conditions = new List<Condition>();

		// Token: 0x0400128F RID: 4751
		[SerializeField]
		private BasketTrigger trigger;

		// Token: 0x04001290 RID: 4752
		public UnityEvent<BasketBall> onGoal;
	}
}
