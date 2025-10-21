using System;
using System.Collections.Generic;
using ItemStatsSystem.Data;
using Saves;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class PlayerStorageBuffer : MonoBehaviour
{
	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000845 RID: 2117 RVA: 0x00024DB3 File Offset: 0x00022FB3
	// (set) Token: 0x06000846 RID: 2118 RVA: 0x00024DBA File Offset: 0x00022FBA
	public static PlayerStorageBuffer Instance { get; private set; }

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000847 RID: 2119 RVA: 0x00024DC2 File Offset: 0x00022FC2
	public static List<ItemTreeData> Buffer
	{
		get
		{
			return PlayerStorageBuffer.incomingItemBuffer;
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00024DC9 File Offset: 0x00022FC9
	private void Awake()
	{
		PlayerStorageBuffer.Instance = this;
		PlayerStorageBuffer.LoadBuffer();
		SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00024DE7 File Offset: 0x00022FE7
	private void OnCollectSaveData()
	{
		PlayerStorageBuffer.SaveBuffer();
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00024DF0 File Offset: 0x00022FF0
	public static void SaveBuffer()
	{
		List<ItemTreeData> list = new List<ItemTreeData>();
		foreach (ItemTreeData itemTreeData in PlayerStorageBuffer.incomingItemBuffer)
		{
			if (itemTreeData != null)
			{
				list.Add(itemTreeData);
			}
		}
		SavesSystem.Save<List<ItemTreeData>>("PlayerStorage_Buffer", list);
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00024E58 File Offset: 0x00023058
	public static void LoadBuffer()
	{
		PlayerStorageBuffer.incomingItemBuffer.Clear();
		List<ItemTreeData> list = SavesSystem.Load<List<ItemTreeData>>("PlayerStorage_Buffer");
		if (list != null)
		{
			if (list.Count <= 0)
			{
				Debug.Log("tree data is empty");
			}
			using (List<ItemTreeData>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemTreeData itemTreeData = enumerator.Current;
					PlayerStorageBuffer.incomingItemBuffer.Add(itemTreeData);
				}
				return;
			}
		}
		Debug.Log("Tree Data is null");
	}

	// Token: 0x0400077C RID: 1916
	private const string bufferSaveKey = "PlayerStorage_Buffer";

	// Token: 0x0400077D RID: 1917
	private static List<ItemTreeData> incomingItemBuffer = new List<ItemTreeData>();
}
