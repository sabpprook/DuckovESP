using System;
using UnityEngine;

namespace Duckov.UI.Animations
{
	// Token: 0x020003D7 RID: 983
	public class LooperClock : MonoBehaviour
	{
		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x060023BA RID: 9146 RVA: 0x0007C9E1 File Offset: 0x0007ABE1
		public float t
		{
			get
			{
				if (this.duration > 0f)
				{
					return this.time / this.duration;
				}
				return 1f;
			}
		}

		// Token: 0x140000EF RID: 239
		// (add) Token: 0x060023BB RID: 9147 RVA: 0x0007CA04 File Offset: 0x0007AC04
		// (remove) Token: 0x060023BC RID: 9148 RVA: 0x0007CA3C File Offset: 0x0007AC3C
		public event Action<LooperClock, float> onTick;

		// Token: 0x060023BD RID: 9149 RVA: 0x0007CA71 File Offset: 0x0007AC71
		private void Update()
		{
			if (this.duration > 0f)
			{
				this.time += Time.unscaledDeltaTime;
				this.time %= this.duration;
				this.Tick();
			}
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x0007CAAB File Offset: 0x0007ACAB
		private void Tick()
		{
			Action<LooperClock, float> action = this.onTick;
			if (action == null)
			{
				return;
			}
			action(this, this.t);
		}

		// Token: 0x04001842 RID: 6210
		[SerializeField]
		private float duration = 1f;

		// Token: 0x04001843 RID: 6211
		private float time;
	}
}
