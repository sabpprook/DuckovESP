using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000121 RID: 289
public class QuestRequiredItem : MonoBehaviour
{
	// Token: 0x06000984 RID: 2436 RVA: 0x00029648 File Offset: 0x00027848
	public void Set(int itemTypeID, int count = 1)
	{
		if (itemTypeID <= 0 || count <= 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(itemTypeID);
		if (metaData.id == 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.icon.sprite = metaData.icon;
		this.text.text = string.Format("{0} x{1}", metaData.DisplayName, count);
		base.gameObject.SetActive(true);
	}

	// Token: 0x04000864 RID: 2148
	[SerializeField]
	private Image icon;

	// Token: 0x04000865 RID: 2149
	[SerializeField]
	private TextMeshProUGUI text;
}
