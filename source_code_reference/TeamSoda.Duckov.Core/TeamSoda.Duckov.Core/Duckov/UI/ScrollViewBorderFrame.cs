using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003C1 RID: 961
	public class ScrollViewBorderFrame : MonoBehaviour
	{
		// Token: 0x060022F3 RID: 8947 RVA: 0x0007A53A File Offset: 0x0007873A
		private void OnEnable()
		{
			this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.Refresh));
			UniTask.Void(async delegate
			{
				await UniTask.Yield();
				await UniTask.Yield();
				await UniTask.Yield();
				this.Refresh();
			});
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x0007A569 File Offset: 0x00078769
		private void OnDisable()
		{
			this.scrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.Refresh));
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x0007A587 File Offset: 0x00078787
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x0007A590 File Offset: 0x00078790
		private void Refresh(Vector2 scrollPos)
		{
			RectTransform viewport = this.scrollRect.viewport;
			RectTransform content = this.scrollRect.content;
			Rect rect = viewport.rect;
			Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);
			float num = bounds.max.y - rect.max.y + this.extendOffset;
			float num2 = rect.min.y - bounds.min.y + this.extendOffset;
			float num3 = rect.min.x - bounds.min.x + this.extendOffset;
			float num4 = bounds.max.x - rect.max.x + this.extendOffset;
			float num5 = Mathf.Lerp(0f, this.maxAlpha, num / this.extendThreshold);
			float num6 = Mathf.Lerp(0f, this.maxAlpha, num2 / this.extendThreshold);
			float num7 = Mathf.Lerp(0f, this.maxAlpha, num3 / this.extendThreshold);
			float num8 = Mathf.Lerp(0f, this.maxAlpha, num4 / this.extendThreshold);
			ScrollViewBorderFrame.<Refresh>g__SetAlpha|11_0(this.upGraphic, num5);
			ScrollViewBorderFrame.<Refresh>g__SetAlpha|11_0(this.downGraphic, num6);
			ScrollViewBorderFrame.<Refresh>g__SetAlpha|11_0(this.leftGraphic, num7);
			ScrollViewBorderFrame.<Refresh>g__SetAlpha|11_0(this.rightGraphic, num8);
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x0007A6E8 File Offset: 0x000788E8
		private void Refresh()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			this.Refresh(this.scrollRect.normalizedPosition);
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x0007A76C File Offset: 0x0007896C
		[CompilerGenerated]
		internal static void <Refresh>g__SetAlpha|11_0(Graphic graphic, float alpha)
		{
			if (graphic == null)
			{
				return;
			}
			Color color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}

		// Token: 0x040017C5 RID: 6085
		[SerializeField]
		private ScrollRect scrollRect;

		// Token: 0x040017C6 RID: 6086
		[Range(0f, 1f)]
		[SerializeField]
		private float maxAlpha = 1f;

		// Token: 0x040017C7 RID: 6087
		[SerializeField]
		private float extendThreshold = 10f;

		// Token: 0x040017C8 RID: 6088
		[SerializeField]
		private float extendOffset;

		// Token: 0x040017C9 RID: 6089
		[SerializeField]
		private Graphic upGraphic;

		// Token: 0x040017CA RID: 6090
		[SerializeField]
		private Graphic downGraphic;

		// Token: 0x040017CB RID: 6091
		[SerializeField]
		private Graphic leftGraphic;

		// Token: 0x040017CC RID: 6092
		[SerializeField]
		private Graphic rightGraphic;
	}
}
