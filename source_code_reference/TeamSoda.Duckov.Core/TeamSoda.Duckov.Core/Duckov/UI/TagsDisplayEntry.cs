using System;
using Duckov.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x0200039D RID: 925
	public class TagsDisplayEntry : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ITooltipsProvider
	{
		// Token: 0x060020F4 RID: 8436 RVA: 0x00073061 File Offset: 0x00071261
		public string GetTooltipsText()
		{
			if (this.target == null)
			{
				return "";
			}
			return this.target.Description;
		}

		// Token: 0x060020F5 RID: 8437 RVA: 0x00073082 File Offset: 0x00071282
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.target == null)
			{
				return;
			}
			if (!this.target.ShowDescription)
			{
				return;
			}
			Tooltips.NotifyEnterTooltipsProvider(this);
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x000730A7 File Offset: 0x000712A7
		public void OnPointerExit(PointerEventData eventData)
		{
			Tooltips.NotifyExitTooltipsProvider(this);
		}

		// Token: 0x060020F7 RID: 8439 RVA: 0x000730AF File Offset: 0x000712AF
		private void OnDisable()
		{
			Tooltips.NotifyExitTooltipsProvider(this);
		}

		// Token: 0x060020F8 RID: 8440 RVA: 0x000730B7 File Offset: 0x000712B7
		public void Setup(Tag tag)
		{
			this.target = tag;
			this.background.color = tag.Color;
			this.text.text = tag.DisplayName;
		}

		// Token: 0x04001662 RID: 5730
		[SerializeField]
		private Image background;

		// Token: 0x04001663 RID: 5731
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001664 RID: 5732
		private Tag target;
	}
}
