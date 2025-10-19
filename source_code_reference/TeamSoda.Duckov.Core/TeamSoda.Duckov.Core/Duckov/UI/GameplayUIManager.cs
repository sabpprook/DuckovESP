using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using UnityEngine;

namespace Duckov.UI
{
	// Token: 0x020003A5 RID: 933
	public class GameplayUIManager : MonoBehaviour
	{
		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x0600215F RID: 8543 RVA: 0x000748CB File Offset: 0x00072ACB
		public static GameplayUIManager Instance
		{
			get
			{
				return GameplayUIManager.instance;
			}
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06002160 RID: 8544 RVA: 0x000748D2 File Offset: 0x00072AD2
		public View ActiveView
		{
			get
			{
				return View.ActiveView;
			}
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000748DC File Offset: 0x00072ADC
		public static T GetViewInstance<T>() where T : View
		{
			if (GameplayUIManager.Instance == null)
			{
				return default(T);
			}
			View view;
			if (GameplayUIManager.Instance.viewDic.TryGetValue(typeof(T), out view))
			{
				return view as T;
			}
			View view2 = GameplayUIManager.Instance.views.Find((View e) => e is T);
			if (view2 == null)
			{
				return default(T);
			}
			GameplayUIManager.Instance.viewDic[typeof(T)] = view2;
			return view2 as T;
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x00074990 File Offset: 0x00072B90
		private void Awake()
		{
			if (GameplayUIManager.instance == null)
			{
				GameplayUIManager.instance = this;
			}
			else
			{
				Debug.LogWarning("Duplicate Gameplay UI Manager detected!");
			}
			foreach (View view in this.views)
			{
				view.gameObject.SetActive(true);
			}
			foreach (GameObject gameObject in this.setActiveOnAwake)
			{
				if (!(gameObject == null))
				{
					gameObject.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06002163 RID: 8547 RVA: 0x00074A58 File Offset: 0x00072C58
		public PrefabPool<ItemDisplay> ItemDisplayPool
		{
			get
			{
				if (this.itemDisplayPool == null)
				{
					this.itemDisplayPool = new PrefabPool<ItemDisplay>(GameplayDataSettings.UIPrefabs.ItemDisplay, base.transform, null, null, null, true, 10, 10000, null);
				}
				return this.itemDisplayPool;
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06002164 RID: 8548 RVA: 0x00074A9C File Offset: 0x00072C9C
		public PrefabPool<SlotDisplay> SlotDisplayPool
		{
			get
			{
				if (this.slotDisplayPool == null)
				{
					this.slotDisplayPool = new PrefabPool<SlotDisplay>(GameplayDataSettings.UIPrefabs.SlotDisplay, base.transform, null, null, null, true, 10, 10000, null);
				}
				return this.slotDisplayPool;
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06002165 RID: 8549 RVA: 0x00074AE0 File Offset: 0x00072CE0
		public PrefabPool<InventoryEntry> InventoryEntryPool
		{
			get
			{
				if (this.inventoryEntryPool == null)
				{
					this.inventoryEntryPool = new PrefabPool<InventoryEntry>(GameplayDataSettings.UIPrefabs.InventoryEntry, base.transform, null, null, null, true, 10, 10000, null);
				}
				return this.inventoryEntryPool;
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06002166 RID: 8550 RVA: 0x00074B22 File Offset: 0x00072D22
		public SplitDialogue SplitDialogue
		{
			get
			{
				return this._splitDialogue;
			}
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x00074B2A File Offset: 0x00072D2A
		public static UniTask TemporaryHide()
		{
			if (GameplayUIManager.Instance == null)
			{
				return UniTask.CompletedTask;
			}
			GameplayUIManager.Instance.canvasGroup.blocksRaycasts = false;
			return GameplayUIManager.Instance.fadeGroup.HideAndReturnTask();
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x00074B5E File Offset: 0x00072D5E
		public static UniTask ReverseTemporaryHide()
		{
			if (GameplayUIManager.Instance == null)
			{
				return UniTask.CompletedTask;
			}
			GameplayUIManager.Instance.canvasGroup.blocksRaycasts = true;
			return GameplayUIManager.Instance.fadeGroup.ShowAndReturnTask();
		}

		// Token: 0x0400169F RID: 5791
		private static GameplayUIManager instance;

		// Token: 0x040016A0 RID: 5792
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x040016A1 RID: 5793
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x040016A2 RID: 5794
		[SerializeField]
		private List<View> views = new List<View>();

		// Token: 0x040016A3 RID: 5795
		[SerializeField]
		private List<GameObject> setActiveOnAwake;

		// Token: 0x040016A4 RID: 5796
		private Dictionary<Type, View> viewDic = new Dictionary<Type, View>();

		// Token: 0x040016A5 RID: 5797
		private PrefabPool<ItemDisplay> itemDisplayPool;

		// Token: 0x040016A6 RID: 5798
		private PrefabPool<SlotDisplay> slotDisplayPool;

		// Token: 0x040016A7 RID: 5799
		private PrefabPool<InventoryEntry> inventoryEntryPool;

		// Token: 0x040016A8 RID: 5800
		[SerializeField]
		private SplitDialogue _splitDialogue;
	}
}
