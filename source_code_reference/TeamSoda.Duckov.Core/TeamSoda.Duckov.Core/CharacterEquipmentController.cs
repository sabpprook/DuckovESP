using System;
using System.Linq;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class CharacterEquipmentController : MonoBehaviour
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x06000280 RID: 640 RVA: 0x0000B3D4 File Offset: 0x000095D4
	// (remove) Token: 0x06000281 RID: 641 RVA: 0x0000B40C File Offset: 0x0000960C
	public event Action<Slot> OnHelmatSlotContentChanged;

	// Token: 0x14000009 RID: 9
	// (add) Token: 0x06000282 RID: 642 RVA: 0x0000B444 File Offset: 0x00009644
	// (remove) Token: 0x06000283 RID: 643 RVA: 0x0000B47C File Offset: 0x0000967C
	public event Action<Slot> OnFaceMaskSlotContentChanged;

	// Token: 0x06000284 RID: 644 RVA: 0x0000B4B4 File Offset: 0x000096B4
	public void SetItem(Item _item)
	{
		this.characterItem = _item;
		this.armorSlot = this.characterItem.Slots.GetSlot(CharacterEquipmentController.armorHash);
		this.helmatSlot = this.characterItem.Slots.GetSlot(CharacterEquipmentController.helmatHash);
		this.faceMaskSlot = this.characterItem.Slots.GetSlot(CharacterEquipmentController.faceMaskHash);
		this.backpackSlot = this.characterItem.Slots.GetSlot(CharacterEquipmentController.backpackHash);
		this.headsetSlot = this.characterItem.Slots.GetSlot(CharacterEquipmentController.headsetHash);
		this.armorSlot.onSlotContentChanged += this.ChangeArmorModel;
		this.helmatSlot.onSlotContentChanged += this.ChangeHelmatModel;
		this.faceMaskSlot.onSlotContentChanged += this.ChangeFaceMaskModel;
		this.backpackSlot.onSlotContentChanged += this.ChangeBackpackModel;
		this.headsetSlot.onSlotContentChanged += this.ChangeHeadsetModel;
		Slot slot = this.armorSlot;
		if (((slot != null) ? slot.Content : null) != null)
		{
			this.ChangeArmorModel(this.armorSlot);
		}
		Slot slot2 = this.helmatSlot;
		if (((slot2 != null) ? slot2.Content : null) != null)
		{
			this.ChangeHelmatModel(this.helmatSlot);
		}
		Slot slot3 = this.faceMaskSlot;
		if (((slot3 != null) ? slot3.Content : null) != null)
		{
			this.ChangeFaceMaskModel(this.faceMaskSlot);
		}
		Slot slot4 = this.backpackSlot;
		if (((slot4 != null) ? slot4.Content : null) != null)
		{
			this.ChangeBackpackModel(this.backpackSlot);
		}
		Slot slot5 = this.headsetSlot;
		if (((slot5 != null) ? slot5.Content : null) != null)
		{
			this.ChangeHeadsetModel(this.headsetSlot);
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0000B680 File Offset: 0x00009880
	private void OnDestroy()
	{
		if (this.armorSlot != null)
		{
			this.armorSlot.onSlotContentChanged -= this.ChangeArmorModel;
		}
		if (this.helmatSlot != null)
		{
			this.helmatSlot.onSlotContentChanged -= this.ChangeHelmatModel;
		}
		if (this.backpackSlot != null)
		{
			this.backpackSlot.onSlotContentChanged -= this.ChangeBackpackModel;
		}
		if (this.faceMaskSlot != null)
		{
			this.faceMaskSlot.onSlotContentChanged -= this.ChangeFaceMaskModel;
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x0000B70C File Offset: 0x0000990C
	private void ChangeArmorModel(Slot slot)
	{
		if (this.characterMainControl.characterModel == null)
		{
			return;
		}
		Transform armorSocket = this.characterMainControl.characterModel.ArmorSocket;
		this.ChangeEquipmentModel(slot, armorSocket);
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000B748 File Offset: 0x00009948
	private void ChangeHelmatModel(Slot slot)
	{
		Action<Slot> onHelmatSlotContentChanged = this.OnHelmatSlotContentChanged;
		if (onHelmatSlotContentChanged != null)
		{
			onHelmatSlotContentChanged(slot);
		}
		if (this.characterMainControl.characterModel == null)
		{
			return;
		}
		Transform helmatSocket = this.characterMainControl.characterModel.HelmatSocket;
		this.ChangeEquipmentModel(slot, helmatSocket);
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000B794 File Offset: 0x00009994
	private void ChangeHeadsetModel(Slot slot)
	{
		if (this.characterMainControl.characterModel == null)
		{
			return;
		}
		Transform helmatSocket = this.characterMainControl.characterModel.HelmatSocket;
		this.ChangeEquipmentModel(slot, helmatSocket);
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000B7D0 File Offset: 0x000099D0
	private void ChangeBackpackModel(Slot slot)
	{
		if (this.characterMainControl.characterModel == null)
		{
			return;
		}
		Transform backpackSocket = this.characterMainControl.characterModel.BackpackSocket;
		this.ChangeEquipmentModel(slot, backpackSocket);
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0000B80C File Offset: 0x00009A0C
	private void ChangeFaceMaskModel(Slot slot)
	{
		Action<Slot> onFaceMaskSlotContentChanged = this.OnFaceMaskSlotContentChanged;
		if (onFaceMaskSlotContentChanged != null)
		{
			onFaceMaskSlotContentChanged(slot);
		}
		if (this.characterMainControl.characterModel == null)
		{
			return;
		}
		Transform faceMaskSocket = this.characterMainControl.characterModel.FaceMaskSocket;
		this.ChangeEquipmentModel(slot, faceMaskSocket);
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000B858 File Offset: 0x00009A58
	private void ChangeEquipmentModel(Slot slot, Transform socket)
	{
		if (slot == null)
		{
			return;
		}
		if (slot.Content == null)
		{
			return;
		}
		ItemAgent itemAgent = slot.Content.AgentUtilities.CreateAgent(CharacterEquipmentController.equipmentModelHash, ItemAgent.AgentTypes.equipment);
		if (itemAgent == null)
		{
			Debug.LogError("生成的装备Item没有装备agent，Item名称：" + slot.Content.gameObject.name);
		}
		if (itemAgent != null)
		{
			itemAgent.transform.SetParent(socket, false);
			itemAgent.transform.localRotation = Quaternion.identity;
			itemAgent.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000B8F0 File Offset: 0x00009AF0
	private bool IsSlotRequireTag(Slot slot, Tag tag)
	{
		return slot.requireTags.Any((Tag e) => e.Hash == tag.Hash);
	}

	// Token: 0x04000205 RID: 517
	[SerializeField]
	private CharacterMainControl characterMainControl;

	// Token: 0x04000206 RID: 518
	private Item characterItem;

	// Token: 0x04000207 RID: 519
	public static int equipmentModelHash = "EquipmentModel".GetHashCode();

	// Token: 0x04000208 RID: 520
	public static int armorHash = "Armor".GetHashCode();

	// Token: 0x04000209 RID: 521
	public static int helmatHash = "Helmat".GetHashCode();

	// Token: 0x0400020A RID: 522
	public static int faceMaskHash = "FaceMask".GetHashCode();

	// Token: 0x0400020B RID: 523
	public static int backpackHash = "Backpack".GetHashCode();

	// Token: 0x0400020C RID: 524
	public static int headsetHash = "Headset".GetHashCode();

	// Token: 0x0400020D RID: 525
	private Slot armorSlot;

	// Token: 0x0400020E RID: 526
	private Slot helmatSlot;

	// Token: 0x0400020F RID: 527
	private Slot backpackSlot;

	// Token: 0x04000210 RID: 528
	private Slot faceMaskSlot;

	// Token: 0x04000211 RID: 529
	private Slot headsetSlot;
}
