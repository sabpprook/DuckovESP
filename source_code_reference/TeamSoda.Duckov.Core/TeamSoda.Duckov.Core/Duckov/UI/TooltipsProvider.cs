using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Duckov.UI
{
	// Token: 0x0200037E RID: 894
	public class TooltipsProvider : MonoBehaviour, ITooltipsProvider, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06001EF6 RID: 7926 RVA: 0x0006C8BF File Offset: 0x0006AABF
		public string GetTooltipsText()
		{
			return this.text;
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x0006C8C7 File Offset: 0x0006AAC7
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (string.IsNullOrEmpty(this.text))
			{
				return;
			}
			Tooltips.NotifyEnterTooltipsProvider(this);
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x0006C8DD File Offset: 0x0006AADD
		public void OnPointerExit(PointerEventData eventData)
		{
			Tooltips.NotifyExitTooltipsProvider(this);
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x0006C8E5 File Offset: 0x0006AAE5
		private void OnDisable()
		{
			Tooltips.NotifyExitTooltipsProvider(this);
		}

		// Token: 0x04001533 RID: 5427
		public string text;
	}
}
