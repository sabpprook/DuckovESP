using System;
using Cysharp.Threading.Tasks;
using ItemStatsSystem.Data;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003AB RID: 939
	public class StorageDockEntry : MonoBehaviour
	{
		// Token: 0x060021B9 RID: 8633 RVA: 0x0007573C File Offset: 0x0007393C
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClick));
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x0007575A File Offset: 0x0007395A
		private void OnButtonClick()
		{
			if (!PlayerStorage.IsAccessableAndNotFull())
			{
				return;
			}
			this.TakeTask().Forget();
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x00075770 File Offset: 0x00073970
		private async UniTask TakeTask()
		{
			if (!PlayerStorage.TakingItem)
			{
				this.loadingIndicator.SetActive(true);
				this.text.gameObject.SetActive(false);
				await PlayerStorage.TakeBufferItem(this.index);
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x000757B4 File Offset: 0x000739B4
		public void Setup(int index, ItemTreeData item)
		{
			this.index = index;
			this.item = item;
			ItemTreeData.DataEntry rootData = item.RootData;
			this.itemDisplay.Setup(rootData.typeID);
			int stackCount = rootData.StackCount;
			if (stackCount > 1)
			{
				this.countText.text = stackCount.ToString();
				this.countDisplay.SetActive(true);
			}
			else
			{
				this.countDisplay.SetActive(false);
			}
			if (PlayerStorage.IsAccessableAndNotFull())
			{
				this.bgImage.color = this.colorNormal;
				this.text.text = this.textKeyNormal.ToPlainText();
			}
			else
			{
				this.bgImage.color = this.colorFull;
				this.text.text = this.textKeyInventoryFull.ToPlainText();
			}
			this.text.gameObject.SetActive(true);
			this.loadingIndicator.SetActive(false);
		}

		// Token: 0x040016C7 RID: 5831
		[SerializeField]
		private ItemMetaDisplay itemDisplay;

		// Token: 0x040016C8 RID: 5832
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x040016C9 RID: 5833
		[SerializeField]
		private GameObject countDisplay;

		// Token: 0x040016CA RID: 5834
		[SerializeField]
		private TextMeshProUGUI countText;

		// Token: 0x040016CB RID: 5835
		[SerializeField]
		private Image bgImage;

		// Token: 0x040016CC RID: 5836
		[SerializeField]
		private Button button;

		// Token: 0x040016CD RID: 5837
		[SerializeField]
		private GameObject loadingIndicator;

		// Token: 0x040016CE RID: 5838
		[SerializeField]
		private Color colorNormal;

		// Token: 0x040016CF RID: 5839
		[SerializeField]
		private Color colorFull;

		// Token: 0x040016D0 RID: 5840
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKeyNormal;

		// Token: 0x040016D1 RID: 5841
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKeyInventoryFull;

		// Token: 0x040016D2 RID: 5842
		private int index;

		// Token: 0x040016D3 RID: 5843
		private ItemTreeData item;
	}
}
