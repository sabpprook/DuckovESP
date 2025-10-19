using System;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x02000291 RID: 657
	public class HookHead : MonoBehaviour
	{
		// Token: 0x06001593 RID: 5523 RVA: 0x00050072 File Offset: 0x0004E272
		private void OnCollisionEnter2D(Collision2D collision)
		{
			Action<HookHead, Collision2D> action = this.onCollisionEnter;
			if (action == null)
			{
				return;
			}
			action(this, collision);
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x00050086 File Offset: 0x0004E286
		private void OnCollisionExit2D(Collision2D collision)
		{
			Action<HookHead, Collision2D> action = this.onCollisionExit;
			if (action == null)
			{
				return;
			}
			action(this, collision);
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0005009A File Offset: 0x0004E29A
		private void OnCollisionStay2D(Collision2D collision)
		{
			Action<HookHead, Collision2D> action = this.onCollisionStay;
			if (action == null)
			{
				return;
			}
			action(this, collision);
		}

		// Token: 0x04000FF8 RID: 4088
		public Action<HookHead, Collision2D> onCollisionEnter;

		// Token: 0x04000FF9 RID: 4089
		public Action<HookHead, Collision2D> onCollisionExit;

		// Token: 0x04000FFA RID: 4090
		public Action<HookHead, Collision2D> onCollisionStay;
	}
}
