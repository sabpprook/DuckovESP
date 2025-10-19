using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using Unity.VisualScripting;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ItemPickerDebug : MonoBehaviour
{
	// Token: 0x06000ABE RID: 2750 RVA: 0x0002E743 File Offset: 0x0002C943
	public void PickPlayerInventoryAndLog()
	{
		this.Pick().Forget();
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0002E750 File Offset: 0x0002C950
	private async UniTask Pick()
	{
		LevelManager instance = LevelManager.Instance;
		IEnumerable<Item> enumerable;
		if (instance == null)
		{
			enumerable = null;
		}
		else
		{
			CharacterMainControl mainCharacter = instance.MainCharacter;
			if (mainCharacter == null)
			{
				enumerable = null;
			}
			else
			{
				Item characterItem = mainCharacter.CharacterItem;
				enumerable = ((characterItem != null) ? characterItem.Inventory : null);
			}
		}
		Item item = await ItemPicker.Pick(enumerable.AsReadOnlyList<Item>());
		if (item == null)
		{
			Debug.Log("Nothing is selected");
		}
		else
		{
			Debug.Log(item.DisplayName);
		}
	}
}
