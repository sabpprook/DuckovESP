using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using Sirenix.Utilities;
using Steamworks;
using UnityEngine;

namespace Duckov.Modding
{
	// Token: 0x0200026A RID: 618
	public class SteamWorkshopManager : MonoBehaviour
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x0600133B RID: 4923 RVA: 0x00047BF4 File Offset: 0x00045DF4
		// (set) Token: 0x0600133C RID: 4924 RVA: 0x00047BFB File Offset: 0x00045DFB
		public static SteamWorkshopManager Instance { get; private set; }

		// Token: 0x0600133D RID: 4925 RVA: 0x00047C03 File Offset: 0x00045E03
		private void Awake()
		{
			SteamWorkshopManager.Instance = this;
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x00047C0B File Offset: 0x00045E0B
		private void OnEnable()
		{
			ModManager.Rescan();
			this.SendQueryDetailsRequest();
			ModManager.OnScan += this.OnScanMods;
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x00047C29 File Offset: 0x00045E29
		private void OnDisable()
		{
			ModManager.OnScan -= this.OnScanMods;
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x00047C3C File Offset: 0x00045E3C
		private void OnScanMods(List<ModInfo> list)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}
			foreach (SteamUGCDetails_t steamUGCDetails_t in SteamWorkshopManager.ugcDetailsCache)
			{
				PublishedFileId_t nPublishedFileId = steamUGCDetails_t.m_nPublishedFileId;
				EItemState itemState = (EItemState)SteamUGC.GetItemState(nPublishedFileId);
				ulong num;
				string text;
				uint num2;
				if ((itemState | EItemState.k_EItemStateInstalled) == itemState && SteamUGC.GetItemInstallInfo(nPublishedFileId, out num, out text, 1024U, out num2))
				{
					ModInfo modInfo;
					if (!ModManager.TryProcessModFolder(text, out modInfo, true, nPublishedFileId.m_PublishedFileId))
					{
						Debug.LogError("Mod processing failed! \nPath:" + text);
					}
					else
					{
						list.Add(modInfo);
					}
				}
			}
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x00047CE4 File Offset: 0x00045EE4
		public void SendQueryDetailsRequest()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}
			if (this.CRSteamUGCQueryCompleted == null)
			{
				this.CRSteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(new CallResult<SteamUGCQueryCompleted_t>.APIDispatchDelegate(this.OnSteamUGCQueryCompleted));
			}
			HashSet<PublishedFileId_t> hashSet = new HashSet<PublishedFileId_t>();
			uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
			PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
			hashSet.AddRange(array);
			foreach (ModInfo modInfo in ModManager.modInfos)
			{
				if (modInfo.publishedFileId != 0UL)
				{
					hashSet.Add((PublishedFileId_t)modInfo.publishedFileId);
				}
			}
			SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(SteamUGC.CreateQueryUGCDetailsRequest(hashSet.ToArray<PublishedFileId_t>(), (uint)hashSet.Count));
			this.CRSteamUGCQueryCompleted.Set(steamAPICall_t, null);
			new StringBuilder();
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x00047DC0 File Offset: 0x00045FC0
		private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t completed, bool bIOFailure)
		{
			if (bIOFailure)
			{
				Debug.LogError("Steam UGC Query failed", base.gameObject);
				return;
			}
			UGCQueryHandle_t handle = completed.m_handle;
			uint unNumResultsReturned = completed.m_unNumResultsReturned;
			for (uint num = 0U; num < unNumResultsReturned; num += 1U)
			{
				SteamUGCDetails_t steamUGCDetails_t;
				SteamUGC.GetQueryUGCResult(handle, num, out steamUGCDetails_t);
				SteamWorkshopManager.ugcDetailsCache.Add(steamUGCDetails_t);
			}
			SteamUGC.ReleaseQueryUGCRequest(handle);
			ModManager.Instance.ScanAndActivateMods();
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x00047E24 File Offset: 0x00046024
		public async UniTask<PublishedFileId_t> RequestNewWorkshopItemID()
		{
			PublishedFileId_t publishedFileId_t;
			if (!SteamManager.Initialized)
			{
				publishedFileId_t = default(PublishedFileId_t);
			}
			else
			{
				if (this.CRCreateItemResult == null)
				{
					this.CRCreateItemResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.OnCreateItemResult));
				}
				Debug.Log("Requesting new PublishedFileId");
				this.createItemResultFired = false;
				SteamAPICall_t steamAPICall_t = SteamUGC.CreateItem((AppId_t)3167020U, EWorkshopFileType.k_EWorkshopFileTypeFirst);
				this.CRCreateItemResult.Set(steamAPICall_t, delegate(CreateItemResult_t result, bool failure)
				{
					Debug.Log("Creat Item Result Fired B");
					this.createItemResultFired = true;
					this.createItemResult = result;
				});
				while (!this.createItemResultFired)
				{
					await UniTask.Yield();
				}
				if (this.createItemResult.m_eResult != EResult.k_EResultOK)
				{
					Debug.LogError(string.Format("Failed to create workshop item.\nResult: {0}", this.createItemResult.m_eResult));
					publishedFileId_t = default(PublishedFileId_t);
				}
				else
				{
					publishedFileId_t = this.createItemResult.m_nPublishedFileId;
				}
			}
			return publishedFileId_t;
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00047E67 File Offset: 0x00046067
		private void OnCreateItemResult(CreateItemResult_t result, bool bIOFailure)
		{
			Debug.Log("Creat Item Result Fired A");
			this.createItemResultFired = true;
			this.createItemResult = result;
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06001345 RID: 4933 RVA: 0x00047E81 File Offset: 0x00046081
		// (set) Token: 0x06001346 RID: 4934 RVA: 0x00047E88 File Offset: 0x00046088
		public static ulong punBytesProcess { get; private set; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x00047E90 File Offset: 0x00046090
		// (set) Token: 0x06001348 RID: 4936 RVA: 0x00047E97 File Offset: 0x00046097
		public static ulong punBytesTotal { get; private set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x00047E9F File Offset: 0x0004609F
		public static float UploadingProgress
		{
			get
			{
				return (float)(SteamWorkshopManager.punBytesProcess / SteamWorkshopManager.punBytesTotal);
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x00047EB1 File Offset: 0x000460B1
		// (set) Token: 0x0600134B RID: 4939 RVA: 0x00047EB9 File Offset: 0x000460B9
		public bool UploadSucceed { get; private set; }

		// Token: 0x0600134C RID: 4940 RVA: 0x00047EC4 File Offset: 0x000460C4
		public UniTask<bool> UploadWorkshopItem(string path, string changeNote = "Unknown")
		{
			SteamWorkshopManager.<UploadWorkshopItem>d__32 <UploadWorkshopItem>d__;
			<UploadWorkshopItem>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
			<UploadWorkshopItem>d__.<>4__this = this;
			<UploadWorkshopItem>d__.path = path;
			<UploadWorkshopItem>d__.changeNote = changeNote;
			<UploadWorkshopItem>d__.<>1__state = -1;
			<UploadWorkshopItem>d__.<>t__builder.Start<SteamWorkshopManager.<UploadWorkshopItem>d__32>(ref <UploadWorkshopItem>d__);
			return <UploadWorkshopItem>d__.<>t__builder.Task;
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x00047F18 File Offset: 0x00046118
		public static bool IsOwner(ModInfo info)
		{
			if (!SteamManager.Initialized)
			{
				return false;
			}
			if (info.publishedFileId == 0UL)
			{
				return false;
			}
			foreach (SteamUGCDetails_t steamUGCDetails_t in SteamWorkshopManager.ugcDetailsCache)
			{
				if (steamUGCDetails_t.m_nPublishedFileId.m_PublishedFileId == info.publishedFileId)
				{
					return steamUGCDetails_t.m_ulSteamIDOwner == SteamUser.GetSteamID().m_SteamID;
				}
			}
			return false;
		}

		// Token: 0x04000E55 RID: 3669
		private CallResult<SteamUGCQueryCompleted_t> CRSteamUGCQueryCompleted;

		// Token: 0x04000E56 RID: 3670
		private CallResult<CreateItemResult_t> CRCreateItemResult;

		// Token: 0x04000E57 RID: 3671
		private UGCQueryHandle_t activeQueryHandle;

		// Token: 0x04000E58 RID: 3672
		private static List<SteamUGCDetails_t> ugcDetailsCache = new List<SteamUGCDetails_t>();

		// Token: 0x04000E59 RID: 3673
		private bool createItemResultFired;

		// Token: 0x04000E5A RID: 3674
		private CreateItemResult_t createItemResult;
	}
}
