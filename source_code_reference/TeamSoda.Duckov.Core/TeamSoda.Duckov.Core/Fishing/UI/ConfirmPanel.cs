using System;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using Duckov.UI.Animations;
using ItemStatsSystem;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fishing.UI
{
	// Token: 0x02000216 RID: 534
	public class ConfirmPanel : MonoBehaviour
	{
		// Token: 0x06000FED RID: 4077 RVA: 0x0003E438 File Offset: 0x0003C638
		private void Awake()
		{
			this.continueButton.onClick.AddListener(new UnityAction(this.OnContinueButtonClicked));
			this.quitButton.onClick.AddListener(new UnityAction(this.OnQuitButtonClicked));
			this.itemDisplay.onPointerClick += this.OnItemDisplayClick;
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x0003E494 File Offset: 0x0003C694
		private void OnItemDisplayClick(ItemDisplay display, PointerEventData data)
		{
			data.Use();
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x0003E49C File Offset: 0x0003C69C
		private void OnContinueButtonClicked()
		{
			this.confirmed = true;
			this.continueFishing = true;
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x0003E4AC File Offset: 0x0003C6AC
		private void OnQuitButtonClicked()
		{
			this.confirmed = true;
			this.continueFishing = false;
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x0003E4BC File Offset: 0x0003C6BC
		internal async UniTask DoConfirmDialogue(Item catchedItem, Action<bool> confirmCallback)
		{
			this.Setup(catchedItem);
			this.fadeGroup.Show();
			this.confirmed = false;
			while (base.gameObject.activeInHierarchy && !this.confirmed)
			{
				await UniTask.Yield();
			}
			confirmCallback(this.continueFishing);
			this.fadeGroup.Hide();
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x0003E510 File Offset: 0x0003C710
		private void Setup(Item item)
		{
			if (item == null)
			{
				this.titleText.text = this.failedTextKey.ToPlainText();
				this.itemDisplay.gameObject.SetActive(false);
				return;
			}
			this.titleText.text = this.succeedTextKey.ToPlainText();
			this.itemDisplay.Setup(item);
			this.itemDisplay.gameObject.SetActive(true);
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x0003E581 File Offset: 0x0003C781
		internal void NotifyStop()
		{
			this.fadeGroup.Hide();
		}

		// Token: 0x04000CC6 RID: 3270
		[SerializeField]
		private FadeGroup fadeGroup;

		// Token: 0x04000CC7 RID: 3271
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04000CC8 RID: 3272
		[SerializeField]
		[LocalizationKey("Default")]
		private string succeedTextKey = "Fishing_Succeed";

		// Token: 0x04000CC9 RID: 3273
		[SerializeField]
		[LocalizationKey("Default")]
		private string failedTextKey = "Fishing_Failed";

		// Token: 0x04000CCA RID: 3274
		[SerializeField]
		private ItemDisplay itemDisplay;

		// Token: 0x04000CCB RID: 3275
		[SerializeField]
		private Button continueButton;

		// Token: 0x04000CCC RID: 3276
		[SerializeField]
		private Button quitButton;

		// Token: 0x04000CCD RID: 3277
		private bool confirmed;

		// Token: 0x04000CCE RID: 3278
		private bool continueFishing;
	}
}
