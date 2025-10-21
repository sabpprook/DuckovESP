using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003C2 RID: 962
	[RequireComponent(typeof(ScrollRect))]
	[ExecuteInEditMode]
	public class ScrollViewMaxHeight : UIBehaviour, ILayoutElement
	{
		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x060022FB RID: 8955 RVA: 0x0007A79C File Offset: 0x0007899C
		public float preferredHeight
		{
			get
			{
				float y = this.scrollRect.content.sizeDelta.y;
				float num = this.maxHeight;
				if (this.useTargetParentSize)
				{
					float num2 = 0f;
					foreach (RectTransform rectTransform in this.siblings)
					{
						num2 += rectTransform.rect.height;
					}
					num = this.targetParentHeight - num2 - this.parentLayoutMargin;
				}
				if (y > num)
				{
					return num;
				}
				return y;
			}
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x0007A840 File Offset: 0x00078A40
		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x0007A842 File Offset: 0x00078A42
		public virtual void CalculateLayoutInputVertical()
		{
		}

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060022FE RID: 8958 RVA: 0x0007A844 File Offset: 0x00078A44
		public virtual float minWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x060022FF RID: 8959 RVA: 0x0007A84B File Offset: 0x00078A4B
		public virtual float minHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06002300 RID: 8960 RVA: 0x0007A852 File Offset: 0x00078A52
		public virtual float preferredWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06002301 RID: 8961 RVA: 0x0007A859 File Offset: 0x00078A59
		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06002302 RID: 8962 RVA: 0x0007A860 File Offset: 0x00078A60
		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06002303 RID: 8963 RVA: 0x0007A867 File Offset: 0x00078A67
		public virtual int layoutPriority
		{
			get
			{
				return this.m_layoutPriority;
			}
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x0007A86F File Offset: 0x00078A6F
		private void OnContentRectChange(RectTransform rectTransform)
		{
			this.SetDirty();
		}

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x06002305 RID: 8965 RVA: 0x0007A877 File Offset: 0x00078A77
		private RectTransform rectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.transform as RectTransform;
				}
				return this._rectTransform;
			}
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x0007A8A0 File Offset: 0x00078AA0
		protected override void OnEnable()
		{
			if (this.scrollRect == null)
			{
				this.scrollRect = base.GetComponent<ScrollRect>();
			}
			if (this.contentRectChangeEventEmitter == null)
			{
				this.contentRectChangeEventEmitter = this.scrollRect.content.GetComponent<RectTransformChangeEventEmitter>();
			}
			if (this.contentRectChangeEventEmitter == null)
			{
				this.contentRectChangeEventEmitter = this.scrollRect.content.gameObject.AddComponent<RectTransformChangeEventEmitter>();
			}
			base.OnEnable();
			this.contentRectChangeEventEmitter.OnRectTransformChange += this.OnContentRectChange;
			this.SetDirty();
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x0007A937 File Offset: 0x00078B37
		protected override void OnDisable()
		{
			this.contentRectChangeEventEmitter.OnRectTransformChange -= this.OnContentRectChange;
			this.SetDirty();
			base.OnDisable();
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x0007A95C File Offset: 0x00078B5C
		private void Update()
		{
			if (this.preferredHeight != this.rectTransform.rect.height)
			{
				this.SetDirty();
			}
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x0007A98A File Offset: 0x00078B8A
		protected void SetDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
		}

		// Token: 0x040017CD RID: 6093
		[SerializeField]
		private ScrollRect scrollRect;

		// Token: 0x040017CE RID: 6094
		[SerializeField]
		private RectTransformChangeEventEmitter contentRectChangeEventEmitter;

		// Token: 0x040017CF RID: 6095
		[SerializeField]
		private int m_layoutPriority = 1;

		// Token: 0x040017D0 RID: 6096
		[SerializeField]
		private bool useTargetParentSize;

		// Token: 0x040017D1 RID: 6097
		[SerializeField]
		private float targetParentHeight = 935f;

		// Token: 0x040017D2 RID: 6098
		[SerializeField]
		private List<RectTransform> siblings = new List<RectTransform>();

		// Token: 0x040017D3 RID: 6099
		[SerializeField]
		private float parentLayoutMargin = 16f;

		// Token: 0x040017D4 RID: 6100
		[SerializeField]
		private float maxHeight = 100f;

		// Token: 0x040017D5 RID: 6101
		private RectTransform _rectTransform;
	}
}
