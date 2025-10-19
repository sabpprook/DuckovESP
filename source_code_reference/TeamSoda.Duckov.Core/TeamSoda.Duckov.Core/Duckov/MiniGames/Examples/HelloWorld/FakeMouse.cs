using System;
using UnityEngine;

namespace Duckov.MiniGames.Examples.HelloWorld
{
	// Token: 0x020002CB RID: 715
	public class FakeMouse : MiniGameBehaviour
	{
		// Token: 0x0600168E RID: 5774 RVA: 0x00052A3D File Offset: 0x00050C3D
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
			this.parentRectTransform = base.transform.parent as RectTransform;
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x00052A68 File Offset: 0x00050C68
		protected override void OnUpdate(float deltaTime)
		{
			Vector3 vector = this.rectTransform.localPosition;
			vector += base.Game.GetAxis(1) * this.sensitivity;
			Rect rect = this.parentRectTransform.rect;
			vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax);
			vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax);
			this.rectTransform.localPosition = vector;
		}

		// Token: 0x0400107F RID: 4223
		[SerializeField]
		private float sensitivity = 1f;

		// Token: 0x04001080 RID: 4224
		private RectTransform rectTransform;

		// Token: 0x04001081 RID: 4225
		private RectTransform parentRectTransform;
	}
}
