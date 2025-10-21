using System;
using ItemStatsSystem;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class BulletTypeDisplay : MonoBehaviour
{
	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06000E82 RID: 3714 RVA: 0x0003A36E File Offset: 0x0003856E
	[LocalizationKey("Default")]
	private string NotAssignedTextKey
	{
		get
		{
			return "UI_Bullet_NotAssigned";
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0003A378 File Offset: 0x00038578
	internal void Setup(int targetBulletID)
	{
		if (targetBulletID < 0)
		{
			this.bulletDisplayName.text = this.NotAssignedTextKey.ToPlainText();
			return;
		}
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(targetBulletID);
		this.bulletDisplayName.text = metaData.DisplayName;
	}

	// Token: 0x04000C0A RID: 3082
	[SerializeField]
	private TextMeshProUGUI bulletDisplayName;
}
