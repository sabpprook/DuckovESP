using System;
using ItemStatsSystem;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000159 RID: 345
public class ItemAmountDisplay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IItemMetaDataProvider
{
	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06000A92 RID: 2706 RVA: 0x0002E178 File Offset: 0x0002C378
	// (remove) Token: 0x06000A93 RID: 2707 RVA: 0x0002E1AC File Offset: 0x0002C3AC
	public static event Action<ItemAmountDisplay> OnMouseEnter;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x06000A94 RID: 2708 RVA: 0x0002E1E0 File Offset: 0x0002C3E0
	// (remove) Token: 0x06000A95 RID: 2709 RVA: 0x0002E214 File Offset: 0x0002C414
	public static event Action<ItemAmountDisplay> OnMouseExit;

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06000A96 RID: 2710 RVA: 0x0002E247 File Offset: 0x0002C447
	public int TypeID
	{
		get
		{
			return this.typeID;
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06000A97 RID: 2711 RVA: 0x0002E24F File Offset: 0x0002C44F
	public ItemMetaData MetaData
	{
		get
		{
			return this.metaData;
		}
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x0002E257 File Offset: 0x0002C457
	public ItemMetaData GetMetaData()
	{
		return this.metaData;
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x0002E25F File Offset: 0x0002C45F
	private void Awake()
	{
		ItemUtilities.OnPlayerItemOperation += this.Refresh;
		LevelManager.OnLevelInitialized += this.Refresh;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0002E283 File Offset: 0x0002C483
	private void OnDestroy()
	{
		ItemUtilities.OnPlayerItemOperation -= this.Refresh;
		LevelManager.OnLevelInitialized -= this.Refresh;
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x0002E2A7 File Offset: 0x0002C4A7
	public void Setup(int itemTypeID, long amount)
	{
		this.typeID = itemTypeID;
		this.amount = amount;
		this.Refresh();
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x0002E2C0 File Offset: 0x0002C4C0
	private void Refresh()
	{
		int itemCount = ItemUtilities.GetItemCount(this.typeID);
		this.metaData = ItemAssetsCollection.GetMetaData(this.typeID);
		this.icon.sprite = this.metaData.icon;
		this.amountText.text = this.amountFormat.Format(new
		{
			amount = this.amount,
			possess = itemCount
		});
		bool flag = (long)itemCount >= this.amount;
		this.background.color = (flag ? this.enoughColor : this.normalColor);
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x0002E34C File Offset: 0x0002C54C
	public void OnPointerEnter(PointerEventData eventData)
	{
		Action<ItemAmountDisplay> onMouseEnter = ItemAmountDisplay.OnMouseEnter;
		if (onMouseEnter == null)
		{
			return;
		}
		onMouseEnter(this);
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x0002E35E File Offset: 0x0002C55E
	public void OnPointerExit(PointerEventData eventData)
	{
		Action<ItemAmountDisplay> onMouseExit = ItemAmountDisplay.OnMouseExit;
		if (onMouseExit == null)
		{
			return;
		}
		onMouseExit(this);
	}

	// Token: 0x0400093D RID: 2365
	[SerializeField]
	private Image background;

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	private Image icon;

	// Token: 0x0400093F RID: 2367
	[SerializeField]
	private TextMeshProUGUI amountText;

	// Token: 0x04000940 RID: 2368
	[SerializeField]
	private string amountFormat = "( {possess} / {amount} )";

	// Token: 0x04000941 RID: 2369
	[SerializeField]
	private Color normalColor;

	// Token: 0x04000942 RID: 2370
	[SerializeField]
	private Color enoughColor;

	// Token: 0x04000943 RID: 2371
	private int typeID;

	// Token: 0x04000944 RID: 2372
	private long amount;

	// Token: 0x04000945 RID: 2373
	private ItemMetaData metaData;
}
