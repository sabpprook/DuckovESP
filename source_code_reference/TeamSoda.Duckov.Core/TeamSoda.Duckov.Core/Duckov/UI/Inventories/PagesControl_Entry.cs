using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI.Inventories
{
	// Token: 0x020003C7 RID: 967
	public class PagesControl_Entry : MonoBehaviour
	{
		// Token: 0x06002329 RID: 9001 RVA: 0x0007B299 File Offset: 0x00079499
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
		}

		// Token: 0x0600232A RID: 9002 RVA: 0x0007B2B7 File Offset: 0x000794B7
		private void OnButtonClicked()
		{
			this.master.NotifySelect(this.index);
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x0007B2CC File Offset: 0x000794CC
		internal void Setup(PagesControl master, int i, bool selected)
		{
			this.master = master;
			this.index = i;
			this.selected = selected;
			this.text.text = string.Format("{0}", this.index);
			this.selectedIndicator.SetActive(this.selected);
		}

		// Token: 0x040017ED RID: 6125
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040017EE RID: 6126
		[SerializeField]
		private GameObject selectedIndicator;

		// Token: 0x040017EF RID: 6127
		[SerializeField]
		private Button button;

		// Token: 0x040017F0 RID: 6128
		private PagesControl master;

		// Token: 0x040017F1 RID: 6129
		private int index;

		// Token: 0x040017F2 RID: 6130
		private bool selected;
	}
}
