using System;
using Duckov.Rules;
using Saves;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI.MainMenu
{
	// Token: 0x020003E9 RID: 1001
	public class SaveSlotSelectionButton : MonoBehaviour
	{
		// Token: 0x06002424 RID: 9252 RVA: 0x0007DB01 File Offset: 0x0007BD01
		private void Awake()
		{
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClick));
		}

		// Token: 0x06002425 RID: 9253 RVA: 0x0007DB1F File Offset: 0x0007BD1F
		private void OnDestroy()
		{
		}

		// Token: 0x06002426 RID: 9254 RVA: 0x0007DB21 File Offset: 0x0007BD21
		private void OnEnable()
		{
			SavesSystem.OnSetFile += this.Refresh;
			this.Refresh();
		}

		// Token: 0x06002427 RID: 9255 RVA: 0x0007DB3A File Offset: 0x0007BD3A
		private void OnDisable()
		{
			SavesSystem.OnSetFile -= this.Refresh;
		}

		// Token: 0x06002428 RID: 9256 RVA: 0x0007DB4D File Offset: 0x0007BD4D
		private void OnButtonClick()
		{
			SavesSystem.SetFile(this.index);
			this.menu.Finish();
		}

		// Token: 0x06002429 RID: 9257 RVA: 0x0007DB65 File Offset: 0x0007BD65
		private void OnValidate()
		{
			if (this.button == null)
			{
				this.button = base.GetComponent<Button>();
			}
			if (this.text == null)
			{
				this.text = base.GetComponentInChildren<TextMeshProUGUI>();
			}
			this.Refresh();
		}

		// Token: 0x0600242A RID: 9258 RVA: 0x0007DBA4 File Offset: 0x0007BDA4
		private void Refresh()
		{
			new ES3Settings(SavesSystem.GetFilePath(this.index), null).location = ES3.Location.File;
			this.text.text = this.format.Format(new
			{
				slotText = this.slotTextKey.ToPlainText(),
				index = this.index
			});
			bool flag = SavesSystem.CurrentSlot == this.index;
			GameObject gameObject = this.activeIndicator;
			if (gameObject != null)
			{
				gameObject.SetActive(flag);
			}
			if (SavesSystem.IsOldGame(this.index))
			{
				this.difficultyText.text = GameRulesManager.GetRuleIndexDisplayNameOfSlot(this.index) ?? "";
				this.playTimeText.gameObject.SetActive(true);
				TimeSpan realTimePlayedOfSaveSlot = GameClock.GetRealTimePlayedOfSaveSlot(this.index);
				this.playTimeText.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)realTimePlayedOfSaveSlot.TotalHours), realTimePlayedOfSaveSlot.Minutes);
				bool flag2 = SavesSystem.IsOldSave(this.index);
				this.oldSlotIndicator.SetActive(flag2);
				long num = SavesSystem.Load<long>("SaveTime", this.index);
				string text = ((num > 0L) ? DateTime.FromBinary(num).ToLocalTime().ToString("yyyy/MM/dd HH:mm") : "???");
				this.saveTimeText.text = text;
				return;
			}
			this.difficultyText.text = this.newGameTextKey.ToPlainText();
			this.playTimeText.gameObject.SetActive(false);
			this.oldSlotIndicator.SetActive(false);
			this.saveTimeText.text = "----/--/-- --:--";
		}

		// Token: 0x04001895 RID: 6293
		[SerializeField]
		private SaveSlotSelectionMenu menu;

		// Token: 0x04001896 RID: 6294
		[SerializeField]
		private Button button;

		// Token: 0x04001897 RID: 6295
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04001898 RID: 6296
		[SerializeField]
		private TextMeshProUGUI difficultyText;

		// Token: 0x04001899 RID: 6297
		[SerializeField]
		private TextMeshProUGUI playTimeText;

		// Token: 0x0400189A RID: 6298
		[SerializeField]
		private TextMeshProUGUI saveTimeText;

		// Token: 0x0400189B RID: 6299
		[SerializeField]
		private string slotTextKey = "MainMenu_SaveSelection_Slot";

		// Token: 0x0400189C RID: 6300
		[SerializeField]
		private string format = "{slotText} {index}";

		// Token: 0x0400189D RID: 6301
		[LocalizationKey("Default")]
		[SerializeField]
		private string newGameTextKey = "NewGame";

		// Token: 0x0400189E RID: 6302
		[SerializeField]
		private GameObject activeIndicator;

		// Token: 0x0400189F RID: 6303
		[SerializeField]
		private GameObject oldSlotIndicator;

		// Token: 0x040018A0 RID: 6304
		[Min(1f)]
		[SerializeField]
		private int index;
	}
}
