using System;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;
using UnityEngine.InputSystem;

// Token: 0x02000069 RID: 105
public class CheatingManager : MonoBehaviour
{
	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x0600040E RID: 1038 RVA: 0x00011D4C File Offset: 0x0000FF4C
	public static CheatingManager Instance
	{
		get
		{
			return CheatingManager._instance;
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x00011D53 File Offset: 0x0000FF53
	private void Awake()
	{
		CheatingManager._instance = this;
		CheatMode.Activate();
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00011D60 File Offset: 0x0000FF60
	private void Update()
	{
		if (!CheatMode.Active)
		{
			return;
		}
		if (!CharacterMainControl.Main)
		{
			return;
		}
		if (Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed && Keyboard.current.equalsKey.wasPressedThisFrame)
		{
			this.ToggleInvincible();
		}
		if (Keyboard.current != null && Keyboard.current.numpadMultiplyKey.wasPressedThisFrame)
		{
			this.typing = !this.typing;
			if (this.typing)
			{
				this.typingID = 0;
				this.LogCurrentTypingID();
			}
			else
			{
				this.LockItem();
			}
		}
		this.UpdateTyping();
		if (Keyboard.current != null && this.typing && Keyboard.current.backspaceKey.wasPressedThisFrame && this.typingID > 0)
		{
			this.typingID /= 10;
			this.LogCurrentTypingID();
		}
		if (Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed && Mouse.current.backButton.wasPressedThisFrame)
		{
			this.CheatMove();
		}
		if (Keyboard.current != null && Keyboard.current.leftAltKey.isPressed && Keyboard.current.sKey.wasPressedThisFrame)
		{
			SleepView.Instance.Open(null);
		}
		if (Keyboard.current != null && Keyboard.current.numpadPlusKey.wasPressedThisFrame)
		{
			if (this.typing)
			{
				this.LockItem();
				this.typing = false;
			}
			this.CreateItem(this.lockedItem, 1);
		}
		if (Keyboard.current != null && Keyboard.current.numpadMinusKey.wasPressedThisFrame)
		{
			int displayingItemID = ItemHoveringUI.DisplayingItemID;
			if (displayingItemID > 0)
			{
				this.SetTypedItem(displayingItemID);
				this.CreateItem(this.lockedItem, 1);
			}
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00011F0C File Offset: 0x0001010C
	private void UpdateTyping()
	{
		if (Keyboard.current != null && Keyboard.current.numpad0Key.wasPressedThisFrame)
		{
			this.TypeOne(0);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad1Key.wasPressedThisFrame)
		{
			this.TypeOne(1);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad2Key.wasPressedThisFrame)
		{
			this.TypeOne(2);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad3Key.wasPressedThisFrame)
		{
			this.TypeOne(3);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad4Key.wasPressedThisFrame)
		{
			this.TypeOne(4);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad5Key.wasPressedThisFrame)
		{
			this.TypeOne(5);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad6Key.wasPressedThisFrame)
		{
			this.TypeOne(6);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad7Key.wasPressedThisFrame)
		{
			this.TypeOne(7);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad8Key.wasPressedThisFrame)
		{
			this.TypeOne(8);
			return;
		}
		if (Keyboard.current != null && Keyboard.current.numpad9Key.wasPressedThisFrame)
		{
			this.TypeOne(9);
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0001205C File Offset: 0x0001025C
	private void LogCurrentTypingID()
	{
		if (this.typingID <= 0)
		{
			CharacterMainControl.Main.PopText("_", 999f);
			return;
		}
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(this.typingID);
		if (metaData.id > 0)
		{
			CharacterMainControl.Main.PopText(string.Format(" {0}_  ({1})", this.typingID, metaData.DisplayName), 999f);
			return;
		}
		CharacterMainControl.Main.PopText(string.Format("{0}_", this.typingID), 999f);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x000120EC File Offset: 0x000102EC
	private void TypeOne(int i)
	{
		this.typingID = this.typingID * 10 + i;
		this.LogCurrentTypingID();
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00012105 File Offset: 0x00010305
	private void SetTypedItem(int id)
	{
		this.typingID = id;
		this.LockItem();
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00012114 File Offset: 0x00010314
	private void LockItem()
	{
		this.typing = false;
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(this.typingID);
		if (metaData.id <= 0)
		{
			CharacterMainControl.Main.PopText("没有这个物品。", 999f);
			return;
		}
		this.lockedItem = this.typingID;
		CharacterMainControl.Main.PopText(metaData.DisplayName + " 已选定", 999f);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00012180 File Offset: 0x00010380
	public void CreateItem(int id, int quantity = 1)
	{
		if (ItemAssetsCollection.GetMetaData(id).id <= 0)
		{
			CharacterMainControl.Main.PopText("没有这个物品。", 999f);
			return;
		}
		this.CreateItemAsync(id, quantity).Forget();
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x000121C0 File Offset: 0x000103C0
	private async UniTaskVoid CreateItemAsync(int id, int quantity = 1)
	{
		while (quantity > 0)
		{
			Item item = await ItemAssetsCollection.InstantiateAsync(id);
			int maxStackCount = item.MaxStackCount;
			if (quantity > maxStackCount)
			{
				item.StackCount = maxStackCount;
				quantity -= maxStackCount;
			}
			else
			{
				item.StackCount = quantity;
				quantity = 0;
			}
			ItemUtilities.SendToPlayer(item, false, true);
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001220B File Offset: 0x0001040B
	private void ToggleTypeing()
	{
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x00012210 File Offset: 0x00010410
	public void ToggleInvincible()
	{
		this.isInvincible = !this.isInvincible;
		CharacterMainControl.Main.Health.SetInvincible(this.isInvincible);
		CharacterMainControl.Main.PopText(this.isInvincible ? "我无敌了" : "我不无敌了", -1f);
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00012264 File Offset: 0x00010464
	public void CheatMove()
	{
		Vector2 vector = Mouse.current.position.ReadValue();
		Ray ray = LevelManager.Instance.GameCamera.renderCamera.ScreenPointToRay(vector);
		LayerMask layerMask = GameplayDataSettings.Layers.wallLayerMask | GameplayDataSettings.Layers.groundLayerMask;
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 100f, layerMask, QueryTriggerInteraction.Ignore))
		{
			CharacterMainControl.Main.SetPosition(raycastHit.point);
		}
	}

	// Token: 0x0400030C RID: 780
	private static CheatingManager _instance;

	// Token: 0x0400030D RID: 781
	private bool isInvincible;

	// Token: 0x0400030E RID: 782
	private bool typing;

	// Token: 0x0400030F RID: 783
	private int typingID;

	// Token: 0x04000310 RID: 784
	private int lockedItem;
}
