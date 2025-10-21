using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using Saves;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class ItemStarterkit : InteractableBase
{
	// Token: 0x06000708 RID: 1800 RVA: 0x0001FB73 File Offset: 0x0001DD73
	protected override bool IsInteractable()
	{
		return !this.picked && this.cached;
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x0001FB8C File Offset: 0x0001DD8C
	private async UniTask CacheItems()
	{
		if (!this.caching)
		{
			if (!this.cached)
			{
				this.caching = true;
				this.cached = false;
				this.itemsCache = new List<Item>();
				foreach (int num in this.items)
				{
					Item item = await ItemAssetsCollection.InstantiateAsync(num);
					if (!(item == null))
					{
						item.transform.SetParent(base.transform);
						this.itemsCache.Add(item);
					}
				}
				List<int>.Enumerator enumerator = default(List<int>.Enumerator);
				this.caching = false;
				this.cached = true;
			}
		}
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0001FBCF File Offset: 0x0001DDCF
	protected override void Awake()
	{
		base.Awake();
		SavesSystem.OnCollectSaveData += this.Save;
		SceneLoader.onStartedLoadingScene += this.OnStartedLoadingScene;
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0001FBF9 File Offset: 0x0001DDF9
	protected override void OnDestroy()
	{
		SavesSystem.OnCollectSaveData -= this.Save;
		SceneLoader.onStartedLoadingScene -= this.OnStartedLoadingScene;
		base.OnDestroy();
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0001FC23 File Offset: 0x0001DE23
	private void OnStartedLoadingScene(SceneLoadingContext context)
	{
		this.picked = false;
		this.Save();
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0001FC32 File Offset: 0x0001DE32
	private void Save()
	{
		SavesSystem.Save<bool>(this.saveKey, this.picked);
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x0001FC48 File Offset: 0x0001DE48
	private void Load()
	{
		this.picked = SavesSystem.Load<bool>(this.saveKey);
		base.MarkerActive = !this.picked;
		if (this.notPickedItem)
		{
			GameObject gameObject = this.notPickedItem;
			if (gameObject != null)
			{
				gameObject.SetActive(!this.picked);
			}
		}
		if (this.pickedItem)
		{
			this.pickedItem.SetActive(this.picked);
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0001FCBA File Offset: 0x0001DEBA
	protected override void Start()
	{
		base.Start();
		this.Load();
		if (!this.picked)
		{
			this.CacheItems().Forget();
		}
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x0001FCDC File Offset: 0x0001DEDC
	protected override void OnInteractFinished()
	{
		foreach (Item item in this.itemsCache)
		{
			ItemUtilities.SendToPlayerCharacter(item, false);
		}
		this.picked = true;
		base.MarkerActive = !this.picked;
		this.itemsCache.Clear();
		this.OnPicked();
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0001FD58 File Offset: 0x0001DF58
	private void OnPicked()
	{
		if (this.notPickedItem)
		{
			this.notPickedItem.SetActive(false);
		}
		if (this.pickedItem)
		{
			this.pickedItem.SetActive(true);
		}
		if (this.pickFX)
		{
			this.pickFX.SetActive(true);
		}
		NotificationText.Push(this.notificationTextKey.ToPlainText());
	}

	// Token: 0x040006AF RID: 1711
	[ItemTypeID]
	[SerializeField]
	private List<int> items;

	// Token: 0x040006B0 RID: 1712
	[SerializeField]
	private GameObject notPickedItem;

	// Token: 0x040006B1 RID: 1713
	[SerializeField]
	private GameObject pickedItem;

	// Token: 0x040006B2 RID: 1714
	[SerializeField]
	private GameObject pickFX;

	// Token: 0x040006B3 RID: 1715
	private List<Item> itemsCache;

	// Token: 0x040006B4 RID: 1716
	[SerializeField]
	private string notificationTextKey;

	// Token: 0x040006B5 RID: 1717
	private bool caching;

	// Token: 0x040006B6 RID: 1718
	private bool cached;

	// Token: 0x040006B7 RID: 1719
	private bool picked;

	// Token: 0x040006B8 RID: 1720
	private string saveKey = "StarterKit_Picked";
}
