using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D8 RID: 984
	public abstract class LooperElement : MonoBehaviour
	{
		// Token: 0x060023C0 RID: 9152 RVA: 0x0007CAD7 File Offset: 0x0007ACD7
		protected virtual void OnEnable()
		{
			this.clock.onTick += this.OnTick;
			this.OnTick(this.clock, this.clock.t);
		}

		// Token: 0x060023C1 RID: 9153 RVA: 0x0007CB08 File Offset: 0x0007AD08
		protected virtual void OnDisable()
		{
			if (this.clock != null)
			{
				this.clock.onTick -= this.OnTick;
			}
		}

		// Token: 0x060023C2 RID: 9154
		protected abstract void OnTick(LooperClock clock, float t);

		// Token: 0x04001845 RID: 6213
		[SerializeField]
		private LooperClock clock;
	}
}
