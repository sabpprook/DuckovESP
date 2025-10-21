using System;
using ItemStatsSystem;
using TMPro;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class StorageDockCountTMP : MonoBehaviour
{
	// Token: 0x06000EE4 RID: 3812 RVA: 0x0003B2D2 File Offset: 0x000394D2
	private void Awake()
	{
		PlayerStorage.OnItemAddedToBuffer += this.OnItemAddedToBuffer;
		PlayerStorage.OnTakeBufferItem += this.OnTakeBufferItem;
		PlayerStorage.OnLoadingFinished += this.OnLoadingFinished;
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0003B307 File Offset: 0x00039507
	private void OnDestroy()
	{
		PlayerStorage.OnItemAddedToBuffer -= this.OnItemAddedToBuffer;
		PlayerStorage.OnTakeBufferItem -= this.OnTakeBufferItem;
		PlayerStorage.OnLoadingFinished -= this.OnLoadingFinished;
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0003B33C File Offset: 0x0003953C
	private void OnLoadingFinished()
	{
		this.Refresh();
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x0003B344 File Offset: 0x00039544
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x0003B34C File Offset: 0x0003954C
	private void OnTakeBufferItem()
	{
		this.Refresh();
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x0003B354 File Offset: 0x00039554
	private void OnItemAddedToBuffer(Item item)
	{
		this.Refresh();
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x0003B35C File Offset: 0x0003955C
	private void Refresh()
	{
		int count = PlayerStorage.IncomingItemBuffer.Count;
		this.tmp.text = string.Format("{0}", count);
		if (this.setActiveFalseWhenCountIsZero)
		{
			base.gameObject.SetActive(count > 0);
		}
	}

	// Token: 0x04000C43 RID: 3139
	[SerializeField]
	private TextMeshPro tmp;

	// Token: 0x04000C44 RID: 3140
	[SerializeField]
	private bool setActiveFalseWhenCountIsZero;
}
