using System;
using Duckov.Bitcoins;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200019F RID: 415
public class BitcoinMinerView : View
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06000C37 RID: 3127 RVA: 0x000337DC File Offset: 0x000319DC
	public static BitcoinMinerView Instance
	{
		get
		{
			return View.GetViewInstance<BitcoinMinerView>();
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06000C38 RID: 3128 RVA: 0x000337E3 File Offset: 0x000319E3
	// (set) Token: 0x06000C39 RID: 3129 RVA: 0x000337EA File Offset: 0x000319EA
	[LocalizationKey("Default")]
	private string ActiveCommentKey
	{
		get
		{
			return "UI_BitcoinMiner_Active";
		}
		set
		{
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06000C3A RID: 3130 RVA: 0x000337EC File Offset: 0x000319EC
	// (set) Token: 0x06000C3B RID: 3131 RVA: 0x000337F3 File Offset: 0x000319F3
	[LocalizationKey("Default")]
	private string StoppedCommentKey
	{
		get
		{
			return "UI_BitcoinMiner_Stopped";
		}
		set
		{
		}
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x000337F8 File Offset: 0x000319F8
	protected override void Awake()
	{
		base.Awake();
		this.minerInventoryDisplay.onDisplayDoubleClicked += this.OnMinerInventoryEntryDoubleClicked;
		this.inventoryDisplay.onDisplayDoubleClicked += this.OnPlayerItemsDoubleClicked;
		this.storageDisplay.onDisplayDoubleClicked += this.OnPlayerItemsDoubleClicked;
		this.minerSlotsDisplay.onElementDoubleClicked += this.OnMinerSlotEntryDoubleClicked;
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00033868 File Offset: 0x00031A68
	private void OnMinerSlotEntryDoubleClicked(ItemSlotCollectionDisplay display1, SlotDisplay slotDisplay)
	{
		Slot target = slotDisplay.Target;
		if (target == null)
		{
			return;
		}
		Item content = target.Content;
		if (content == null)
		{
			return;
		}
		ItemUtilities.SendToPlayer(content, false, PlayerStorage.Instance != null);
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x000338A4 File Offset: 0x00031AA4
	private void OnPlayerItemsDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
	{
		Item content = entry.Content;
		if (content == null)
		{
			return;
		}
		Item item = BitcoinMiner.Instance.Item;
		if (item == null)
		{
			return;
		}
		item.TryPlug(content, true, content.InInventory, 0);
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x000338E8 File Offset: 0x00031AE8
	private void OnMinerInventoryEntryDoubleClicked(InventoryDisplay display, InventoryEntry entry, PointerEventData data)
	{
		Item content = entry.Content;
		if (content == null)
		{
			return;
		}
		if (data.button == PointerEventData.InputButton.Left)
		{
			ItemUtilities.SendToPlayer(content, false, true);
		}
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00033916 File Offset: 0x00031B16
	public static void Show()
	{
		if (BitcoinMinerView.Instance == null)
		{
			return;
		}
		if (BitcoinMiner.Instance == null)
		{
			return;
		}
		BitcoinMinerView.Instance.Open(null);
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00033940 File Offset: 0x00031B40
	protected override void OnOpen()
	{
		base.OnOpen();
		CharacterMainControl main = CharacterMainControl.Main;
		if (!(main == null))
		{
			Item characterItem = main.CharacterItem;
			if (!(characterItem == null))
			{
				BitcoinMiner instance = BitcoinMiner.Instance;
				if (!instance.Loading)
				{
					Item item = instance.Item;
					if (!(item == null))
					{
						this.inventoryDisplay.Setup(characterItem.Inventory, null, null, false, null);
						if (PlayerStorage.Inventory != null)
						{
							this.storageDisplay.gameObject.SetActive(true);
							this.storageDisplay.Setup(PlayerStorage.Inventory, null, null, false, null);
						}
						else
						{
							this.storageDisplay.gameObject.SetActive(false);
						}
						this.minerSlotsDisplay.Setup(item, false);
						this.minerInventoryDisplay.Setup(item.Inventory, null, null, false, null);
						this.fadeGroup.Show();
						return;
					}
				}
			}
		}
		Debug.Log("Failed");
		base.Close();
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x00033A34 File Offset: 0x00031C34
	protected override void OnClose()
	{
		base.OnClose();
		this.fadeGroup.Hide();
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x00033A47 File Offset: 0x00031C47
	private void FixedUpdate()
	{
		this.RefreshStatus();
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x00033A50 File Offset: 0x00031C50
	private void RefreshStatus()
	{
		if (BitcoinMiner.Instance.WorkPerSecond > 0.0)
		{
			TimeSpan remainingTime = BitcoinMiner.Instance.RemainingTime;
			TimeSpan timePerCoin = BitcoinMiner.Instance.TimePerCoin;
			this.remainingTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt((float)remainingTime.TotalHours), remainingTime.Minutes, remainingTime.Seconds);
			this.timeEachCoinText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt((float)timePerCoin.TotalHours), timePerCoin.Minutes, timePerCoin.Seconds);
			this.performanceText.text = string.Format("{0:0.#}", BitcoinMiner.Instance.Performance);
			this.commentText.text = this.ActiveCommentKey.ToPlainText();
		}
		else
		{
			this.remainingTimeText.text = "--:--:--";
			this.timeEachCoinText.text = "--:--:--";
			this.commentText.text = this.StoppedCommentKey.ToPlainText();
			this.performanceText.text = string.Format("{0:0.#}", BitcoinMiner.Instance.Performance);
		}
		this.fill.fillAmount = BitcoinMiner.Instance.NormalizedProgress;
	}

	// Token: 0x04000A95 RID: 2709
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000A96 RID: 2710
	[SerializeField]
	private InventoryDisplay inventoryDisplay;

	// Token: 0x04000A97 RID: 2711
	[SerializeField]
	private InventoryDisplay storageDisplay;

	// Token: 0x04000A98 RID: 2712
	[SerializeField]
	private ItemSlotCollectionDisplay minerSlotsDisplay;

	// Token: 0x04000A99 RID: 2713
	[SerializeField]
	private InventoryDisplay minerInventoryDisplay;

	// Token: 0x04000A9A RID: 2714
	[SerializeField]
	private TextMeshProUGUI commentText;

	// Token: 0x04000A9B RID: 2715
	[SerializeField]
	private TextMeshProUGUI remainingTimeText;

	// Token: 0x04000A9C RID: 2716
	[SerializeField]
	private TextMeshProUGUI timeEachCoinText;

	// Token: 0x04000A9D RID: 2717
	[SerializeField]
	private TextMeshProUGUI performanceText;

	// Token: 0x04000A9E RID: 2718
	[SerializeField]
	private Image fill;
}
