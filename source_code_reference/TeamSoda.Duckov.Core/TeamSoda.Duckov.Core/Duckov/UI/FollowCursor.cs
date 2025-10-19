using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Duckov.UI
{
	// Token: 0x020003BF RID: 959
	public class FollowCursor : MonoBehaviour
	{
		// Token: 0x060022EA RID: 8938 RVA: 0x0007A423 File Offset: 0x00078623
		private void Awake()
		{
			this.parentRectTransform = base.transform.parent as RectTransform;
			this.rectTransform = base.transform as RectTransform;
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x0007A44C File Offset: 0x0007864C
		private unsafe void Update()
		{
			Vector2 vector = *Mouse.current.position.value;
			Vector2 vector2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.parentRectTransform, vector, null, out vector2);
			this.rectTransform.localPosition = vector2;
		}

		// Token: 0x040017C2 RID: 6082
		private RectTransform parentRectTransform;

		// Token: 0x040017C3 RID: 6083
		private RectTransform rectTransform;
	}
}
