using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003C5 RID: 965
	public class KontextMenuEntry : MonoBehaviour, IPoolable, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x0600231B RID: 8987 RVA: 0x0007AF77 File Offset: 0x00079177
		public void NotifyPooled()
		{
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x0007AF79 File Offset: 0x00079179
		public void NotifyReleased()
		{
			this.target = null;
		}

		// Token: 0x0600231D RID: 8989 RVA: 0x0007AF82 File Offset: 0x00079182
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.menu != null)
			{
				this.menu.InstanceHide();
			}
			if (this.target != null)
			{
				Action action = this.target.action;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x0600231E RID: 8990 RVA: 0x0007AFBC File Offset: 0x000791BC
		public void Setup(KontextMenu menu, int index, KontextMenuDataEntry data)
		{
			this.menu = menu;
			this.target = data;
			if (this.icon)
			{
				if (data.icon)
				{
					this.icon.sprite = data.icon;
					this.icon.gameObject.SetActive(true);
				}
				else
				{
					this.icon.gameObject.SetActive(false);
				}
			}
			if (this.text)
			{
				if (!string.IsNullOrEmpty(this.target.text))
				{
					this.text.text = this.target.text;
					this.text.gameObject.SetActive(true);
				}
				else
				{
					this.text.gameObject.SetActive(false);
				}
			}
			foreach (FadeElement fadeElement in this.fadeInElements)
			{
				fadeElement.SkipHide();
				fadeElement.Show(this.delayByIndex * (float)index).Forget();
			}
		}

		// Token: 0x040017E3 RID: 6115
		[SerializeField]
		private Image icon;

		// Token: 0x040017E4 RID: 6116
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040017E5 RID: 6117
		[SerializeField]
		private float delayByIndex = 0.1f;

		// Token: 0x040017E6 RID: 6118
		[SerializeField]
		private List<FadeElement> fadeInElements;

		// Token: 0x040017E7 RID: 6119
		private KontextMenu menu;

		// Token: 0x040017E8 RID: 6120
		private KontextMenuDataEntry target;
	}
}
