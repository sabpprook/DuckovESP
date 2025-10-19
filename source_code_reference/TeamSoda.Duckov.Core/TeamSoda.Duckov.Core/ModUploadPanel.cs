using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Modding;
using Duckov.UI.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class ModUploadPanel : MonoBehaviour
{
	// Token: 0x06000D67 RID: 3431 RVA: 0x000374E7 File Offset: 0x000356E7
	private void Awake()
	{
		this.btnCancel.onClick.AddListener(new UnityAction(this.OnCancelBtnClick));
		this.btnUpload.onClick.AddListener(new UnityAction(this.OnUploadBtnClick));
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00037521 File Offset: 0x00035721
	private void OnUploadBtnClick()
	{
		this.uploadClicked = true;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x0003752A File Offset: 0x0003572A
	private void OnCancelBtnClick()
	{
		this.cancelClicked = true;
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00037534 File Offset: 0x00035734
	public async UniTask Execute(ModInfo info)
	{
		string path = info.path;
		SteamWorkshopManager workshopManager = SteamWorkshopManager.Instance;
		if (workshopManager == null || !SteamManager.Initialized)
		{
			Debug.LogError("Cannot execute uplaod panel. SteamWorkshopManager and SteamManager are required.");
		}
		this.Clean();
		this.fgMain.Show();
		this.fgLoading.Show();
		bool flag = ModManager.TryProcessModFolder(path, out info, false, 0UL);
		this.txtPath.text = path.Replace('\\', '/');
		this.btnUpload.gameObject.SetActive(flag);
		if (flag)
		{
			this.txtTitle.text = info.displayName;
			this.txtDescription.text = info.description;
			this.txtPublishedFileID.text = ((info.publishedFileId > 0UL) ? info.publishedFileId.ToString() : "-");
			this.txtModName.text = info.name;
			this.preview.texture = info.preview;
		}
		else
		{
			this.txtTitle.text = "???";
			this.txtDescription.text = "???";
			this.txtPublishedFileID.text = "???";
			this.txtModName.text = "???";
			this.preview.texture = this.defaultPreviewTexture;
		}
		bool flag2 = flag && info.publishedFileId == 0UL;
		bool flag3 = SteamWorkshopManager.IsOwner(info);
		this.indicatorNew.SetActive(flag2);
		this.indicatorUpdate.SetActive(!flag2);
		this.indicatorOwnershipWarning.SetActive(!flag3);
		this.indicatorInvalidContent.SetActive(!flag);
		await this.fgLoading.HideAndReturnTask();
		this.fgContent.Show();
		this.fgButtonMain.Show();
		this.cancelClicked = false;
		this.uploadClicked = false;
		while (!this.cancelClicked && !this.uploadClicked)
		{
			await UniTask.Yield();
		}
		if (this.cancelClicked)
		{
			this.fgMain.Hide();
		}
		else
		{
			this.fgButtonMain.Hide();
			this.fgProgressBar.Show();
			this.waitingForUpload = true;
			bool flag4 = await workshopManager.UploadWorkshopItem(path, "");
			this.waitingForUpload = false;
			this.fgProgressBar.Hide();
			if (flag4)
			{
				ModInfo modInfo;
				if (ModManager.TryProcessModFolder(path, out modInfo, false, 0UL))
				{
					this.txtPublishedFileID.text = string.Format("{0}", modInfo.publishedFileId);
				}
				this.fgSucceed.Show();
			}
			else
			{
				this.fgFailed.Show();
			}
			await UniTask.WaitForSeconds(this.closeAfterSeconds, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.fgMain.Hide();
		}
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00037580 File Offset: 0x00035780
	private void Update()
	{
		if (this.waitingForUpload)
		{
			this.progressBarFill.fillAmount = SteamWorkshopManager.UploadingProgress;
			ulong punBytesProcess = SteamWorkshopManager.punBytesProcess;
			ulong punBytesTotal = SteamWorkshopManager.punBytesTotal;
			this.progressText.text = ModUploadPanel.FormatBytes(punBytesProcess) + " / " + ModUploadPanel.FormatBytes(punBytesTotal);
		}
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000375D4 File Offset: 0x000357D4
	private static string FormatBytes(ulong bytes)
	{
		if (bytes < 1024UL)
		{
			return string.Format("{0}bytes", bytes);
		}
		if (bytes < 1048576UL)
		{
			return string.Format("{0:0.0}KB", bytes / 1024f);
		}
		if (bytes < 1073741824UL)
		{
			return string.Format("{0:0.0}MB", bytes / 1048576f);
		}
		return string.Format("{0:0.0}GB", bytes / 1.0737418E+09f);
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x00037658 File Offset: 0x00035858
	private void Clean()
	{
		this.fgLoading.SkipHide();
		this.fgContent.SkipHide();
		this.indicatorNew.SetActive(false);
		this.indicatorUpdate.SetActive(false);
		this.indicatorOwnershipWarning.SetActive(false);
		this.indicatorInvalidContent.SetActive(false);
		this.txtPublishedFileID.text = "-";
		this.txtPath.text = "-";
		this.fgButtonMain.SkipHide();
		this.fgProgressBar.SkipHide();
		this.fgSucceed.SkipHide();
		this.fgFailed.SkipHide();
		this.waitingForUpload = false;
	}

	// Token: 0x04000B64 RID: 2916
	[SerializeField]
	private FadeGroup fgMain;

	// Token: 0x04000B65 RID: 2917
	[SerializeField]
	private FadeGroup fgLoading;

	// Token: 0x04000B66 RID: 2918
	[SerializeField]
	private FadeGroup fgContent;

	// Token: 0x04000B67 RID: 2919
	[SerializeField]
	private TextMeshProUGUI txtTitle;

	// Token: 0x04000B68 RID: 2920
	[SerializeField]
	private TextMeshProUGUI txtDescription;

	// Token: 0x04000B69 RID: 2921
	[SerializeField]
	private RawImage preview;

	// Token: 0x04000B6A RID: 2922
	[SerializeField]
	private TextMeshProUGUI txtModName;

	// Token: 0x04000B6B RID: 2923
	[SerializeField]
	private TextMeshProUGUI txtPath;

	// Token: 0x04000B6C RID: 2924
	[SerializeField]
	private TextMeshProUGUI txtPublishedFileID;

	// Token: 0x04000B6D RID: 2925
	[SerializeField]
	private GameObject indicatorNew;

	// Token: 0x04000B6E RID: 2926
	[SerializeField]
	private GameObject indicatorUpdate;

	// Token: 0x04000B6F RID: 2927
	[SerializeField]
	private GameObject indicatorOwnershipWarning;

	// Token: 0x04000B70 RID: 2928
	[SerializeField]
	private GameObject indicatorInvalidContent;

	// Token: 0x04000B71 RID: 2929
	[SerializeField]
	private Button btnUpload;

	// Token: 0x04000B72 RID: 2930
	[SerializeField]
	private Button btnCancel;

	// Token: 0x04000B73 RID: 2931
	[SerializeField]
	private FadeGroup fgButtonMain;

	// Token: 0x04000B74 RID: 2932
	[SerializeField]
	private FadeGroup fgProgressBar;

	// Token: 0x04000B75 RID: 2933
	[SerializeField]
	private TextMeshProUGUI progressText;

	// Token: 0x04000B76 RID: 2934
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x04000B77 RID: 2935
	[SerializeField]
	private FadeGroup fgSucceed;

	// Token: 0x04000B78 RID: 2936
	[SerializeField]
	private FadeGroup fgFailed;

	// Token: 0x04000B79 RID: 2937
	[SerializeField]
	private float closeAfterSeconds = 2f;

	// Token: 0x04000B7A RID: 2938
	[SerializeField]
	private Texture2D defaultPreviewTexture;

	// Token: 0x04000B7B RID: 2939
	private bool cancelClicked;

	// Token: 0x04000B7C RID: 2940
	private bool uploadClicked;

	// Token: 0x04000B7D RID: 2941
	private bool waitingForUpload;
}
