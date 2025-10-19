using System;
using TMPro;
using UnityEngine;

namespace Duckov.CreditsUtility
{
	// Token: 0x020002FD RID: 765
	public class TextEntry : MonoBehaviour
	{
		// Token: 0x060018C9 RID: 6345 RVA: 0x0005A4C8 File Offset: 0x000586C8
		internal void Setup(string text, Color color, int size = -1, bool bold = false)
		{
			this.text.text = text;
			if (size < 0)
			{
				size = this.defaultSize;
			}
			this.text.color = color;
			this.text.fontSize = (float)size;
			this.text.fontStyle = (this.text.fontStyle & ~FontStyles.Bold) | (bold ? FontStyles.Bold : FontStyles.Normal);
		}

		// Token: 0x0400120D RID: 4621
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x0400120E RID: 4622
		public int defaultSize = 26;
	}
}
