using System;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using FX;
using ItemStatsSystem;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class Action_Fishing : CharacterActionBase
{
	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06000551 RID: 1361 RVA: 0x00017C8C File Offset: 0x00015E8C
	// (remove) Token: 0x06000552 RID: 1362 RVA: 0x00017CC0 File Offset: 0x00015EC0
	public static event Action<Action_Fishing, ICollection<Item>, Func<Item, bool>> OnPlayerStartSelectBait;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000553 RID: 1363 RVA: 0x00017CF4 File Offset: 0x00015EF4
	// (remove) Token: 0x06000554 RID: 1364 RVA: 0x00017D28 File Offset: 0x00015F28
	public static event Action<Action_Fishing> OnPlayerStartFishing;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000555 RID: 1365 RVA: 0x00017D5C File Offset: 0x00015F5C
	// (remove) Token: 0x06000556 RID: 1366 RVA: 0x00017D90 File Offset: 0x00015F90
	public static event Action<Action_Fishing, float, Func<float>> OnPlayerStartCatching;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000557 RID: 1367 RVA: 0x00017DC4 File Offset: 0x00015FC4
	// (remove) Token: 0x06000558 RID: 1368 RVA: 0x00017DF8 File Offset: 0x00015FF8
	public static event Action<Action_Fishing, Item, Action<bool>> OnPlayerStopCatching;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000559 RID: 1369 RVA: 0x00017E2C File Offset: 0x0001602C
	// (remove) Token: 0x0600055A RID: 1370 RVA: 0x00017E60 File Offset: 0x00016060
	public static event Action<Action_Fishing> OnPlayerStopFishing;

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x0600055B RID: 1371 RVA: 0x00017E93 File Offset: 0x00016093
	public Action_Fishing.FishingStates FishingState
	{
		get
		{
			return this.fishingState;
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x00017E9B File Offset: 0x0001609B
	private void Awake()
	{
		this.fishingCamera.gameObject.SetActive(false);
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x00017EAE File Offset: 0x000160AE
	public override bool CanEditInventory()
	{
		return false;
	}

	// Token: 0x0600055E RID: 1374 RVA: 0x00017EB1 File Offset: 0x000160B1
	public override CharacterActionBase.ActionPriorities ActionPriority()
	{
		return CharacterActionBase.ActionPriorities.Fishing;
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00017EB4 File Offset: 0x000160B4
	protected override bool OnStart()
	{
		if (!this.characterController)
		{
			return false;
		}
		this.fishingCamera.gameObject.SetActive(true);
		this.fishingRod = this.characterController.CurrentHoldItemAgent.GetComponent<FishingRod>();
		bool flag = this.fishingRod != null;
		this.currentTask = this.Fishing();
		InputManager.OnInteractButtonDown = (Action)Delegate.Remove(InputManager.OnInteractButtonDown, new Action(this.OnCatchButton));
		InputManager.OnInteractButtonDown = (Action)Delegate.Combine(InputManager.OnInteractButtonDown, new Action(this.OnCatchButton));
		UIInputManager.OnCancel -= this.UIOnCancle;
		UIInputManager.OnCancel += this.UIOnCancle;
		return flag;
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00017F71 File Offset: 0x00016171
	private void OnCatchButton()
	{
		if (this.fishingState != Action_Fishing.FishingStates.catching)
		{
			return;
		}
		this.catchInput = true;
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x00017F84 File Offset: 0x00016184
	private void UIOnCancle(UIInputEventData data)
	{
		data.Use();
		this.Quit();
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00017F94 File Offset: 0x00016194
	protected override void OnStop()
	{
		base.OnStop();
		this.fishingState = Action_Fishing.FishingStates.notStarted;
		Action<Action_Fishing> onPlayerStopFishing = Action_Fishing.OnPlayerStopFishing;
		if (onPlayerStopFishing != null)
		{
			onPlayerStopFishing(this);
		}
		InputManager.OnInteractButtonDown = (Action)Delegate.Remove(InputManager.OnInteractButtonDown, new Action(this.OnCatchButton));
		UIInputManager.OnCancel -= this.UIOnCancle;
		this.fishingCamera.gameObject.SetActive(false);
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x00018001 File Offset: 0x00016201
	public override bool CanControlAim()
	{
		return false;
	}

	// Token: 0x06000564 RID: 1380 RVA: 0x00018004 File Offset: 0x00016204
	public override bool CanMove()
	{
		return false;
	}

	// Token: 0x06000565 RID: 1381 RVA: 0x00018007 File Offset: 0x00016207
	public override bool CanRun()
	{
		return false;
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x0001800A File Offset: 0x0001620A
	public override bool CanUseHand()
	{
		return false;
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x0001800D File Offset: 0x0001620D
	public override bool IsReady()
	{
		return true;
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x00018010 File Offset: 0x00016210
	private int NewToken()
	{
		this.fishingTaskToken++;
		this.fishingTaskToken %= 1000;
		return this.fishingTaskToken;
	}

	// Token: 0x06000569 RID: 1385 RVA: 0x00018038 File Offset: 0x00016238
	private async UniTask Fishing()
	{
		Action_Fishing.<>c__DisplayClass48_0 CS$<>8__locals1 = new Action_Fishing.<>c__DisplayClass48_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.token = this.NewToken();
		this.quit = false;
		this.fishingState = Action_Fishing.FishingStates.intro;
		await UniTask.WaitForSeconds(this.introTime, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		while (CS$<>8__locals1.<Fishing>g__IsTaskValid|0())
		{
			await UniTask.WaitForEndOfFrame(this);
			await this.SingleFishingLoop(new Func<bool>(CS$<>8__locals1.<Fishing>g__IsTaskValid|0));
		}
	}

	// Token: 0x0600056A RID: 1386 RVA: 0x0001807C File Offset: 0x0001627C
	private async UniTask SingleFishingLoop(Func<bool> IsTaskValid)
	{
		if (IsTaskValid())
		{
			this.fishingState = Action_Fishing.FishingStates.selectingBait;
			bool flag = await this.WaitForSelectBait();
			if (IsTaskValid() && flag)
			{
				this.fishingState = Action_Fishing.FishingStates.fishing;
				Action<Action_Fishing> onPlayerStartFishing = Action_Fishing.OnPlayerStartFishing;
				if (onPlayerStartFishing != null)
				{
					onPlayerStartFishing(this);
				}
				await UniTask.WaitForSeconds(this.fishingWaitTime, false, PlayerLoopTiming.Update, default(CancellationToken), false);
				if (IsTaskValid())
				{
					bool flag2 = await this.Catching(IsTaskValid);
					if (IsTaskValid())
					{
						this.fishingState = Action_Fishing.FishingStates.over;
						this.resultConfirmed = false;
						this.continueFishing = false;
						if (flag2)
						{
							Item item = await ItemAssetsCollection.InstantiateAsync(this.testCatchItem);
							Action<Action_Fishing, Item, Action<bool>> onPlayerStopCatching = Action_Fishing.OnPlayerStopCatching;
							if (onPlayerStopCatching != null)
							{
								onPlayerStopCatching(this, item, new Action<bool>(this.ResultConfirm));
							}
							PopText.Pop("成功", base.transform.position, Color.white, 1f, null);
						}
						else
						{
							Action<Action_Fishing, Item, Action<bool>> onPlayerStopCatching2 = Action_Fishing.OnPlayerStopCatching;
							if (onPlayerStopCatching2 != null)
							{
								onPlayerStopCatching2(this, null, new Action<bool>(this.ResultConfirm));
							}
							PopText.Pop("失败", base.transform.position, Color.white, 1f, null);
						}
						await UniTask.WaitUntil(() => this.quit || this.resultConfirmed, PlayerLoopTiming.Update, default(CancellationToken), false);
						if (IsTaskValid() && this.continueFishing)
						{
							this.fishingState = Action_Fishing.FishingStates.notStarted;
							return;
						}
					}
				}
			}
		}
		this.fishingState = Action_Fishing.FishingStates.notStarted;
		this.quit = true;
		if (base.Running)
		{
			base.StopAction();
		}
	}

	// Token: 0x0600056B RID: 1387 RVA: 0x000180C7 File Offset: 0x000162C7
	private void ResultConfirm(bool _continueFishing)
	{
		this.resultConfirmed = true;
		this.continueFishing = _continueFishing;
	}

	// Token: 0x0600056C RID: 1388 RVA: 0x000180D8 File Offset: 0x000162D8
	private async UniTask<bool> Catching(Func<bool> IsTaskValid)
	{
		this.catchInput = false;
		this.fishingState = Action_Fishing.FishingStates.catching;
		PopText.Pop(string.Format("FFFF,控制:{0}", InputManager.InputActived), base.transform.position, Color.white, 1f, null);
		float currentTime = 0f;
		Action<Action_Fishing, float, Func<float>> onPlayerStartCatching = Action_Fishing.OnPlayerStartCatching;
		if (onPlayerStartCatching != null)
		{
			onPlayerStartCatching(this, this.catchTime, () => currentTime);
		}
		await UniTask.WaitForEndOfFrame(this);
		float startCatchTime = Time.time;
		bool catchOver = false;
		while (!catchOver)
		{
			currentTime = Time.time - startCatchTime;
			bool flag;
			if (!IsTaskValid())
			{
				flag = false;
			}
			else if (this.catchInput && currentTime < this.catchTime)
			{
				Debug.Log("catch");
				catchOver = true;
				flag = true;
			}
			else
			{
				if (currentTime < this.catchTime)
				{
					await UniTask.WaitForEndOfFrame(this);
					continue;
				}
				catchOver = true;
				flag = false;
			}
			return flag;
		}
		return 0;
	}

	// Token: 0x0600056D RID: 1389 RVA: 0x00018124 File Offset: 0x00016324
	private async UniTask<bool> WaitForSelectBait()
	{
		this.bait = null;
		Action<Action_Fishing, ICollection<Item>, Func<Item, bool>> onPlayerStartSelectBait = Action_Fishing.OnPlayerStartSelectBait;
		if (onPlayerStartSelectBait != null)
		{
			onPlayerStartSelectBait(this, this.GetAllBaits(), new Func<Item, bool>(this.SelectBaitAndStartFishing));
		}
		await UniTask.WaitUntil(() => this.quit || this.bait != null, PlayerLoopTiming.Update, default(CancellationToken), false);
		bool flag;
		if (this.quit)
		{
			flag = false;
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	// Token: 0x0600056E RID: 1390 RVA: 0x00018168 File Offset: 0x00016368
	public List<Item> GetAllBaits()
	{
		List<Item> list = new List<Item>();
		if (!this.characterController)
		{
			return list;
		}
		foreach (Item item in this.characterController.CharacterItem.Inventory)
		{
			if (item.Tags.Contains(GameplayDataSettings.Tags.Bait))
			{
				list.Add(item);
			}
		}
		return list;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x000181EC File Offset: 0x000163EC
	public void CatchButton()
	{
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000181EE File Offset: 0x000163EE
	public void Quit()
	{
		Debug.Log("Quit");
		this.quit = true;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x00018204 File Offset: 0x00016404
	private bool SelectBaitAndStartFishing(Item _bait)
	{
		if (_bait == null)
		{
			Debug.Log("鱼饵选了个null, 退出");
			this.Quit();
			return false;
		}
		if (!_bait.Tags.Contains(GameplayDataSettings.Tags.Bait))
		{
			this.Quit();
			return false;
		}
		this.bait = _bait;
		return true;
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00018254 File Offset: 0x00016454
	private void OnDestroy()
	{
		if (base.Running)
		{
			Action<Action_Fishing> onPlayerStopFishing = Action_Fishing.OnPlayerStopFishing;
			if (onPlayerStopFishing != null)
			{
				onPlayerStopFishing(this);
			}
		}
		InputManager.OnInteractButtonDown = (Action)Delegate.Remove(InputManager.OnInteractButtonDown, new Action(this.OnCatchButton));
		UIInputManager.OnCancel -= this.UIOnCancle;
	}

	// Token: 0x040004C7 RID: 1223
	[SerializeField]
	private CinemachineVirtualCamera fishingCamera;

	// Token: 0x040004C8 RID: 1224
	private FishingRod fishingRod;

	// Token: 0x040004C9 RID: 1225
	[SerializeField]
	private FishingPoint fishingPoint;

	// Token: 0x040004CA RID: 1226
	[SerializeField]
	private float introTime = 0.2f;

	// Token: 0x040004CB RID: 1227
	private float fishingWaitTime = 2f;

	// Token: 0x040004CC RID: 1228
	private float catchTime = 0.5f;

	// Token: 0x040004CD RID: 1229
	private Item bait;

	// Token: 0x040004CE RID: 1230
	private Transform socket;

	// Token: 0x040004CF RID: 1231
	[SerializeField]
	[ItemTypeID]
	private int testCatchItem;

	// Token: 0x040004D0 RID: 1232
	private Item catchedItem;

	// Token: 0x040004D1 RID: 1233
	private bool quit;

	// Token: 0x040004D2 RID: 1234
	private UniTask currentTask;

	// Token: 0x040004D3 RID: 1235
	private bool catchInput;

	// Token: 0x040004D4 RID: 1236
	private bool resultConfirmed;

	// Token: 0x040004D5 RID: 1237
	private bool continueFishing;

	// Token: 0x040004DB RID: 1243
	private Action_Fishing.FishingStates fishingState;

	// Token: 0x040004DC RID: 1244
	private int fishingTaskToken;

	// Token: 0x02000449 RID: 1097
	public enum FishingStates
	{
		// Token: 0x04001AA0 RID: 6816
		notStarted,
		// Token: 0x04001AA1 RID: 6817
		intro,
		// Token: 0x04001AA2 RID: 6818
		selectingBait,
		// Token: 0x04001AA3 RID: 6819
		fishing,
		// Token: 0x04001AA4 RID: 6820
		catching,
		// Token: 0x04001AA5 RID: 6821
		over
	}
}
