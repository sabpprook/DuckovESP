using System;
using UnityEngine;

namespace Duckov.MiniGames.Examples.FPS
{
	// Token: 0x020002CD RID: 717
	public class UICrossHair : MiniGameBehaviour
	{
		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001697 RID: 5783 RVA: 0x00052C00 File Offset: 0x00050E00
		private float ScatterAngle
		{
			get
			{
				if (this.gunControl)
				{
					return this.gunControl.ScatterAngle;
				}
				return 0f;
			}
		}

		// Token: 0x06001698 RID: 5784 RVA: 0x00052C20 File Offset: 0x00050E20
		private void Awake()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = base.GetComponent<RectTransform>();
			}
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x00052C3C File Offset: 0x00050E3C
		protected override void OnUpdate(float deltaTime)
		{
			float scatterAngle = this.ScatterAngle;
			float fieldOfView = base.Game.Camera.fieldOfView;
			float y = this.canvasRectTransform.sizeDelta.y;
			float num = scatterAngle / fieldOfView;
			float num2 = (float)(Mathf.FloorToInt(y * num / 2f) * 2 + 1);
			this.rectTransform.sizeDelta = num2 * Vector2.one;
		}

		// Token: 0x04001086 RID: 4230
		[SerializeField]
		private RectTransform rectTransform;

		// Token: 0x04001087 RID: 4231
		[SerializeField]
		private RectTransform canvasRectTransform;

		// Token: 0x04001088 RID: 4232
		[SerializeField]
		private FPSGunControl gunControl;
	}
}
