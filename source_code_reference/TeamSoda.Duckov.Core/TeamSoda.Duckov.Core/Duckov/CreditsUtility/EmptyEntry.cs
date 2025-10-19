using System;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002FA RID: 762
	public class EmptyEntry : MonoBehaviour
	{
		// Token: 0x060018C1 RID: 6337 RVA: 0x0005A370 File Offset: 0x00058570
		public void Setup(params string[] args)
		{
			this.layoutElement.preferredWidth = this.defaultWidth;
			this.layoutElement.preferredHeight = this.defaultHeight;
			if (args == null)
			{
				return;
			}
			for (int i = 0; i < args.Length; i++)
			{
				if (i == 1)
				{
					this.TrySetWidth(args[i]);
				}
				if (i == 2)
				{
					this.TrySetHeight(args[i]);
				}
			}
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x0005A3CC File Offset: 0x000585CC
		private void TrySetWidth(string v)
		{
			float num;
			if (!float.TryParse(v, out num))
			{
				return;
			}
			this.layoutElement.preferredWidth = num;
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x0005A3F0 File Offset: 0x000585F0
		private void TrySetHeight(string v)
		{
			float num;
			if (!float.TryParse(v, out num))
			{
				return;
			}
			this.layoutElement.preferredHeight = num;
		}

		// Token: 0x04001208 RID: 4616
		[SerializeField]
		private LayoutElement layoutElement;

		// Token: 0x04001209 RID: 4617
		[SerializeField]
		private float defaultWidth;

		// Token: 0x0400120A RID: 4618
		[SerializeField]
		private float defaultHeight;
	}
}
