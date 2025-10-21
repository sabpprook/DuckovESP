using System;
using Duckov;
using SodaCraft.Localizations;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000162 RID: 354
public class AddToWishListButton : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000AC8 RID: 2760 RVA: 0x0002E897 File Offset: 0x0002CA97
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			AddToWishListButton.ShowPage();
		}
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0002E8A8 File Offset: 0x0002CAA8
	public static void ShowPage()
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.ActivateGameOverlayToStore(new AppId_t(3167020U), EOverlayToStoreFlag.k_EOverlayToStoreFlag_None);
			return;
		}
		if (GameMetaData.Instance.Platform == Platform.Steam)
		{
			Application.OpenURL("https://store.steampowered.com/app/3167020/");
			return;
		}
		if (LocalizationManager.CurrentLanguage == SystemLanguage.ChineseSimplified)
		{
			Application.OpenURL("https://game.bilibili.com/duckov/");
			return;
		}
		Application.OpenURL("https://www.duckov.com");
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0002E903 File Offset: 0x0002CB03
	private void Start()
	{
		if (!SteamManager.Initialized)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000953 RID: 2387
	private const string url = "https://store.steampowered.com/app/3167020/";

	// Token: 0x04000954 RID: 2388
	private const string CNUrl = "https://game.bilibili.com/duckov/";

	// Token: 0x04000955 RID: 2389
	private const string ENUrl = "https://www.duckov.com";

	// Token: 0x04000956 RID: 2390
	private const uint appid = 3167020U;
}
