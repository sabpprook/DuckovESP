using System;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.Buildings.UI
{
	// Token: 0x02000317 RID: 791
	public class BuildingBtnEntry : MonoBehaviour
	{
		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001A3D RID: 6717 RVA: 0x0005F07E File Offset: 0x0005D27E
		private string TokenFormat
		{
			get
			{
				return this.tokenFormatKey.ToPlainText();
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001A3E RID: 6718 RVA: 0x0005F08B File Offset: 0x0005D28B
		public BuildingInfo Info
		{
			get
			{
				return this.info;
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001A3F RID: 6719 RVA: 0x0005F093 File Offset: 0x0005D293
		public bool CostEnough
		{
			get
			{
				return this.info.TokenAmount > 0 || this.info.cost.Enough;
			}
		}

		// Token: 0x140000AA RID: 170
		// (add) Token: 0x06001A40 RID: 6720 RVA: 0x0005F0BC File Offset: 0x0005D2BC
		// (remove) Token: 0x06001A41 RID: 6721 RVA: 0x0005F0F4 File Offset: 0x0005D2F4
		public event Action<BuildingBtnEntry> onButtonClicked;

		// Token: 0x140000AB RID: 171
		// (add) Token: 0x06001A42 RID: 6722 RVA: 0x0005F12C File Offset: 0x0005D32C
		// (remove) Token: 0x06001A43 RID: 6723 RVA: 0x0005F164 File Offset: 0x0005D364
		public event Action<BuildingBtnEntry> onRecycleRequested;

		// Token: 0x06001A44 RID: 6724 RVA: 0x0005F199 File Offset: 0x0005D399
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			this.recycleButton.onPressFullfilled.AddListener(new UnityAction(this.OnRecycleButtonTriggered));
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x0005F1D3 File Offset: 0x0005D3D3
		private void OnRecycleButtonTriggered()
		{
			Action<BuildingBtnEntry> action = this.onRecycleRequested;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x0005F1E6 File Offset: 0x0005D3E6
		private void OnEnable()
		{
			BuildingManager.OnBuildingListChanged += this.Refresh;
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x0005F1F9 File Offset: 0x0005D3F9
		private void OnDisable()
		{
			BuildingManager.OnBuildingListChanged -= this.Refresh;
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x0005F20C File Offset: 0x0005D40C
		private void OnButtonClicked()
		{
			Action<BuildingBtnEntry> action = this.onButtonClicked;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x0005F21F File Offset: 0x0005D41F
		internal void Setup(BuildingInfo buildingInfo)
		{
			this.info = buildingInfo;
			this.Refresh();
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x0005F230 File Offset: 0x0005D430
		private void Refresh()
		{
			int tokenAmount = this.info.TokenAmount;
			this.nameText.text = this.info.DisplayName;
			this.descriptionText.text = this.info.Description;
			this.tokenText.text = this.TokenFormat.Format(new { tokenAmount });
			this.icon.sprite = this.info.iconReference;
			this.costDisplay.Setup(this.info.cost, 1);
			this.costDisplay.gameObject.SetActive(tokenAmount <= 0);
			bool reachedAmountLimit = this.info.ReachedAmountLimit;
			this.amountText.text = ((this.info.maxAmount > 0) ? string.Format("{0}/{1}", this.info.CurrentAmount, this.info.maxAmount) : string.Format("{0}/∞", this.info.CurrentAmount));
			this.reachedAmountLimitationIndicator.SetActive(reachedAmountLimit);
			bool flag = !this.info.ReachedAmountLimit && this.CostEnough;
			this.backGround.color = (flag ? this.avaliableColor : this.normalColor);
			this.recycleButton.gameObject.SetActive(this.info.CurrentAmount > 0);
		}

		// Token: 0x040012D7 RID: 4823
		[SerializeField]
		private Button button;

		// Token: 0x040012D8 RID: 4824
		[SerializeField]
		private Image icon;

		// Token: 0x040012D9 RID: 4825
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040012DA RID: 4826
		[SerializeField]
		private TextMeshProUGUI descriptionText;

		// Token: 0x040012DB RID: 4827
		[SerializeField]
		private CostDisplay costDisplay;

		// Token: 0x040012DC RID: 4828
		[SerializeField]
		private LongPressButton recycleButton;

		// Token: 0x040012DD RID: 4829
		[SerializeField]
		private TextMeshProUGUI amountText;

		// Token: 0x040012DE RID: 4830
		[SerializeField]
		[LocalizationKey("Default")]
		private string tokenFormatKey;

		// Token: 0x040012DF RID: 4831
		[SerializeField]
		private TextMeshProUGUI tokenText;

		// Token: 0x040012E0 RID: 4832
		[SerializeField]
		private GameObject reachedAmountLimitationIndicator;

		// Token: 0x040012E1 RID: 4833
		[SerializeField]
		private Image backGround;

		// Token: 0x040012E2 RID: 4834
		[SerializeField]
		private Color normalColor;

		// Token: 0x040012E3 RID: 4835
		[SerializeField]
		private Color avaliableColor;

		// Token: 0x040012E4 RID: 4836
		private BuildingInfo info;
	}
}
