using System;
using Cysharp.Threading.Tasks;
using Duckov.Rules;
using Duckov.UI.Animations;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI.MainMenu
{
	// Token: 0x020003E8 RID: 1000
	public class SavesButton : MonoBehaviour
	{
		// Token: 0x0600241C RID: 9244 RVA: 0x0007D978 File Offset: 0x0007BB78
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClick));
			SavesSystem.OnSetFile += this.Refresh;
			LocalizationManager.OnSetLanguage += this.OnSetLanguage;
			SavesSystem.OnSaveDeleted += this.Refresh;
		}

		// Token: 0x0600241D RID: 9245 RVA: 0x0007D9D4 File Offset: 0x0007BBD4
		private void OnDestroy()
		{
			SavesSystem.OnSetFile -= this.Refresh;
			LocalizationManager.OnSetLanguage -= this.OnSetLanguage;
			SavesSystem.OnSaveDeleted -= this.Refresh;
		}

		// Token: 0x0600241E RID: 9246 RVA: 0x0007DA09 File Offset: 0x0007BC09
		private void OnSetLanguage(SystemLanguage language)
		{
			this.Refresh();
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x0007DA11 File Offset: 0x0007BC11
		private void OnButtonClick()
		{
			if (!this.executing)
			{
				this.SavesSelectionTask().Forget();
			}
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x0007DA28 File Offset: 0x0007BC28
		private async UniTask SavesSelectionTask()
		{
			this.executing = true;
			this.currentMenuFadeGroup.Hide();
			await this.selectionMenu.Execute();
			this.currentMenuFadeGroup.Show();
			this.executing = false;
		}

		// Token: 0x06002421 RID: 9249 RVA: 0x0007DA6B File Offset: 0x0007BC6B
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x06002422 RID: 9250 RVA: 0x0007DA74 File Offset: 0x0007BC74
		private void Refresh()
		{
			bool flag = SavesSystem.IsOldGame();
			string text = (flag ? GameRulesManager.Current.DisplayName : "");
			this.text.text = this.textFormat.Format(new
			{
				text = this.textKey.ToPlainText(),
				slotNumber = SavesSystem.CurrentSlot,
				difficulty = text
			});
			bool flag2 = flag && SavesSystem.IsOldSave(SavesSystem.CurrentSlot);
			this.oldSaveIndicator.SetActive(flag2);
		}

		// Token: 0x0400188D RID: 6285
		[SerializeField]
		private FadeGroup currentMenuFadeGroup;

		// Token: 0x0400188E RID: 6286
		[SerializeField]
		private SaveSlotSelectionMenu selectionMenu;

		// Token: 0x0400188F RID: 6287
		[SerializeField]
		private GameObject oldSaveIndicator;

		// Token: 0x04001890 RID: 6288
		[SerializeField]
		private Button button;

		// Token: 0x04001891 RID: 6289
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001892 RID: 6290
		[SerializeField]
		[LocalizationKey("Default")]
		private string textKey = "MainMenu_SaveSlot";

		// Token: 0x04001893 RID: 6291
		[SerializeField]
		private string textFormat = "{text}: {slotNumber}";

		// Token: 0x04001894 RID: 6292
		private bool executing;
	}
}
