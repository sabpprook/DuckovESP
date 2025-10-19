using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x020003C0 RID: 960
	[ExecuteInEditMode]
	public class RectTransformChangeEventEmitter : UIBehaviour
	{
		// Token: 0x140000EB RID: 235
		// (add) Token: 0x060022ED RID: 8941 RVA: 0x0007A498 File Offset: 0x00078698
		// (remove) Token: 0x060022EE RID: 8942 RVA: 0x0007A4D0 File Offset: 0x000786D0
		public event Action<RectTransform> OnRectTransformChange;

		// Token: 0x060022EF RID: 8943 RVA: 0x0007A505 File Offset: 0x00078705
		private void SetDirty()
		{
			Action<RectTransform> onRectTransformChange = this.OnRectTransformChange;
			if (onRectTransformChange == null)
			{
				return;
			}
			onRectTransformChange(base.transform as RectTransform);
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x0007A522 File Offset: 0x00078722
		protected override void OnRectTransformDimensionsChange()
		{
			this.SetDirty();
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x0007A52A File Offset: 0x0007872A
		protected override void OnEnable()
		{
			this.SetDirty();
		}
	}
}
