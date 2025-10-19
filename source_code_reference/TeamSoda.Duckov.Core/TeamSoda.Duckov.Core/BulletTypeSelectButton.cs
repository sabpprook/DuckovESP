using System;
using ItemStatsSystem;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class BulletTypeSelectButton : MonoBehaviour
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000606 RID: 1542 RVA: 0x0001B1E1 File Offset: 0x000193E1
	public int BulletTypeID
	{
		get
		{
			return this.bulletTypeID;
		}
	}

	// Token: 0x06000607 RID: 1543 RVA: 0x0001B1E9 File Offset: 0x000193E9
	public void SetSelection(bool selected)
	{
		this.selectShadow.enabled = selected;
		this.indicator.SetActive(selected);
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0001B203 File Offset: 0x00019403
	public void Init(int id, int count)
	{
		this.bulletTypeID = id;
		this.bulletCount = count;
		this.SetSelection(false);
		this.RefreshContent();
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0001B220 File Offset: 0x00019420
	public void RefreshContent()
	{
		this.nameText.text = this.GetBulletName(this.bulletTypeID);
		this.countText.text = this.bulletCount.ToString();
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0001B250 File Offset: 0x00019450
	public string GetBulletName(int id)
	{
		if (id > 0)
		{
			return ItemAssetsCollection.GetMetaData(id).DisplayName;
		}
		return "UI_Bullet_NotAssigned".ToPlainText();
	}

	// Token: 0x0400059B RID: 1435
	private int bulletTypeID;

	// Token: 0x0400059C RID: 1436
	private int bulletCount;

	// Token: 0x0400059D RID: 1437
	public BulletTypeHUD bulletTypeHUD;

	// Token: 0x0400059E RID: 1438
	public TextMeshProUGUI nameText;

	// Token: 0x0400059F RID: 1439
	public TextMeshProUGUI countText;

	// Token: 0x040005A0 RID: 1440
	public TrueShadow selectShadow;

	// Token: 0x040005A1 RID: 1441
	public GameObject indicator;
}
