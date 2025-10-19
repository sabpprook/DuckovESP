using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.Animations;
using Duckov.Utilities;
using ItemStatsSystem;
using LeTai.TrueShadow;
using SodaCraft.Localizations;
using SodaCraft.StringUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov
{
	// Token: 0x0200023C RID: 572
	public class ItemUnlockNotification : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060011BC RID: 4540 RVA: 0x0004425D File Offset: 0x0004245D
		public string MainTextFormat
		{
			get
			{
				return this.mainTextFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060011BD RID: 4541 RVA: 0x0004426A File Offset: 0x0004246A
		private string SubTextFormat
		{
			get
			{
				return this.subTextFormatKey.ToPlainText();
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060011BE RID: 4542 RVA: 0x00044277 File Offset: 0x00042477
		// (set) Token: 0x060011BF RID: 4543 RVA: 0x0004427E File Offset: 0x0004247E
		public static ItemUnlockNotification Instance { get; private set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x00044286 File Offset: 0x00042486
		private bool showing
		{
			get
			{
				return this.showingTask.Status == UniTaskStatus.Pending;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060011C1 RID: 4545 RVA: 0x00044296 File Offset: 0x00042496
		public static bool Showing
		{
			get
			{
				return !(ItemUnlockNotification.Instance == null) && ItemUnlockNotification.Instance.showing;
			}
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x000442B1 File Offset: 0x000424B1
		private void Awake()
		{
			if (ItemUnlockNotification.Instance == null)
			{
				ItemUnlockNotification.Instance = this;
			}
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x000442C6 File Offset: 0x000424C6
		private void Update()
		{
			if (!this.showing && ItemUnlockNotification.pending.Count > 0)
			{
				this.BeginShow();
			}
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x000442E3 File Offset: 0x000424E3
		private void BeginShow()
		{
			this.showingTask = this.ShowTask();
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x000442F4 File Offset: 0x000424F4
		private async UniTask ShowTask()
		{
			await this.mainFadeGroup.ShowAndReturnTask();
			await UniTask.WaitForSeconds(this.contentDelay, true, PlayerLoopTiming.Update, default(CancellationToken), false);
			while (ItemUnlockNotification.pending.Count > 0)
			{
				int num = ItemUnlockNotification.pending[0];
				ItemUnlockNotification.pending.RemoveAt(0);
				await this.DisplayContent(num);
			}
			await this.mainFadeGroup.HideAndReturnTask();
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x00044338 File Offset: 0x00042538
		private async UniTask DisplayContent(int itemTypeID)
		{
			this.Setup(itemTypeID);
			await this.contentFadeGroup.ShowAndReturnTask();
			this.pointerClicked = false;
			while (!this.pointerClicked)
			{
				await UniTask.NextFrame();
			}
			await this.contentFadeGroup.HideAndReturnTask();
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00044384 File Offset: 0x00042584
		private void Setup(int itemTypeID)
		{
			ItemMetaData metaData = ItemAssetsCollection.GetMetaData(itemTypeID);
			string displayName = metaData.DisplayName;
			Sprite icon = metaData.icon;
			this.image.sprite = icon;
			this.textMain.text = this.MainTextFormat.Format(new
			{
				itemDisplayName = displayName
			});
			this.textSub.text = this.SubTextFormat;
			DisplayQuality displayQuality = metaData.displayQuality;
			GameplayDataSettings.UIStyle.GetDisplayQualityLook(displayQuality).Apply(this.shadow);
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x000443FD File Offset: 0x000425FD
		public void OnPointerClick(PointerEventData eventData)
		{
			this.pointerClicked = true;
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x00044406 File Offset: 0x00042606
		public static void Push(int itemTypeID)
		{
			ItemUnlockNotification.pending.Add(itemTypeID);
		}

		// Token: 0x04000DA9 RID: 3497
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x04000DAA RID: 3498
		[SerializeField]
		private FadeGroup contentFadeGroup;

		// Token: 0x04000DAB RID: 3499
		[SerializeField]
		private Image image;

		// Token: 0x04000DAC RID: 3500
		[SerializeField]
		private TrueShadow shadow;

		// Token: 0x04000DAD RID: 3501
		[SerializeField]
		private TextMeshProUGUI textMain;

		// Token: 0x04000DAE RID: 3502
		[SerializeField]
		private TextMeshProUGUI textSub;

		// Token: 0x04000DAF RID: 3503
		[SerializeField]
		private float contentDelay = 0.5f;

		// Token: 0x04000DB0 RID: 3504
		[SerializeField]
		[LocalizationKey("Default")]
		private string mainTextFormatKey = "UI_ItemUnlockNotification";

		// Token: 0x04000DB1 RID: 3505
		[SerializeField]
		[LocalizationKey("Default")]
		private string subTextFormatKey = "UI_ItemUnlockNotification_Sub";

		// Token: 0x04000DB2 RID: 3506
		private static List<int> pending = new List<int>();

		// Token: 0x04000DB4 RID: 3508
		private UniTask showingTask;

		// Token: 0x04000DB5 RID: 3509
		private bool pointerClicked;
	}
}
