using System;
using UnityEngine;

namespace Duckov.UI.BarDisplays
{
	// Token: 0x020003C9 RID: 969
	public class BarDisplayController : MonoBehaviour
	{
		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002335 RID: 9013 RVA: 0x0007B44F File Offset: 0x0007964F
		protected virtual float Current
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06002336 RID: 9014 RVA: 0x0007B456 File Offset: 0x00079656
		protected virtual float Max
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x0007B460 File Offset: 0x00079660
		protected void Refresh()
		{
			float num = this.Current;
			float max = this.Max;
			this.bar.SetValue(num, max, "0.#", 0f);
		}

		// Token: 0x040017F8 RID: 6136
		[SerializeField]
		private BarDisplay bar;
	}
}
