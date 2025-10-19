using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Economy;
using Duckov.Scenes;
using Duckov.UI.Animations;
using Eflatun.SceneReference;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Duckov.UI
{
	// Token: 0x020003B5 RID: 949
	public class MapSelectionView : View
	{
		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06002262 RID: 8802 RVA: 0x00078437 File Offset: 0x00076637
		public static MapSelectionView Instance
		{
			get
			{
				return View.GetViewInstance<MapSelectionView>();
			}
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x0007843E File Offset: 0x0007663E
		protected override void Awake()
		{
			base.Awake();
			this.btnConfirm.onClick.AddListener(delegate
			{
				this.confirmButtonClicked = true;
			});
			this.btnCancel.onClick.AddListener(delegate
			{
				this.cancelButtonClicked = true;
			});
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x0007847E File Offset: 0x0007667E
		protected override void OnOpen()
		{
			base.OnOpen();
			this.confirmIndicatorFadeGroup.SkipHide();
			this.mainFadeGroup.Show();
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x0007849C File Offset: 0x0007669C
		protected override void OnClose()
		{
			base.OnClose();
			this.mainFadeGroup.Hide();
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x000784B0 File Offset: 0x000766B0
		internal void NotifyEntryClicked(MapSelectionEntry mapSelectionEntry, PointerEventData eventData)
		{
			if (!mapSelectionEntry.Cost.Enough)
			{
				return;
			}
			AudioManager.Post(this.sfx_EntryClicked);
			string sceneID = mapSelectionEntry.SceneID;
			LevelManager.loadLevelBeaconIndex = mapSelectionEntry.BeaconIndex;
			this.loading = true;
			this.LoadTask(sceneID, mapSelectionEntry.Cost).Forget();
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x00078508 File Offset: 0x00076708
		private async UniTask LoadTask(string sceneID, Cost cost)
		{
			this.btnCancel.gameObject.SetActive(true);
			this.btnConfirm.gameObject.SetActive(false);
			SceneInfoEntry sceneInfo = SceneInfoCollection.GetSceneInfo(sceneID);
			this.SetupSceneInfo(sceneInfo);
			this.confirmCostDisplay.Setup(cost, 1);
			this.confirmCostDisplay.gameObject.SetActive(!cost.IsFree);
			AudioManager.Post(this.sfx_ShowDestination);
			await this.confirmIndicatorFadeGroup.ShowAndReturnTask();
			this.btnConfirm.gameObject.SetActive(true);
			bool flag = await this.WaitForConfirm();
			this.btnCancel.gameObject.SetActive(false);
			this.btnConfirm.gameObject.SetActive(false);
			if (flag && cost.Enough)
			{
				cost.Pay(true, true);
				this.confirmColorPunch.Punch();
				AudioManager.Post(this.sfx_ConfirmDestination);
				await UniTask.WaitForSeconds(0.5f, true, PlayerLoopTiming.Update, default(CancellationToken), false);
				SceneLoader.Instance.LoadScene(sceneID, this.overrideLoadingScreen, true, false, true, false, default(MultiSceneLocation), true, false).Forget();
			}
			else
			{
				this.confirmIndicatorFadeGroup.Hide();
			}
			this.loading = false;
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x0007855C File Offset: 0x0007675C
		private async UniTask<bool> WaitForConfirm()
		{
			this.confirmButtonClicked = false;
			this.cancelButtonClicked = false;
			while (!this.cancelButtonClicked)
			{
				if (this.confirmButtonClicked)
				{
					return 1;
				}
				await UniTask.Yield();
			}
			return 0;
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x000785A0 File Offset: 0x000767A0
		private void SetupSceneInfo(SceneInfoEntry info)
		{
			if (info == null)
			{
				return;
			}
			string displayName = info.DisplayName;
			this.destinationDisplayNameText.text = displayName;
			this.destinationDisplayNameText.color = Color.white;
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x000785D4 File Offset: 0x000767D4
		internal override void TryQuit()
		{
			if (!this.loading)
			{
				base.Close();
			}
		}

		// Token: 0x0400175B RID: 5979
		[SerializeField]
		private FadeGroup mainFadeGroup;

		// Token: 0x0400175C RID: 5980
		[SerializeField]
		private FadeGroup confirmIndicatorFadeGroup;

		// Token: 0x0400175D RID: 5981
		[SerializeField]
		private TextMeshProUGUI destinationDisplayNameText;

		// Token: 0x0400175E RID: 5982
		[SerializeField]
		private CostDisplay confirmCostDisplay;

		// Token: 0x0400175F RID: 5983
		private string sfx_EntryClicked = "UI/confirm";

		// Token: 0x04001760 RID: 5984
		private string sfx_ShowDestination = "UI/destination_show";

		// Token: 0x04001761 RID: 5985
		private string sfx_ConfirmDestination = "UI/destination_confirm";

		// Token: 0x04001762 RID: 5986
		[SerializeField]
		private ColorPunch confirmColorPunch;

		// Token: 0x04001763 RID: 5987
		[SerializeField]
		private Button btnConfirm;

		// Token: 0x04001764 RID: 5988
		[SerializeField]
		private Button btnCancel;

		// Token: 0x04001765 RID: 5989
		[SerializeField]
		private SceneReference overrideLoadingScreen;

		// Token: 0x04001766 RID: 5990
		private bool loading;

		// Token: 0x04001767 RID: 5991
		private bool confirmButtonClicked;

		// Token: 0x04001768 RID: 5992
		private bool cancelButtonClicked;
	}
}
