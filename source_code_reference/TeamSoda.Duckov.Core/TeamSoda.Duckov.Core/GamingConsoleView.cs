using System;
using Duckov.MiniGames;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class GamingConsoleView : View
{
	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06000D41 RID: 3393 RVA: 0x00036F5A File Offset: 0x0003515A
	public static GamingConsoleView Instance
	{
		get
		{
			return View.GetViewInstance<GamingConsoleView>();
		}
	}

	// Token: 0x06000D42 RID: 3394 RVA: 0x00036F64 File Offset: 0x00035164
	protected override void OnOpen()
	{
		base.OnOpen();
		this.fadeGroup.Show();
		this.Setup(this.target);
		if (CharacterMainControl.Main)
		{
			this.characterInventory.Setup(CharacterMainControl.Main.CharacterItem.Inventory, null, null, false, null);
		}
		if (PetProxy.PetInventory)
		{
			this.petInventory.Setup(PetProxy.PetInventory, null, null, false, null);
		}
		if (PlayerStorage.Inventory)
		{
			this.storageInventory.Setup(PlayerStorage.Inventory, null, null, false, null);
		}
		this.RefreshConsole();
	}

	// Token: 0x06000D43 RID: 3395 RVA: 0x00036FFE File Offset: 0x000351FE
	protected override void OnClose()
	{
		base.OnClose();
		this.fadeGroup.Hide();
	}

	// Token: 0x06000D44 RID: 3396 RVA: 0x00037014 File Offset: 0x00035214
	private void SetTarget(GamingConsole target)
	{
		if (this.target != null)
		{
			this.target.onContentChanged -= this.OnTargetContentChanged;
		}
		if (target != null)
		{
			this.target = target;
			return;
		}
		this.target = global::UnityEngine.Object.FindObjectOfType<GamingConsole>();
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x00037064 File Offset: 0x00035264
	private void Setup(GamingConsole target)
	{
		this.SetTarget(target);
		if (this.target == null)
		{
			return;
		}
		this.target.onContentChanged += this.OnTargetContentChanged;
		this.consoleSlotDisplay.Setup(this.target.ConsoleSlot);
		this.monitorSlotDisplay.Setup(this.target.MonitorSlot);
		this.RefreshConsole();
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x000370D0 File Offset: 0x000352D0
	private void OnTargetContentChanged(GamingConsole console)
	{
		this.RefreshConsole();
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x000370D8 File Offset: 0x000352D8
	private void RefreshConsole()
	{
		if (this.isBeingDestroyed)
		{
			return;
		}
		Slot consoleSlot = this.target.ConsoleSlot;
		if (consoleSlot == null)
		{
			return;
		}
		Item content = consoleSlot.Content;
		this.consoleSlotCollectionDisplay.gameObject.SetActive(content);
		if (content)
		{
			this.consoleSlotCollectionDisplay.Setup(content, false);
		}
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x00037130 File Offset: 0x00035330
	internal static void Show(GamingConsole console)
	{
		GamingConsoleView.Instance.target = console;
		GamingConsoleView.Instance.Open(null);
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00037148 File Offset: 0x00035348
	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.isBeingDestroyed = true;
	}

	// Token: 0x04000B4F RID: 2895
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000B50 RID: 2896
	[SerializeField]
	private InventoryDisplay characterInventory;

	// Token: 0x04000B51 RID: 2897
	[SerializeField]
	private InventoryDisplay petInventory;

	// Token: 0x04000B52 RID: 2898
	[SerializeField]
	private InventoryDisplay storageInventory;

	// Token: 0x04000B53 RID: 2899
	[SerializeField]
	private SlotDisplay monitorSlotDisplay;

	// Token: 0x04000B54 RID: 2900
	[SerializeField]
	private SlotDisplay consoleSlotDisplay;

	// Token: 0x04000B55 RID: 2901
	[SerializeField]
	private ItemSlotCollectionDisplay consoleSlotCollectionDisplay;

	// Token: 0x04000B56 RID: 2902
	private GamingConsole target;

	// Token: 0x04000B57 RID: 2903
	private bool isBeingDestroyed;
}
