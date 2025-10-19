using System;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class CharacterCreator : MonoBehaviour
{
	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06000867 RID: 2151 RVA: 0x00025774 File Offset: 0x00023974
	public CharacterMainControl characterPfb
	{
		get
		{
			return GameplayDataSettings.Prefabs.CharacterPrefab;
		}
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00025780 File Offset: 0x00023980
	public async UniTask<CharacterMainControl> CreateCharacter(Item itemInstance, CharacterModel modelPrefab, Vector3 pos, Quaternion rotation)
	{
		CharacterMainControl characterMainControl = global::UnityEngine.Object.Instantiate<CharacterMainControl>(this.characterPfb, pos, rotation);
		CharacterModel characterModel = global::UnityEngine.Object.Instantiate<CharacterModel>(modelPrefab);
		characterMainControl.SetCharacterModel(characterModel);
		CharacterMainControl characterMainControl2;
		if (itemInstance == null)
		{
			if (characterMainControl)
			{
				global::UnityEngine.Object.Destroy(characterMainControl.gameObject);
			}
			characterMainControl2 = null;
		}
		else
		{
			characterMainControl.SetItem(itemInstance);
			if (!LevelManager.Instance.IsRaidMap)
			{
				characterMainControl.AddBuff(GameplayDataSettings.Buffs.BaseBuff, null, 0);
			}
			characterMainControl2 = characterMainControl;
		}
		return characterMainControl2;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000257E4 File Offset: 0x000239E4
	public async UniTask<Item> LoadOrCreateCharacterItemInstance(int itemTypeID)
	{
		return await ItemAssetsCollection.InstantiateAsync(itemTypeID);
	}
}
