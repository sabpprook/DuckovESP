using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Duckov.UI;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001A8 RID: 424
public class CraftView : View, ISingleSelectionMenu<CraftView_ListEntry>
{
	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06000C83 RID: 3203 RVA: 0x00034A04 File Offset: 0x00032C04
	private static CraftView Instance
	{
		get
		{
			return View.GetViewInstance<CraftView>();
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06000C84 RID: 3204 RVA: 0x00034A0C File Offset: 0x00032C0C
	private PrefabPool<CraftView_ListEntry> ListEntryPool
	{
		get
		{
			if (this._listEntryPool == null)
			{
				this._listEntryPool = new PrefabPool<CraftView_ListEntry>(this.listEntryTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._listEntryPool;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06000C85 RID: 3205 RVA: 0x00034A45 File Offset: 0x00032C45
	private string NotificationFormat
	{
		get
		{
			return this.notificationFormatKey.ToPlainText();
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00034A54 File Offset: 0x00032C54
	private PrefabPool<CraftViewFilterBtnEntry> FilterBtnPool
	{
		get
		{
			if (this._filterBtnPool == null)
			{
				this._filterBtnPool = new PrefabPool<CraftViewFilterBtnEntry>(this.filterBtnTemplate, null, null, null, null, true, 10, 10000, null);
			}
			return this._filterBtnPool;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06000C87 RID: 3207 RVA: 0x00034A8D File Offset: 0x00032C8D
	private CraftView.FilterInfo CurrentFilter
	{
		get
		{
			if (this.currentFilterIndex < 0 || this.currentFilterIndex >= this.filters.Length)
			{
				this.currentFilterIndex = 0;
			}
			return this.filters[this.currentFilterIndex];
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00034AC0 File Offset: 0x00032CC0
	public void SetFilter(int index)
	{
		if (index < 0 || index >= this.filters.Length)
		{
			return;
		}
		this.currentFilterIndex = index;
		this.selectedEntry = null;
		this.RefreshDetails();
		this.RefreshList(this.predicate);
		this.RefreshFilterButtons();
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00034AF8 File Offset: 0x00032CF8
	private static bool CheckFilter(CraftingFormula formula, CraftView.FilterInfo filter)
	{
		if (filter.requireTags.Length == 0)
		{
			return true;
		}
		ItemMetaData metaData = ItemAssetsCollection.GetMetaData(formula.result.id);
		foreach (Tag tag in filter.requireTags)
		{
			if (metaData.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x00034B4B File Offset: 0x00032D4B
	protected override void Awake()
	{
		base.Awake();
		this.listEntryTemplate.gameObject.SetActive(false);
		this.craftButton.onClick.AddListener(new UnityAction(this.OnCraftButtonClicked));
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00034B80 File Offset: 0x00032D80
	private void OnCraftButtonClicked()
	{
		this.CraftTask().Forget();
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00034B90 File Offset: 0x00032D90
	private async UniTask CraftTask()
	{
		if (!this.crafting && !(this.selectedEntry == null) && !(CraftingManager.Instance == null))
		{
			this.crafting = true;
			List<Item> list = await CraftingManager.Instance.Craft(this.selectedEntry.Formula.id);
			if (list != null)
			{
				foreach (Item item in list)
				{
					this.OnCraftFinished(item);
				}
			}
		}
		this.crafting = false;
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00034BD4 File Offset: 0x00032DD4
	private void OnCraftFinished(Item item)
	{
		if (item == null)
		{
			return;
		}
		string displayName = item.DisplayName;
		NotificationText.Push(this.NotificationFormat.Format(new
		{
			itemDisplayName = displayName
		}));
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00034C08 File Offset: 0x00032E08
	protected override void OnOpen()
	{
		base.OnOpen();
		this.fadeGroup.Show();
		this.SetFilter(0);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00034C22 File Offset: 0x00032E22
	protected override void OnClose()
	{
		base.OnClose();
		this.fadeGroup.Hide();
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00034C35 File Offset: 0x00032E35
	public static void SetupAndOpenView(Predicate<CraftingFormula> predicate)
	{
		if (!CraftView.Instance)
		{
			return;
		}
		CraftView.Instance.SetupAndOpen(predicate);
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00034C50 File Offset: 0x00032E50
	public void SetupAndOpen(Predicate<CraftingFormula> predicate)
	{
		this.predicate = predicate;
		this.detailsFadeGroup.SkipHide();
		this.loadingIndicator.SkipHide();
		this.placeHolderFadeGroup.SkipShow();
		this.selectedEntry = null;
		this.RefreshDetails();
		this.RefreshList(predicate);
		this.RefreshFilterButtons();
		base.Open(null);
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00034CA8 File Offset: 0x00032EA8
	private void RefreshList(Predicate<CraftingFormula> predicate)
	{
		this.ListEntryPool.ReleaseAll();
		IEnumerable<string> unlockedFormulaIDs = CraftingManager.UnlockedFormulaIDs;
		CraftView.FilterInfo currentFilter = this.CurrentFilter;
		bool flag = currentFilter.requireTags != null && currentFilter.requireTags.Length != 0;
		using (IEnumerator<string> enumerator = unlockedFormulaIDs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CraftingFormula craftingFormula;
				if (CraftingFormulaCollection.TryGetFormula(enumerator.Current, out craftingFormula) && predicate(craftingFormula) && (!flag || CraftView.CheckFilter(craftingFormula, currentFilter)))
				{
					this.ListEntryPool.Get(null).Setup(this, craftingFormula);
				}
			}
		}
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x00034D48 File Offset: 0x00032F48
	private int CountFilter(CraftView.FilterInfo filter)
	{
		IEnumerable<string> unlockedFormulaIDs = CraftingManager.UnlockedFormulaIDs;
		bool flag = filter.requireTags != null && filter.requireTags.Length != 0;
		int num = 0;
		using (IEnumerator<string> enumerator = unlockedFormulaIDs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				CraftingFormula craftingFormula;
				if (CraftingFormulaCollection.TryGetFormula(enumerator.Current, out craftingFormula) && this.predicate(craftingFormula) && (!flag || CraftView.CheckFilter(craftingFormula, filter)))
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00034DCC File Offset: 0x00032FCC
	private void RefreshFilterButtons()
	{
		this.FilterBtnPool.ReleaseAll();
		int num = 0;
		foreach (CraftView.FilterInfo filterInfo in this.filters)
		{
			if (this.CountFilter(filterInfo) < 1)
			{
				num++;
			}
			else
			{
				this.FilterBtnPool.Get(null).Setup(this, filterInfo, num, num == this.currentFilterIndex);
				num++;
			}
		}
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x00034E34 File Offset: 0x00033034
	public CraftView_ListEntry GetSelection()
	{
		return this.selectedEntry;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x00034E3C File Offset: 0x0003303C
	public bool SetSelection(CraftView_ListEntry selection)
	{
		if (this.selectedEntry != null)
		{
			CraftView_ListEntry craftView_ListEntry = this.selectedEntry;
			this.selectedEntry = null;
			craftView_ListEntry.NotifyUnselected();
		}
		this.selectedEntry = selection;
		this.selectedEntry.NotifySelected();
		this.RefreshDetails();
		return true;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x00034E77 File Offset: 0x00033077
	private void RefreshDetails()
	{
		this.RefreshTask(this.NewRefreshToken()).Forget();
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00034E8C File Offset: 0x0003308C
	private int NewRefreshToken()
	{
		int num;
		do
		{
			num = global::UnityEngine.Random.Range(0, int.MaxValue);
		}
		while (num == this.refreshTaskToken);
		this.refreshTaskToken = num;
		return num;
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00034EB8 File Offset: 0x000330B8
	private UniTask RefreshTask(int token)
	{
		CraftView.<RefreshTask>d__50 <RefreshTask>d__;
		<RefreshTask>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<RefreshTask>d__.<>4__this = this;
		<RefreshTask>d__.token = token;
		<RefreshTask>d__.<>1__state = -1;
		<RefreshTask>d__.<>t__builder.Start<CraftView.<RefreshTask>d__50>(ref <RefreshTask>d__);
		return <RefreshTask>d__.<>t__builder.Task;
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00034F03 File Offset: 0x00033103
	private void TestShow()
	{
		CraftingManager.UnlockFormula("Biscuit");
		CraftingManager.UnlockFormula("Character");
		this.SetupAndOpen((CraftingFormula e) => true);
	}

	// Token: 0x04000AD7 RID: 2775
	[SerializeField]
	private FadeGroup fadeGroup;

	// Token: 0x04000AD8 RID: 2776
	[SerializeField]
	private CraftView_ListEntry listEntryTemplate;

	// Token: 0x04000AD9 RID: 2777
	private PrefabPool<CraftView_ListEntry> _listEntryPool;

	// Token: 0x04000ADA RID: 2778
	[SerializeField]
	private FadeGroup detailsFadeGroup;

	// Token: 0x04000ADB RID: 2779
	[SerializeField]
	private FadeGroup loadingIndicator;

	// Token: 0x04000ADC RID: 2780
	[SerializeField]
	private FadeGroup placeHolderFadeGroup;

	// Token: 0x04000ADD RID: 2781
	[SerializeField]
	private ItemDetailsDisplay detailsDisplay;

	// Token: 0x04000ADE RID: 2782
	[SerializeField]
	private CostDisplay costDisplay;

	// Token: 0x04000ADF RID: 2783
	[SerializeField]
	private Color crafableColor;

	// Token: 0x04000AE0 RID: 2784
	[SerializeField]
	private Color notCraftableColor;

	// Token: 0x04000AE1 RID: 2785
	[SerializeField]
	private Image buttonImage;

	// Token: 0x04000AE2 RID: 2786
	[SerializeField]
	private Button craftButton;

	// Token: 0x04000AE3 RID: 2787
	[LocalizationKey("Default")]
	[SerializeField]
	private string notificationFormatKey;

	// Token: 0x04000AE4 RID: 2788
	[SerializeField]
	private CraftViewFilterBtnEntry filterBtnTemplate;

	// Token: 0x04000AE5 RID: 2789
	[SerializeField]
	private CraftView.FilterInfo[] filters;

	// Token: 0x04000AE6 RID: 2790
	private PrefabPool<CraftViewFilterBtnEntry> _filterBtnPool;

	// Token: 0x04000AE7 RID: 2791
	private int currentFilterIndex;

	// Token: 0x04000AE8 RID: 2792
	private bool crafting;

	// Token: 0x04000AE9 RID: 2793
	private Predicate<CraftingFormula> predicate;

	// Token: 0x04000AEA RID: 2794
	private CraftView_ListEntry selectedEntry;

	// Token: 0x04000AEB RID: 2795
	private int refreshTaskToken;

	// Token: 0x04000AEC RID: 2796
	private Item tempItem;

	// Token: 0x020004C3 RID: 1219
	[Serializable]
	public struct FilterInfo
	{
		// Token: 0x04001CA2 RID: 7330
		[LocalizationKey("Default")]
		[SerializeField]
		public string displayNameKey;

		// Token: 0x04001CA3 RID: 7331
		[SerializeField]
		public Sprite icon;

		// Token: 0x04001CA4 RID: 7332
		[SerializeField]
		public Tag[] requireTags;
	}
}
