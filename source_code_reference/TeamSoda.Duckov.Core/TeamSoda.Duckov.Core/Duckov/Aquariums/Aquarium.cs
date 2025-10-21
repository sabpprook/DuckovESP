using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using ItemStatsSystem;
using Saves;
using UnityEngine;

namespace Duckov.Aquariums
{
	// Token: 0x0200031C RID: 796
	public class Aquarium : MonoBehaviour
	{
		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001A73 RID: 6771 RVA: 0x0005FA3D File Offset: 0x0005DC3D
		private string ItemSaveKey
		{
			get
			{
				return "Aquarium/Item/" + this.id;
			}
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x0005FA4F File Offset: 0x0005DC4F
		private void Awake()
		{
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0005FA62 File Offset: 0x0005DC62
		private void Start()
		{
			this.Load().Forget();
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0005FA6F File Offset: 0x0005DC6F
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0005FA84 File Offset: 0x0005DC84
		private async UniTask Load()
		{
			if (this.aquariumItem != null)
			{
				this.aquariumItem.DestroyTree();
			}
			int num = this.loadToken + 1;
			this.loadToken = num;
			int token = num;
			this.loading = true;
			Item item = await ItemSavesUtilities.LoadItem(this.ItemSaveKey);
			if (token == this.loadToken)
			{
				if (item == null)
				{
					item = await ItemAssetsCollection.InstantiateAsync(this.aquariumItemTypeID);
					if (token != this.loadToken)
					{
						return;
					}
				}
				this.aquariumItem = item;
				this.aquariumItem.transform.SetParent(base.transform);
				this.aquariumItem.onChildChanged += this.OnChildChanged;
				this.loading = false;
				this.loaded = true;
			}
		}

		// Token: 0x06001A78 RID: 6776 RVA: 0x0005FAC7 File Offset: 0x0005DCC7
		private void OnChildChanged(Item item)
		{
			this.dirty = true;
		}

		// Token: 0x06001A79 RID: 6777 RVA: 0x0005FAD0 File Offset: 0x0005DCD0
		private void FixedUpdate()
		{
			if (this.loading)
			{
				return;
			}
			if (this.dirty)
			{
				this.Refresh();
				this.dirty = false;
			}
		}

		// Token: 0x06001A7A RID: 6778 RVA: 0x0005FAF0 File Offset: 0x0005DCF0
		private void Refresh()
		{
			if (this.aquariumItem == null)
			{
				return;
			}
			foreach (Item item in this.aquariumItem.GetAllChildren(false, true))
			{
				if (!(item == null) && item.Tags.Contains("Fish"))
				{
					this.GetOrCreateGraphic(item) == null;
				}
			}
			this.graphicRecords.RemoveAll((Aquarium.ItemGraphicPair e) => e == null || e.graphic == null);
			for (int i = 0; i < this.graphicRecords.Count; i++)
			{
				Aquarium.ItemGraphicPair itemGraphicPair = this.graphicRecords[i];
				if (itemGraphicPair.item == null || itemGraphicPair.item.ParentItem != this.aquariumItem)
				{
					if (itemGraphicPair.graphic != null)
					{
						global::UnityEngine.Object.Destroy(itemGraphicPair.graphic);
					}
					this.graphicRecords.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x06001A7B RID: 6779 RVA: 0x0005FC18 File Offset: 0x0005DE18
		private ItemGraphicInfo GetOrCreateGraphic(Item item)
		{
			if (item == null)
			{
				return null;
			}
			Aquarium.ItemGraphicPair itemGraphicPair = this.graphicRecords.Find((Aquarium.ItemGraphicPair e) => e != null && e.item == item);
			if (itemGraphicPair != null && itemGraphicPair.graphic != null)
			{
				return itemGraphicPair.graphic;
			}
			ItemGraphicInfo itemGraphicInfo = ItemGraphicInfo.CreateAGraphic(item, this.graphicsParent);
			if (itemGraphicPair != null)
			{
				this.graphicRecords.Remove(itemGraphicPair);
			}
			if (itemGraphicInfo == null)
			{
				return null;
			}
			IAquariumContent component = itemGraphicInfo.GetComponent<IAquariumContent>();
			if (component != null)
			{
				component.Setup(this);
			}
			this.graphicRecords.Add(new Aquarium.ItemGraphicPair
			{
				item = item,
				graphic = itemGraphicInfo
			});
			return itemGraphicInfo;
		}

		// Token: 0x06001A7C RID: 6780 RVA: 0x0005FCD4 File Offset: 0x0005DED4
		public void Loot()
		{
			LootView.LootItem(this.aquariumItem);
		}

		// Token: 0x06001A7D RID: 6781 RVA: 0x0005FCE1 File Offset: 0x0005DEE1
		private void Save()
		{
			if (this.loading)
			{
				return;
			}
			if (!this.loaded)
			{
				return;
			}
			this.aquariumItem.Save(this.ItemSaveKey);
		}

		// Token: 0x040012F9 RID: 4857
		[SerializeField]
		private string id = "Default";

		// Token: 0x040012FA RID: 4858
		[SerializeField]
		private Transform graphicsParent;

		// Token: 0x040012FB RID: 4859
		[ItemTypeID]
		private int aquariumItemTypeID = 1158;

		// Token: 0x040012FC RID: 4860
		private Item aquariumItem;

		// Token: 0x040012FD RID: 4861
		private List<Aquarium.ItemGraphicPair> graphicRecords = new List<Aquarium.ItemGraphicPair>();

		// Token: 0x040012FE RID: 4862
		private bool loading;

		// Token: 0x040012FF RID: 4863
		private bool loaded;

		// Token: 0x04001300 RID: 4864
		private int loadToken;

		// Token: 0x04001301 RID: 4865
		private bool dirty = true;

		// Token: 0x020005B7 RID: 1463
		private class ItemGraphicPair
		{
			// Token: 0x04002057 RID: 8279
			public Item item;

			// Token: 0x04002058 RID: 8280
			public ItemGraphicInfo graphic;
		}
	}
}
