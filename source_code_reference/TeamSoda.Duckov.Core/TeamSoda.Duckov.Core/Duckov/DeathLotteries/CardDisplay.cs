using System;
using Duckov.UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.DeathLotteries
{
	// Token: 0x02000301 RID: 769
	public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerMoveHandler
	{
		// Token: 0x060018ED RID: 6381 RVA: 0x0005AA12 File Offset: 0x00058C12
		private void Awake()
		{
			this.rectTransform = base.transform as RectTransform;
			this.RefreshFadeGroups();
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x0005AA2C File Offset: 0x00058C2C
		private void CacheRadius()
		{
			this.cachedRect = this.rectTransform.rect;
			Rect rect = this.cachedRect;
			this.cachedRadius = Mathf.Sqrt(rect.width * rect.width + rect.height * rect.height) / 2f;
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x0005AA81 File Offset: 0x00058C81
		private void Update()
		{
			if (this.rectTransform.rect != this.cachedRect)
			{
				this.CacheRadius();
			}
			this.HandleAnimation();
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x0005AAA8 File Offset: 0x00058CA8
		private void HandleAnimation()
		{
			Quaternion quaternion = this.cardTransform.rotation;
			if ((this.facingFront && !this.frontFadeGroup.IsShown) || (!this.facingFront && !this.backFadeGroup.IsShown))
			{
				quaternion = Quaternion.RotateTowards(quaternion, Quaternion.Euler(0f, 90f, 0f), this.flipSpeed * Time.deltaTime);
				if (Mathf.Approximately(Quaternion.Angle(quaternion, Quaternion.Euler(0f, 90f, 0f)), 0f))
				{
					quaternion = Quaternion.Euler(0f, -90f, 0f);
					this.RefreshFadeGroups();
				}
			}
			else
			{
				quaternion = Quaternion.RotateTowards(quaternion, this.GetIdealRotation(), this.rotateSpeed * Time.deltaTime);
			}
			this.cardTransform.rotation = quaternion;
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x0005AB83 File Offset: 0x00058D83
		private void OnEnable()
		{
			this.CacheRadius();
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x0005AB8C File Offset: 0x00058D8C
		private Quaternion GetIdealRotation()
		{
			if (this.rectTransform.rect != this.cachedRect)
			{
				this.CacheRadius();
			}
			if (this.hovering && !Mathf.Approximately(this.cachedRadius, 0f))
			{
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, this.pointerPosition, null, out vector);
				Vector2 center = this.rectTransform.rect.center;
				Vector2 vector2 = vector - center;
				float num = Mathf.Max(10f, this.cachedRadius);
				Vector2 vector3 = Vector2.ClampMagnitude(vector2 / num, 1f);
				return Quaternion.Euler(-vector3.y * this.idleAmp, -vector3.x * this.idleAmp, 0f);
			}
			return Quaternion.Euler(Mathf.Sin(Time.time * this.idleFrequency * 3.1415927f * 2f) * this.idleAmp, Mathf.Cos(Time.time * this.idleFrequency * 3.1415927f * 2f) * this.idleAmp, 0f);
		}

		// Token: 0x060018F3 RID: 6387 RVA: 0x0005ACA0 File Offset: 0x00058EA0
		private void SkipAnimation()
		{
			this.RefreshFadeGroups();
			this.cardTransform.rotation = this.GetIdealRotation();
		}

		// Token: 0x060018F4 RID: 6388 RVA: 0x0005ACB9 File Offset: 0x00058EB9
		public void SetFacing(bool facingFront, bool skipAnimation = false)
		{
			this.facingFront = facingFront;
			if (skipAnimation)
			{
				this.SkipAnimation();
			}
		}

		// Token: 0x060018F5 RID: 6389 RVA: 0x0005ACCB File Offset: 0x00058ECB
		public void Flip()
		{
			this.SetFacing(!this.facingFront, false);
		}

		// Token: 0x060018F6 RID: 6390 RVA: 0x0005ACDD File Offset: 0x00058EDD
		private void RefreshFadeGroups()
		{
			if (this.facingFront)
			{
				this.frontFadeGroup.Show();
				this.backFadeGroup.Hide();
				return;
			}
			this.frontFadeGroup.Hide();
			this.backFadeGroup.Show();
		}

		// Token: 0x060018F7 RID: 6391 RVA: 0x0005AD14 File Offset: 0x00058F14
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.hovering = true;
		}

		// Token: 0x060018F8 RID: 6392 RVA: 0x0005AD1D File Offset: 0x00058F1D
		public void OnPointerExit(PointerEventData eventData)
		{
			this.hovering = false;
		}

		// Token: 0x060018F9 RID: 6393 RVA: 0x0005AD26 File Offset: 0x00058F26
		public void OnPointerMove(PointerEventData eventData)
		{
			this.pointerPosition = eventData.position;
		}

		// Token: 0x0400121C RID: 4636
		private RectTransform rectTransform;

		// Token: 0x0400121D RID: 4637
		[SerializeField]
		private RectTransform cardTransform;

		// Token: 0x0400121E RID: 4638
		[SerializeField]
		private FadeGroup frontFadeGroup;

		// Token: 0x0400121F RID: 4639
		[SerializeField]
		private FadeGroup backFadeGroup;

		// Token: 0x04001220 RID: 4640
		[SerializeField]
		private float idleAmp = 10f;

		// Token: 0x04001221 RID: 4641
		[SerializeField]
		private float idleFrequency = 0.5f;

		// Token: 0x04001222 RID: 4642
		[SerializeField]
		private float rotateSpeed = 300f;

		// Token: 0x04001223 RID: 4643
		[SerializeField]
		private float flipSpeed = 300f;

		// Token: 0x04001224 RID: 4644
		private bool facingFront;

		// Token: 0x04001225 RID: 4645
		private bool hovering;

		// Token: 0x04001226 RID: 4646
		private Vector2 pointerPosition;

		// Token: 0x04001227 RID: 4647
		private Rect cachedRect;

		// Token: 0x04001228 RID: 4648
		private float cachedRadius;
	}
}
