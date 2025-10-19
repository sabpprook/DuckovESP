using System;
using TMPro;
using UnityEngine;

namespace Duckov.Endowment.UI
{
	// Token: 0x020002F3 RID: 755
	public class EndowmentDisplay : MonoBehaviour
	{
		// Token: 0x06001882 RID: 6274 RVA: 0x00059474 File Offset: 0x00057674
		private void Refresh()
		{
			EndowmentEntry endowmentEntry = EndowmentManager.Current;
			if (endowmentEntry == null)
			{
				this.displayNameText.text = "?";
				this.descriptionsText.text = "?";
				return;
			}
			this.displayNameText.text = endowmentEntry.DisplayName;
			this.descriptionsText.text = endowmentEntry.DescriptionAndEffects;
		}

		// Token: 0x06001883 RID: 6275 RVA: 0x000594D3 File Offset: 0x000576D3
		private void OnEnable()
		{
			this.Refresh();
		}

		// Token: 0x040011DD RID: 4573
		[SerializeField]
		private TextMeshProUGUI displayNameText;

		// Token: 0x040011DE RID: 4574
		[SerializeField]
		private TextMeshProUGUI descriptionsText;
	}
}
