using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class DynamicItemDebugger : MonoBehaviour
{
	// Token: 0x060004B0 RID: 1200 RVA: 0x000157C4 File Offset: 0x000139C4
	private void Awake()
	{
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.Add();
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x000157D8 File Offset: 0x000139D8
	private void Add()
	{
		foreach (Item item in this.prefabs)
		{
			ItemAssetsCollection.AddDynamicEntry(item);
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001582C File Offset: 0x00013A2C
	private void CreateCorresponding()
	{
		this.CreateTask().Forget();
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001583C File Offset: 0x00013A3C
	private async UniTask CreateTask()
	{
		foreach (Item item in this.prefabs)
		{
			Item item2 = await ItemAssetsCollection.InstantiateAsync(item.TypeID);
			item2.transform.SetParent(base.transform);
			if (CharacterMainControl.Main)
			{
				ItemUtilities.SendToPlayer(item2, false, true);
			}
		}
		List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
	}

	// Token: 0x040003F8 RID: 1016
	[SerializeField]
	private List<Item> prefabs;
}
