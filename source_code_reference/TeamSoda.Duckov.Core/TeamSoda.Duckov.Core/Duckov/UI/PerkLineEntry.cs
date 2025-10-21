using System;
using Duckov.PerkTrees;
using TMPro;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003B8 RID: 952
	public class PerkLineEntry : MonoBehaviour
	{
		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06002290 RID: 8848 RVA: 0x00078EAF File Offset: 0x000770AF
		public RectTransform RectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.GetComponent<RectTransform>();
				}
				return this._rectTransform;
			}
		}

		// Token: 0x06002291 RID: 8849 RVA: 0x00078ED1 File Offset: 0x000770D1
		internal void Setup(PerkTreeView perkTreeView, PerkLevelLineNode cur)
		{
			this.target = cur;
			this.label.text = this.target.DisplayName;
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x00078EF0 File Offset: 0x000770F0
		internal Vector2 GetLayoutPosition()
		{
			if (this.target == null)
			{
				return Vector2.zero;
			}
			return this.target.cachedPosition;
		}

		// Token: 0x0400178F RID: 6031
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04001790 RID: 6032
		private RectTransform _rectTransform;

		// Token: 0x04001791 RID: 6033
		private PerkLevelLineNode target;
	}
}
