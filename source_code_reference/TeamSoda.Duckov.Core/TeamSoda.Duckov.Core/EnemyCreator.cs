using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov;
using Duckov.Scenes;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Items;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class EnemyCreator : MonoBehaviour
{
	// Token: 0x1700020B RID: 523
	// (get) Token: 0x060009F7 RID: 2551 RVA: 0x0002ABB4 File Offset: 0x00028DB4
	private int characterItemTypeID
	{
		get
		{
			return GameplayDataSettings.ItemAssets.DefaultCharacterItemTypeID;
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0002ABC0 File Offset: 0x00028DC0
	private void Start()
	{
		Debug.LogError("This scripts shouldn't exist!", this);
		if (LevelManager.LevelInited)
		{
			this.StartCreate();
			return;
		}
		LevelManager.OnLevelInitialized += this.StartCreate;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0002ABEC File Offset: 0x00028DEC
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.StartCreate;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0002AC00 File Offset: 0x00028E00
	private void StartCreate()
	{
		int creatorID = this.GetCreatorID();
		if (MultiSceneCore.Instance != null)
		{
			if (MultiSceneCore.Instance.usedCreatorIds.Contains(creatorID))
			{
				return;
			}
			MultiSceneCore.Instance.usedCreatorIds.Add(creatorID);
		}
		this.CreateCharacterAsync();
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0002AC4C File Offset: 0x00028E4C
	private async UniTaskVoid CreateCharacterAsync()
	{
		Item item = await this.LoadOrCreateCharacterItemInstance();
		Item characterItemInstance = item;
		List<Item> initialItems = await this.GenerateItems();
		this.character = await LevelManager.Instance.CharacterCreator.CreateCharacter(characterItemInstance, this.characterModel, base.transform.position, base.transform.rotation);
		this.character.SetTeam(Teams.scav);
		if (this.aiController)
		{
			global::UnityEngine.Object.Instantiate<AICharacterController>(this.aiController).Init(this.character, base.transform.position, this.voiceType, AudioManager.FootStepMaterialType.organic);
		}
		await UniTask.NextFrame();
		if (initialItems != null)
		{
			foreach (Item item2 in initialItems)
			{
				if (!(item2 == null) && !characterItemInstance.TryPlug(item2, false, null, 0) && !characterItemInstance.Inventory.AddAndMerge(item2, 0))
				{
					item2.DestroyTree();
				}
			}
		}
		await this.AddBullet();
		this.PlugAccessories();
		if (MultiSceneCore.MainScene != null && MultiSceneCore.MainScene.Value != base.gameObject.scene)
		{
			MultiSceneCore.MoveToActiveWithScene(base.gameObject);
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0002AC90 File Offset: 0x00028E90
	private void PlugAccessories()
	{
		Slot slot = this.character.PrimWeaponSlot();
		Item item = ((slot != null) ? slot.Content : null);
		if (item == null)
		{
			return;
		}
		CharacterMainControl characterMainControl = this.character;
		Inventory inventory;
		if (characterMainControl == null)
		{
			inventory = null;
		}
		else
		{
			Item characterItem = characterMainControl.CharacterItem;
			inventory = ((characterItem != null) ? characterItem.Inventory : null);
		}
		Inventory inventory2 = inventory;
		if (inventory2 == null)
		{
			return;
		}
		foreach (Item item2 in inventory2)
		{
			if (!(item2 == null))
			{
				item.TryPlug(item2, true, null, 0);
			}
		}
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0002AD30 File Offset: 0x00028F30
	private async UniTask AddBullet()
	{
		Slot slot = this.character.PrimWeaponSlot();
		Item item = ((slot != null) ? slot.Content : null);
		if (item != null)
		{
			string @string = item.Constants.GetString("Caliber", null);
			if (!string.IsNullOrEmpty(@string))
			{
				this.bulletFilter.caliber = @string;
				int[] array = ItemAssetsCollection.Search(this.bulletFilter);
				if (array.Length >= 1)
				{
					Item item2 = await ItemAssetsCollection.InstantiateAsync(array.GetRandom<int>());
					CharacterMainControl characterMainControl = this.character;
					if (characterMainControl != null)
					{
						Item characterItem = characterMainControl.CharacterItem;
						if (characterItem != null)
						{
							Inventory inventory = characterItem.Inventory;
							if (inventory != null)
							{
								inventory.AddItem(item2);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0002AD74 File Offset: 0x00028F74
	private async UniTask<List<Item>> GenerateItems()
	{
		List<Item> items = new List<Item>();
		foreach (RandomItemGenerateDescription randomItemGenerateDescription in this.itemsToGenerate)
		{
			List<Item> list = await randomItemGenerateDescription.Generate(-1);
			items.AddRange(list);
		}
		List<RandomItemGenerateDescription>.Enumerator enumerator = default(List<RandomItemGenerateDescription>.Enumerator);
		return items;
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0002ADB8 File Offset: 0x00028FB8
	private async UniTask<Item> LoadOrCreateCharacterItemInstance()
	{
		return await ItemAssetsCollection.InstantiateAsync(this.characterItemTypeID);
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0002ADFC File Offset: 0x00028FFC
	private int GetCreatorID()
	{
		Transform transform = base.transform.parent;
		string text = base.transform.GetSiblingIndex().ToString();
		while (transform != null)
		{
			text = string.Format("{0}/{1}", transform.GetSiblingIndex(), text);
			transform = transform.parent;
		}
		text = string.Format("{0}/{1}", base.gameObject.scene.buildIndex, text);
		return text.GetHashCode();
	}

	// Token: 0x040008BC RID: 2236
	private CharacterMainControl character;

	// Token: 0x040008BD RID: 2237
	[SerializeField]
	private List<RandomItemGenerateDescription> itemsToGenerate;

	// Token: 0x040008BE RID: 2238
	[SerializeField]
	private ItemFilter bulletFilter;

	// Token: 0x040008BF RID: 2239
	[SerializeField]
	private AudioManager.VoiceType voiceType;

	// Token: 0x040008C0 RID: 2240
	[SerializeField]
	private CharacterModel characterModel;

	// Token: 0x040008C1 RID: 2241
	[SerializeField]
	private AICharacterController aiController;
}
