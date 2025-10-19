using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using ItemStatsSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B1 RID: 945
	public class ItemDecomposeView : View
	{
		// Token: 0x17000681 RID: 1665
		// (get) Token: 0x0600220C RID: 8716 RVA: 0x000769EA File Offset: 0x00074BEA
		public static ItemDecomposeView Instance
		{
			get
			{
				return View.GetViewInstance<ItemDecomposeView>();
			}
		}

		// Token: 0x17000682 RID: 1666
		// (get) Token: 0x0600220D RID: 8717 RVA: 0x000769F1 File Offset: 0x00074BF1
		private Item SelectedItem
		{
			get
			{
				return ItemUIUtilities.SelectedItem;
			}
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x000769F8 File Offset: 0x00074BF8
		protected override void Awake()
		{
			base.Awake();
			this.decomposeButton.onClick.AddListener(new UnityAction(this.OnDecomposeButtonClick));
			this.countSlider.OnValueChangedEvent += this.OnSliderValueChanged;
			this.SetupEmpty();
		}

		// Token: 0x0600220F RID: 8719 RVA: 0x00076A44 File Offset: 0x00074C44
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.countSlider.OnValueChangedEvent -= this.OnSliderValueChanged;
		}

		// Token: 0x06002210 RID: 8720 RVA: 0x00076A64 File Offset: 0x00074C64
		private void OnDecomposeButtonClick()
		{
			if (this.decomposing)
			{
				return;
			}
			if (this.SelectedItem == null)
			{
				return;
			}
			int value = this.countSlider.Value;
			this.DecomposeTask(this.SelectedItem, value).Forget();
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x00076AA7 File Offset: 0x00074CA7
		private void OnFastPick(UIInputEventData data)
		{
			this.OnDecomposeButtonClick();
			data.Use();
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x00076AB8 File Offset: 0x00074CB8
		private async UniTask DecomposeTask(Item item, int count)
		{
			this.decomposing = true;
			this.busyIndicator.SetActive(true);
			if (item != null)
			{
				AudioManager.PlayPutItemSFX(item, false);
			}
			await DecomposeDatabase.Decompose(item, count);
			this.busyIndicator.SetActive(false);
			this.decomposing = false;
			this.Refresh();
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x00076B0C File Offset: 0x00074D0C
		protected override void OnOpen()
		{
			base.OnOpen();
			this.fadeGroup.Show();
			ItemUIUtilities.Select(null);
			this.detailsFadeGroup.SkipHide();
			if (CharacterMainControl.Main != null)
			{
				this.characterInventoryDisplay.gameObject.SetActive(true);
				this.characterInventoryDisplay.Setup(CharacterMainControl.Main.CharacterItem.Inventory, null, (Item e) => e == null || DecomposeDatabase.CanDecompose(e.TypeID), false, null);
			}
			else
			{
				this.characterInventoryDisplay.gameObject.SetActive(false);
			}
			if (PlayerStorage.Inventory != null)
			{
				this.storageDisplay.gameObject.SetActive(true);
				this.storageDisplay.Setup(PlayerStorage.Inventory, null, (Item e) => e == null || DecomposeDatabase.CanDecompose(e.TypeID), false, null);
			}
			else
			{
				this.storageDisplay.gameObject.SetActive(false);
			}
			this.Refresh();
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x00076C11 File Offset: 0x00074E11
		protected override void OnClose()
		{
			base.OnClose();
			this.fadeGroup.Hide();
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x00076C24 File Offset: 0x00074E24
		private void OnEnable()
		{
			ItemUIUtilities.OnSelectionChanged += this.OnSelectionChanged;
			UIInputManager.OnFastPick += this.OnFastPick;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x00076C48 File Offset: 0x00074E48
		private void OnDisable()
		{
			ItemUIUtilities.OnSelectionChanged -= this.OnSelectionChanged;
			UIInputManager.OnFastPick -= this.OnFastPick;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x00076C6C File Offset: 0x00074E6C
		private void OnSelectionChanged()
		{
			this.Refresh();
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x00076C74 File Offset: 0x00074E74
		private void OnSliderValueChanged(float value)
		{
			this.RefreshResult(this.SelectedItem, Mathf.RoundToInt(value));
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x00076C88 File Offset: 0x00074E88
		private void Refresh()
		{
			if (this.SelectedItem == null)
			{
				this.SetupEmpty();
				return;
			}
			this.Setup(this.SelectedItem);
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x00076CAC File Offset: 0x00074EAC
		private void SetupEmpty()
		{
			this.detailsFadeGroup.Hide();
			this.targetNameDisplay.text = "-";
			this.resultDisplay.Clear();
			this.cannotDecomposeIndicator.SetActive(false);
			this.decomposeButton.gameObject.SetActive(false);
			this.noItemSelectedIndicator.SetActive(true);
			this.busyIndicator.SetActive(false);
			this.countSlider.SetMinMax(1, 1);
			this.countSlider.Value = 1;
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x00076D30 File Offset: 0x00074F30
		private void Setup(Item selectedItem)
		{
			if (selectedItem == null)
			{
				return;
			}
			this.noItemSelectedIndicator.SetActive(false);
			this.detailsDisplay.Setup(selectedItem);
			this.detailsFadeGroup.Show();
			this.targetNameDisplay.text = selectedItem.DisplayName;
			bool valid = DecomposeDatabase.GetDecomposeFormula(selectedItem.TypeID).valid;
			this.decomposeButton.gameObject.SetActive(valid);
			this.cannotDecomposeIndicator.gameObject.SetActive(!valid);
			this.SetupSlider(selectedItem);
			this.RefreshResult(selectedItem, Mathf.RoundToInt((float)this.countSlider.Value));
			this.busyIndicator.SetActive(this.decomposing);
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x00076DE4 File Offset: 0x00074FE4
		private void SetupSlider(Item selectedItem)
		{
			if (selectedItem.Stackable)
			{
				this.countSlider.SetMinMax(1, selectedItem.StackCount);
				this.countSlider.Value = selectedItem.StackCount;
				return;
			}
			this.countSlider.SetMinMax(1, 1);
			this.countSlider.Value = 1;
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x00076E38 File Offset: 0x00075038
		private void RefreshResult(Item selectedItem, int count)
		{
			if (selectedItem == null)
			{
				this.countSlider.SetMinMax(1, 1);
				this.countSlider.Value = 1;
				return;
			}
			DecomposeFormula decomposeFormula = DecomposeDatabase.GetDecomposeFormula(selectedItem.TypeID);
			if (decomposeFormula.valid)
			{
				bool stackable = selectedItem.Stackable;
				this.resultDisplay.Setup(decomposeFormula.result, count);
				return;
			}
			this.resultDisplay.Clear();
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x00076EA4 File Offset: 0x000750A4
		internal static void Show()
		{
			ItemDecomposeView instance = ItemDecomposeView.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Open(null);
		}

		// Token: 0x0400171B RID: 5915
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x0400171C RID: 5916
		[SerializeField]
		private InventoryDisplay characterInventoryDisplay;

		// Token: 0x0400171D RID: 5917
		[SerializeField]
		private InventoryDisplay storageDisplay;

		// Token: 0x0400171E RID: 5918
		[SerializeField]
		private FadeGroup detailsFadeGroup;

		// Token: 0x0400171F RID: 5919
		[SerializeField]
		private ItemDetailsDisplay detailsDisplay;

		// Token: 0x04001720 RID: 5920
		[SerializeField]
		private DecomposeSlider countSlider;

		// Token: 0x04001721 RID: 5921
		[SerializeField]
		private TextMeshProUGUI targetNameDisplay;

		// Token: 0x04001722 RID: 5922
		[SerializeField]
		private CostDisplay resultDisplay;

		// Token: 0x04001723 RID: 5923
		[SerializeField]
		private GameObject cannotDecomposeIndicator;

		// Token: 0x04001724 RID: 5924
		[SerializeField]
		private GameObject noItemSelectedIndicator;

		// Token: 0x04001725 RID: 5925
		[SerializeField]
		private Button decomposeButton;

		// Token: 0x04001726 RID: 5926
		[SerializeField]
		private GameObject busyIndicator;

		// Token: 0x04001727 RID: 5927
		private bool decomposing;
	}
}
