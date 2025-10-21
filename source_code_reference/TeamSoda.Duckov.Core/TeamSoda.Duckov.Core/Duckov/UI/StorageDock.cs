using System;
using System.Collections.Generic;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem.Data;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003AA RID: 938
	public class StorageDock : View
	{
		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x060021AD RID: 8621 RVA: 0x000755E1 File Offset: 0x000737E1
		public static StorageDock Instance
		{
			get
			{
				return View.GetViewInstance<StorageDock>();
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x060021AE RID: 8622 RVA: 0x000755E8 File Offset: 0x000737E8
		private PrefabPool<StorageDockEntry> EntryPool
		{
			get
			{
				if (this._entryPool == null)
				{
					this._entryPool = new PrefabPool<StorageDockEntry>(this.entryTemplate, null, null, null, null, true, 10, 10000, null);
				}
				return this._entryPool;
			}
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x00075621 File Offset: 0x00073821
		protected override void Awake()
		{
			base.Awake();
			this.entryTemplate.gameObject.SetActive(false);
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x0007563A File Offset: 0x0007383A
		private void OnEnable()
		{
			PlayerStorage.OnTakeBufferItem += this.OnTakeBufferItem;
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x0007564D File Offset: 0x0007384D
		private void OnDisable()
		{
			PlayerStorage.OnTakeBufferItem -= this.OnTakeBufferItem;
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x00075660 File Offset: 0x00073860
		private void OnTakeBufferItem()
		{
			this.Refresh();
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x00075668 File Offset: 0x00073868
		protected override void OnOpen()
		{
			base.OnOpen();
			if (PlayerStorage.Instance == null)
			{
				base.Close();
				return;
			}
			this.fadeGroup.Show();
			this.Setup();
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x00075695 File Offset: 0x00073895
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x000756A8 File Offset: 0x000738A8
		private void Setup()
		{
			this.Refresh();
		}

		// Token: 0x060021B6 RID: 8630 RVA: 0x000756B0 File Offset: 0x000738B0
		private void Refresh()
		{
			this.EntryPool.ReleaseAll();
			List<ItemTreeData> incomingItemBuffer = PlayerStorage.IncomingItemBuffer;
			for (int i = 0; i < incomingItemBuffer.Count; i++)
			{
				ItemTreeData itemTreeData = incomingItemBuffer[i];
				if (itemTreeData != null)
				{
					this.EntryPool.Get(null).Setup(i, itemTreeData);
				}
			}
			this.placeHolder.gameObject.SetActive(incomingItemBuffer.Count <= 0);
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x00075719 File Offset: 0x00073919
		internal static void Show()
		{
			if (StorageDock.Instance == null)
			{
				return;
			}
			StorageDock.Instance.Open(null);
		}

		// Token: 0x040016C3 RID: 5827
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040016C4 RID: 5828
		[SerializeField]
		private StorageDockEntry entryTemplate;

		// Token: 0x040016C5 RID: 5829
		[SerializeField]
		private GameObject placeHolder;

		// Token: 0x040016C6 RID: 5830
		private PrefabPool<StorageDockEntry> _entryPool;
	}
}
