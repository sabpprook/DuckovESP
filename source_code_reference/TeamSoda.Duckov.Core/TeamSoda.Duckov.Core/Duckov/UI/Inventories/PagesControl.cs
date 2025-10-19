using System;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI.Inventories
{
	// Token: 0x020003C6 RID: 966
	public class PagesControl : MonoBehaviour
	{
		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06002320 RID: 8992 RVA: 0x0007B0EC File Offset: 0x000792EC
		private PrefabPool<PagesControl_Entry> Pool
		{
			get
			{
				if (this._pool == null)
				{
					this._pool = new PrefabPool<PagesControl_Entry>(this.template, null, null, null, null, true, 10, 10000, null);
				}
				return this._pool;
			}
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x0007B125 File Offset: 0x00079325
		private void Start()
		{
			if (this.target != null)
			{
				this.Setup(this.target);
			}
		}

		// Token: 0x06002322 RID: 8994 RVA: 0x0007B141 File Offset: 0x00079341
		public void Setup(InventoryDisplay target)
		{
			this.UnregisterEvents();
			this.target = target;
			this.RegisterEvents();
			this.Refresh();
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x0007B15C File Offset: 0x0007935C
		private void RegisterEvents()
		{
			this.UnregisterEvents();
			if (this.target == null)
			{
				return;
			}
			this.target.onPageInfoRefreshed += this.OnPageInfoRefreshed;
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x0007B18A File Offset: 0x0007938A
		private void UnregisterEvents()
		{
			if (this.target == null)
			{
				return;
			}
			this.target.onPageInfoRefreshed -= this.OnPageInfoRefreshed;
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x0007B1B2 File Offset: 0x000793B2
		private void OnPageInfoRefreshed()
		{
			this.Refresh();
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x0007B1BC File Offset: 0x000793BC
		private void Refresh()
		{
			this.Pool.ReleaseAll();
			if (this.inputIndicators)
			{
				GameObject gameObject = this.inputIndicators;
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			if (this.target == null)
			{
				return;
			}
			if (!this.target.UsePages)
			{
				return;
			}
			if (this.target.MaxPage <= 1)
			{
				return;
			}
			for (int i = 0; i < this.target.MaxPage; i++)
			{
				this.Pool.Get(null).Setup(this, i, this.target.SelectedPage == i);
			}
			if (this.inputIndicators)
			{
				GameObject gameObject2 = this.inputIndicators;
				if (gameObject2 == null)
				{
					return;
				}
				gameObject2.SetActive(true);
			}
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x0007B274 File Offset: 0x00079474
		internal void NotifySelect(int i)
		{
			if (this.target == null)
			{
				return;
			}
			this.target.SetPage(i);
		}

		// Token: 0x040017E9 RID: 6121
		[SerializeField]
		private InventoryDisplay target;

		// Token: 0x040017EA RID: 6122
		[SerializeField]
		private PagesControl_Entry template;

		// Token: 0x040017EB RID: 6123
		[SerializeField]
		private GameObject inputIndicators;

		// Token: 0x040017EC RID: 6124
		private PrefabPool<PagesControl_Entry> _pool;
	}
}
