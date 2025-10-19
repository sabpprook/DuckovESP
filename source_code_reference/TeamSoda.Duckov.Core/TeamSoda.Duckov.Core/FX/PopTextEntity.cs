using System;
using TMPro;
using UnityEngine;

namespace FX
{
	// Token: 0x0200020C RID: 524
	public class PopTextEntity : MonoBehaviour
	{
		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x0003D561 File Offset: 0x0003B761
		private RectTransform spriteRendererRectTransform
		{
			get
			{
				if (this._spriteRendererRectTransform_cache == null)
				{
					this._spriteRendererRectTransform_cache = this.spriteRenderer.GetComponent<RectTransform>();
				}
				return this._spriteRendererRectTransform_cache;
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x0003D588 File Offset: 0x0003B788
		private TextMeshPro tmp
		{
			get
			{
				return this._tmp;
			}
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0003D590 File Offset: 0x0003B790
		public TextMeshPro Tmp
		{
			get
			{
				return this.tmp;
			}
		}

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0003D598 File Offset: 0x0003B798
		public Color EndColor
		{
			get
			{
				return this.endColor;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x0003D5A0 File Offset: 0x0003B7A0
		// (set) Token: 0x06000F95 RID: 3989 RVA: 0x0003D5A8 File Offset: 0x0003B7A8
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.endColor = this.color;
				this.endColor.a = 0f;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x0003D5CD File Offset: 0x0003B7CD
		public float timeSinceSpawn
		{
			get
			{
				return Time.time - this.spawnTime;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000F97 RID: 3991 RVA: 0x0003D5DB File Offset: 0x0003B7DB
		// (set) Token: 0x06000F98 RID: 3992 RVA: 0x0003D5E8 File Offset: 0x0003B7E8
		private string text
		{
			get
			{
				return this.tmp.text;
			}
			set
			{
				this.tmp.text = value;
			}
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0003D5F8 File Offset: 0x0003B7F8
		public void SetupContent(string text, Sprite sprite = null)
		{
			this.text = text;
			if (sprite == null)
			{
				this.spriteRenderer.gameObject.SetActive(false);
				return;
			}
			this.spriteRenderer.gameObject.SetActive(true);
			this.spriteRenderer.sprite = sprite;
			this.spriteRenderer.transform.localScale = Vector3.one * (0.5f / (sprite.rect.width / sprite.pixelsPerUnit));
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0003D679 File Offset: 0x0003B879
		internal void SetColor(Color newColor)
		{
			this.Tmp.color = newColor;
			this.spriteRenderer.color = newColor;
		}

		// Token: 0x04000C94 RID: 3220
		[SerializeField]
		private SpriteRenderer spriteRenderer;

		// Token: 0x04000C95 RID: 3221
		private RectTransform _spriteRendererRectTransform_cache;

		// Token: 0x04000C96 RID: 3222
		[SerializeField]
		private TextMeshPro _tmp;

		// Token: 0x04000C97 RID: 3223
		public Vector3 velocity;

		// Token: 0x04000C98 RID: 3224
		public float size;

		// Token: 0x04000C99 RID: 3225
		private Color color;

		// Token: 0x04000C9A RID: 3226
		private Color endColor;

		// Token: 0x04000C9B RID: 3227
		public float spawnTime;
	}
}
