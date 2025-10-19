using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem;
using Saves;
using UnityEngine;

namespace Debugging
{
	// Token: 0x0200021E RID: 542
	public class InventorySaveLoad : MonoBehaviour
	{
		// Token: 0x06001045 RID: 4165 RVA: 0x0003F12D File Offset: 0x0003D32D
		public void Save()
		{
			this.inventory.Save(this.key);
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x0003F140 File Offset: 0x0003D340
		public async UniTask Load()
		{
			this.loading = true;
			await ItemSavesUtilities.LoadInventory(this.key, this.inventory);
			this.loading = false;
			this.OnLoadFinished();
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x0003F183 File Offset: 0x0003D383
		private void OnLoadFinished()
		{
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x0003F185 File Offset: 0x0003D385
		public void BeginLoad()
		{
			this.Load().Forget();
		}

		// Token: 0x04000CFA RID: 3322
		public Inventory inventory;

		// Token: 0x04000CFB RID: 3323
		public string key = "helloInventory";

		// Token: 0x04000CFC RID: 3324
		private bool loading;
	}
}
