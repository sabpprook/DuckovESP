using System;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002FE RID: 766
	public class VerticalEntry : MonoBehaviour
	{
		// Token: 0x060018CB RID: 6347 RVA: 0x0005A538 File Offset: 0x00058738
		public void Setup(params string[] args)
		{
		}

		// Token: 0x060018CC RID: 6348 RVA: 0x0005A53A File Offset: 0x0005873A
		public void SetLayoutSpacing(float spacing)
		{
			this.layoutGroup.spacing = spacing;
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0005A548 File Offset: 0x00058748
		public void SetPreferredWidth(float width)
		{
			this.layoutElement.preferredWidth = width;
		}

		// Token: 0x0400120F RID: 4623
		[SerializeField]
		private VerticalLayoutGroup layoutGroup;

		// Token: 0x04001210 RID: 4624
		[SerializeField]
		private LayoutElement layoutElement;
	}
}
