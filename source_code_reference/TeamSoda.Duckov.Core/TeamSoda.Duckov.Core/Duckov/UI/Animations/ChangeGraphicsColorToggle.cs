using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.UI.Animations
{
	// Token: 0x020003DD RID: 989
	public class ChangeGraphicsColorToggle : ToggleComponent
	{
		// Token: 0x060023D4 RID: 9172 RVA: 0x0007CE81 File Offset: 0x0007B081
		protected override void OnSetToggle(ToggleAnimation master, bool value)
		{
			this.image.DOKill(false);
			this.image.DOColor(value ? this.trueColor : this.falseColor, this.duration);
		}

		// Token: 0x04001853 RID: 6227
		[SerializeField]
		private Image image;

		// Token: 0x04001854 RID: 6228
		[SerializeField]
		private Color trueColor;

		// Token: 0x04001855 RID: 6229
		[SerializeField]
		private Color falseColor;

		// Token: 0x04001856 RID: 6230
		[SerializeField]
		private float duration = 0.1f;
	}
}
