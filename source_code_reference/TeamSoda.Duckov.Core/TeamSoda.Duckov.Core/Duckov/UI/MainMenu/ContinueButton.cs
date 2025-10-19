using System;
using Cysharp.Threading.Tasks;
using Duckov.Scenes;
using Duckov.Utilities;
using Eflatun.SceneReference;
using Saves;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Duckov.UI.MainMenu
{
	// Token: 0x020003E7 RID: 999
	public class ContinueButton : MonoBehaviour
	{
		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06002413 RID: 9235 RVA: 0x0007D7F5 File Offset: 0x0007B9F5
		[SerializeField]
		private string Text_NewGame
		{
			get
			{
				return this.text_NewGame.ToPlainText();
			}
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06002414 RID: 9236 RVA: 0x0007D802 File Offset: 0x0007BA02
		[SerializeField]
		private string Text_Continue
		{
			get
			{
				return this.text_Continue.ToPlainText();
			}
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x0007D810 File Offset: 0x0007BA10
		private void Awake()
		{
			SavesSystem.OnSetFile += this.Refresh;
			SavesSystem.OnSaveDeleted += this.Refresh;
			this.button.onClick.AddListener(new UnityAction(this.OnButtonClicked));
			LocalizationManager.OnSetLanguage += this.OnSetLanguage;
		}

		// Token: 0x06002416 RID: 9238 RVA: 0x0007D86C File Offset: 0x0007BA6C
		private void OnDestroy()
		{
			SavesSystem.OnSetFile -= this.Refresh;
			SavesSystem.OnSaveDeleted -= this.Refresh;
			LocalizationManager.OnSetLanguage -= this.OnSetLanguage;
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x0007D8A1 File Offset: 0x0007BAA1
		private void OnSetLanguage(SystemLanguage language)
		{
			this.Refresh();
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x0007D8AC File Offset: 0x0007BAAC
		private void OnButtonClicked()
		{
			GameManager.newBoot = true;
			if (MultiSceneCore.GetVisited("Base"))
			{
				SceneLoader.Instance.LoadBaseScene(null, true).Forget();
				return;
			}
			SavesSystem.Save<VersionData>("CreatedWithVersion", GameMetaData.Instance.Version);
			SceneLoader.Instance.LoadScene(GameplayDataSettings.SceneManagement.PrologueScene, this.overrideCurtainScene, false, false, true, false, default(MultiSceneLocation), true, false).Forget();
		}

		// Token: 0x06002419 RID: 9241 RVA: 0x0007D91F File Offset: 0x0007BB1F
		private void Start()
		{
			this.Refresh();
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x0007D928 File Offset: 0x0007BB28
		private void Refresh()
		{
			bool flag = SavesSystem.IsOldGame();
			this.text.text = (flag ? this.Text_Continue : this.Text_NewGame);
		}

		// Token: 0x04001888 RID: 6280
		[SerializeField]
		private Button button;

		// Token: 0x04001889 RID: 6281
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x0400188A RID: 6282
		[LocalizationKey("Default")]
		[SerializeField]
		private string text_NewGame = "新游戏";

		// Token: 0x0400188B RID: 6283
		[LocalizationKey("Default")]
		[SerializeField]
		private string text_Continue = "继续";

		// Token: 0x0400188C RID: 6284
		[SerializeField]
		private SceneReference overrideCurtainScene;
	}
}
