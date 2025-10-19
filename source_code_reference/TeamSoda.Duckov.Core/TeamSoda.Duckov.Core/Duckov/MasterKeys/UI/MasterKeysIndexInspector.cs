using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Duckov.MasterKeys.UI
{
	// Token: 0x020002DD RID: 733
	public class MasterKeysIndexInspector : MonoBehaviour
	{
		// Token: 0x06001768 RID: 5992 RVA: 0x00055E45 File Offset: 0x00054045
		internal void Setup(MasterKeysIndexEntry target)
		{
			if (target == null)
			{
				this.SetupEmpty();
				return;
			}
			this.SetupNormal(target);
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x00055E60 File Offset: 0x00054060
		private void SetupNormal(MasterKeysIndexEntry target)
		{
			this.targetItemID = target.ItemID;
			this.placeHolder.SetActive(false);
			this.content.SetActive(true);
			this.nameText.text = target.DisplayName;
			this.descriptionText.text = target.Description;
			this.icon.sprite = target.Icon;
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x00055EC4 File Offset: 0x000540C4
		private void SetupEmpty()
		{
			this.content.gameObject.SetActive(false);
			this.placeHolder.SetActive(true);
		}

		// Token: 0x0400111E RID: 4382
		[SerializeField]
		private int targetItemID;

		// Token: 0x0400111F RID: 4383
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04001120 RID: 4384
		[SerializeField]
		private TextMeshProUGUI descriptionText;

		// Token: 0x04001121 RID: 4385
		[SerializeField]
		private Image icon;

		// Token: 0x04001122 RID: 4386
		[SerializeField]
		private GameObject content;

		// Token: 0x04001123 RID: 4387
		[SerializeField]
		private GameObject placeHolder;
	}
}
