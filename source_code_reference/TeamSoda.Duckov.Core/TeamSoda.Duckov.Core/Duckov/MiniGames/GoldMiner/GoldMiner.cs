using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using Saves;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028C RID: 652
	public class GoldMiner : MiniGameBehaviour
	{
		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x0004E2BA File Offset: 0x0004C4BA
		public Hook Hook
		{
			get
			{
				return this.hook;
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06001522 RID: 5410 RVA: 0x0004E2C2 File Offset: 0x0004C4C2
		public Bounds Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06001523 RID: 5411 RVA: 0x0004E2CA File Offset: 0x0004C4CA
		public int Money
		{
			get
			{
				if (this.run == null)
				{
					return 0;
				}
				return this.run.money;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06001524 RID: 5412 RVA: 0x0004E2E1 File Offset: 0x0004C4E1
		public ReadOnlyCollection<GoldMinerArtifact> ArtifactPrefabs
		{
			get
			{
				if (this.artifactPrefabs_ReadOnly == null)
				{
					this.artifactPrefabs_ReadOnly = new ReadOnlyCollection<GoldMinerArtifact>(this.artifactPrefabs);
				}
				return this.artifactPrefabs_ReadOnly;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x0004E302 File Offset: 0x0004C502
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x0004E30E File Offset: 0x0004C50E
		public static int HighLevel
		{
			get
			{
				return SavesSystem.Load<int>("MiniGame/GoldMiner/HighLevel");
			}
			set
			{
				SavesSystem.Save<int>("MiniGame/GoldMiner/HighLevel", value);
			}
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x0004E31C File Offset: 0x0004C51C
		private void Awake()
		{
			this.Hook.OnBeginRetrieve += this.OnHookBeginRetrieve;
			this.Hook.OnEndRetrieve += this.OnHookEndRetrieve;
			this.Hook.OnLaunch += this.OnHookLaunch;
			this.Hook.OnResolveTarget += this.OnHookResolveEntity;
			this.Hook.OnAttach += this.OnHookAttach;
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x0004E39C File Offset: 0x0004C59C
		protected override void Start()
		{
			base.Start();
			this.hook.BeginSwing();
			this.Main().Forget();
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0004E3BA File Offset: 0x0004C5BA
		internal bool PayMoney(int price)
		{
			if (this.run.money < price)
			{
				return false;
			}
			this.run.money -= price;
			return true;
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x0600152A RID: 5418 RVA: 0x0004E3E0 File Offset: 0x0004C5E0
		// (set) Token: 0x0600152B RID: 5419 RVA: 0x0004E3E8 File Offset: 0x0004C5E8
		public GoldMinerRunData run { get; private set; }

		// Token: 0x1400008D RID: 141
		// (add) Token: 0x0600152C RID: 5420 RVA: 0x0004E3F4 File Offset: 0x0004C5F4
		// (remove) Token: 0x0600152D RID: 5421 RVA: 0x0004E428 File Offset: 0x0004C628
		public static event Action<int> OnLevelClear;

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x0600152E RID: 5422 RVA: 0x0004E45B File Offset: 0x0004C65B
		private bool ShouldQuit
		{
			get
			{
				return base.gameObject == null;
			}
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x0004E46E File Offset: 0x0004C66E
		public float GlobalPriceFactor
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06001530 RID: 5424 RVA: 0x0004E478 File Offset: 0x0004C678
		private async UniTask Main()
		{
			await this.DoTitleScreen();
			while (!this.ShouldQuit)
			{
				this.Cleanup();
				await this.Run(global::UnityEngine.Random.Range(int.MinValue, int.MaxValue));
			}
		}

		// Token: 0x06001531 RID: 5425 RVA: 0x0004E4BC File Offset: 0x0004C6BC
		private async UniTask DoTitleScreen()
		{
			this.titleConfirmed = false;
			if (this.titleScreen)
			{
				this.titleScreen.SetActive(true);
			}
			while (!this.titleConfirmed)
			{
				await UniTask.Yield();
				if (base.Game.GetButtonDown(MiniGame.Button.A) || base.Game.GetButtonDown(MiniGame.Button.Start))
				{
					this.titleConfirmed = true;
				}
			}
			if (this.titleScreen)
			{
				this.titleScreen.SetActive(false);
			}
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x0004E500 File Offset: 0x0004C700
		private async UniTask DoGameOver()
		{
			Debug.Log("Game Over");
			return;
			IL_0020:
			await UniTask.Yield();
			if (!base.Game.GetButtonDown(MiniGame.Button.A) && !base.Game.GetButtonDown(MiniGame.Button.Start))
			{
				goto IL_00A0;
			}
			this.gameOverConfirmed = true;
			IL_00A0:
			if (!this.gameOverConfirmed)
			{
				goto IL_0020;
			}
			if (!this.gameoverScreen)
			{
				goto IL_00C4;
			}
			this.gameoverScreen.SetActive(false);
			IL_00C4:;
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x0004E543 File Offset: 0x0004C743
		public void Cleanup()
		{
			if (this.run != null)
			{
				this.run.Cleanup();
			}
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x0004E558 File Offset: 0x0004C758
		private void GenerateLevel()
		{
			GoldMiner.<>c__DisplayClass58_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			for (int i = 0; i < this.activeEntities.Count; i++)
			{
				GoldMinerEntity goldMinerEntity = this.activeEntities[i];
				if (!(goldMinerEntity == null))
				{
					if (Application.isPlaying)
					{
						global::UnityEngine.Object.Destroy(goldMinerEntity.gameObject);
					}
					else
					{
						global::UnityEngine.Object.DestroyImmediate(goldMinerEntity.gameObject);
					}
				}
			}
			this.activeEntities.Clear();
			for (int j = 0; j < this.resolvedEntities.Count; j++)
			{
				GoldMinerEntity goldMinerEntity2 = this.activeEntities[j];
				if (!(goldMinerEntity2 == null))
				{
					if (Application.isPlaying)
					{
						global::UnityEngine.Object.Destroy(goldMinerEntity2.gameObject);
					}
					else
					{
						global::UnityEngine.Object.DestroyImmediate(goldMinerEntity2.gameObject);
					}
				}
			}
			this.resolvedEntities.Clear();
			int num = this.run.levelRandom.Next();
			CS$<>8__locals1.levelGenRandom = new global::System.Random(num);
			int num2 = 10;
			int num3 = 20;
			int num4 = CS$<>8__locals1.levelGenRandom.Next(num2, num3);
			for (int k = 0; k < num4; k++)
			{
				GoldMinerEntity random = this.entities.GetRandom(CS$<>8__locals1.levelGenRandom, 0f);
				this.<GenerateLevel>g__Generate|58_0(random, ref CS$<>8__locals1);
			}
			for (float num5 = this.run.extraRocks; num5 > 0f; num5 -= 1f)
			{
				if (num5 > 1f || CS$<>8__locals1.levelGenRandom.NextDouble() < (double)num5)
				{
					GoldMinerEntity random2 = this.entities.GetRandom(CS$<>8__locals1.levelGenRandom, (GoldMinerEntity e) => e.tags.Contains(GoldMinerEntity.Tag.Rock), 0f);
					this.<GenerateLevel>g__Generate|58_0(random2, ref CS$<>8__locals1);
				}
			}
			for (float num6 = this.run.extraGold; num6 > 0f; num6 -= 1f)
			{
				if (num6 > 1f || CS$<>8__locals1.levelGenRandom.NextDouble() < (double)num6)
				{
					GoldMinerEntity random3 = this.entities.GetRandom(CS$<>8__locals1.levelGenRandom, (GoldMinerEntity e) => e.tags.Contains(GoldMinerEntity.Tag.Gold), 0f);
					this.<GenerateLevel>g__Generate|58_0(random3, ref CS$<>8__locals1);
				}
			}
			for (float num7 = this.run.extraDiamond; num7 > 0f; num7 -= 1f)
			{
				if (num7 > 1f || CS$<>8__locals1.levelGenRandom.NextDouble() < (double)num7)
				{
					GoldMinerEntity random4 = this.entities.GetRandom(CS$<>8__locals1.levelGenRandom, (GoldMinerEntity e) => e.tags.Contains(GoldMinerEntity.Tag.Diamond), 0f);
					this.<GenerateLevel>g__Generate|58_0(random4, ref CS$<>8__locals1);
				}
			}
			this.run.shopRandom = new global::System.Random(this.run.seed + CS$<>8__locals1.levelGenRandom.Next());
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x0004E82C File Offset: 0x0004CA2C
		private Vector3 NormalizedPosToLocalPos(Vector2 posNormalized)
		{
			float num = Mathf.Lerp(this.bounds.min.x, this.bounds.max.x, posNormalized.x);
			float num2 = Mathf.Lerp(this.bounds.min.y, this.bounds.max.y, posNormalized.y);
			return new Vector3(num, num2, 0f);
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x0004E89B File Offset: 0x0004CA9B
		private void OnDrawGizmosSelected()
		{
			Gizmos.matrix = this.levelLayout.localToWorldMatrix;
			Gizmos.DrawWireCube(this.bounds.center, this.bounds.extents * 2f);
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x0004E8D4 File Offset: 0x0004CAD4
		private async UniTask Run(int seed = 0)
		{
			this.run = new GoldMinerRunData(this, seed);
			for (;;)
			{
				await this.DoLevel();
				UniTask<bool>.Awaiter awaiter = this.SettleLevel().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					await awaiter;
					UniTask<bool>.Awaiter awaiter2;
					awaiter = awaiter2;
					awaiter2 = default(UniTask<bool>.Awaiter);
				}
				if (!awaiter.GetResult())
				{
					break;
				}
				if (this.run.level > GoldMiner.HighLevel)
				{
					GoldMiner.HighLevel = this.run.level;
				}
				await this.DoShop();
				this.run.level++;
			}
			await this.DoGameOver();
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0004E920 File Offset: 0x0004CB20
		private async UniTask<bool> SettleLevel()
		{
			int moneySum = 0;
			float factor = this.run.scoreFactorBase.Value + this.run.levelScoreFactor;
			int targetScore = this.run.targetScore;
			this.settlementUI.Reset();
			this.settlementUI.SetTargetScore(targetScore);
			this.settlementUI.Show();
			await UniTask.WaitForSeconds(0.5f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			foreach (GoldMinerEntity goldMinerEntity in this.resolvedEntities)
			{
				int num = goldMinerEntity.Value;
				num = Mathf.CeilToInt((float)num * this.run.charm.Value);
				foreach (Func<int, int> func in this.run.settleValueProcessor)
				{
					num = func(num);
				}
				if (num != 0)
				{
					moneySum += num;
					int num2 = Mathf.CeilToInt((float)moneySum * factor);
					this.settlementUI.StepResolveEntity(goldMinerEntity);
					this.settlementUI.Step(moneySum, factor, num2);
					await UniTask.WaitForSeconds(0.2f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
				}
			}
			List<GoldMinerEntity>.Enumerator enumerator = default(List<GoldMinerEntity>.Enumerator);
			foreach (Func<float> func2 in this.run.additionalFactorFuncs)
			{
				float num3 = func2();
				if (num3 != 0f)
				{
					factor += num3;
					int num4 = Mathf.CeilToInt((float)moneySum * factor);
					this.settlementUI.Step(moneySum, factor, num4);
					await UniTask.WaitForSeconds(0.2f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
				}
			}
			List<Func<float>>.Enumerator enumerator3 = default(List<Func<float>>.Enumerator);
			if (moneySum < this.run.minMoneySum)
			{
				moneySum = this.run.minMoneySum;
			}
			int num5 = Mathf.CeilToInt((float)moneySum * factor);
			this.settlementUI.Step(moneySum, factor, num5);
			bool result = num5 >= targetScore;
			this.settlementUI.StepResult(result);
			if (!result)
			{
				for (int i = 0; i < this.run.forceLevelSuccessFuncs.Count; i++)
				{
					Func<bool> func3 = this.run.forceLevelSuccessFuncs[i];
					if (func3 != null && func3())
					{
						result = true;
						this.settlementUI.StepResult(result);
						break;
					}
				}
			}
			this.run.money += moneySum;
			while (!base.Game.GetButton(MiniGame.Button.A))
			{
				await UniTask.Yield();
			}
			this.settlementUI.Hide();
			if (result)
			{
				Action<int> onLevelClear = GoldMiner.OnLevelClear;
				if (onLevelClear != null)
				{
					onLevelClear(this.run.level);
				}
			}
			return result;
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0004E964 File Offset: 0x0004CB64
		private async UniTask DoLevel()
		{
			this.Hook.Reset();
			this.resolvedEntities.Clear();
			this.GenerateLevel();
			this.run.levelScoreFactor = 0f;
			this.run.stamina = this.run.maxStamina.Value;
			int level = this.run.level;
			this.run.targetScore = Mathf.CeilToInt(40.564f * Mathf.Exp(0.2118f * (float)(level + 1))) * 10;
			Action<GoldMiner> action = this.onLevelBegin;
			if (action != null)
			{
				action(this);
			}
			this.popText.Pop(string.Format("LEVEL {0}", this.run.level + 1), this.hook.Axis.position);
			await UniTask.WaitForSeconds(0.5f, false, PlayerLoopTiming.Update, default(CancellationToken), false);
			this.popText.Pop("Begin!", this.hook.Axis.position);
			this.levelPlaying = true;
			this.launchHook = false;
			while (!this.IsLevelOver())
			{
				await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
			}
			Action<GoldMiner> action2 = this.onLevelEnd;
			if (action2 != null)
			{
				action2(this);
			}
			this.levelPlaying = false;
			if (this.Hook.GrabbingTarget)
			{
				this.Hook.ReleaseClaw();
			}
		}

		// Token: 0x0600153A RID: 5434 RVA: 0x0004E9A7 File Offset: 0x0004CBA7
		protected override void OnUpdate(float deltaTime)
		{
			if (this.levelPlaying)
			{
				this.UpdateLevelPlaying(deltaTime);
			}
		}

		// Token: 0x0600153B RID: 5435 RVA: 0x0004E9B8 File Offset: 0x0004CBB8
		private void UpdateLevelPlaying(float deltaTime)
		{
			Action<GoldMiner> action = this.onEarlyLevelPlayTick;
			if (action != null)
			{
				action(this);
			}
			this.Hook.SetParameters(this.run.GameSpeedFactor, this.run.emptySpeed.Value, this.run.strength.Value);
			this.Hook.Tick(deltaTime);
			Hook.HookStatus status = this.Hook.Status;
			if (status != Hook.HookStatus.Swinging)
			{
				if (status == Hook.HookStatus.Retrieving)
				{
					this.run.stamina -= deltaTime * this.run.staminaDrain.Value;
				}
			}
			else if (this.launchHook)
			{
				this.Hook.Launch();
			}
			Action<GoldMiner> action2 = this.onLateLevelPlayTick;
			if (action2 != null)
			{
				action2(this);
			}
			this.launchHook = false;
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x0004EA81 File Offset: 0x0004CC81
		public void LaunchHook()
		{
			this.launchHook = true;
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x0004EA8C File Offset: 0x0004CC8C
		private bool IsLevelOver()
		{
			this.activeEntities.RemoveAll((GoldMinerEntity e) => e == null);
			return this.activeEntities.Count <= 0 || (this.hook.Status == Hook.HookStatus.Swinging && this.run.stamina <= 0f) || (this.Hook.Status == Hook.HookStatus.Retrieving && this.run.stamina < -this.run.extraStamina.Value);
		}

		// Token: 0x0600153E RID: 5438 RVA: 0x0004EB28 File Offset: 0x0004CD28
		private async UniTask DoShop()
		{
			Action<GoldMiner> action = this.onShopBegin;
			if (action != null)
			{
				action(this);
			}
			await this.shop.Execute();
			Action<GoldMiner> action2 = this.onShopEnd;
			if (action2 != null)
			{
				action2(this);
			}
		}

		// Token: 0x0600153F RID: 5439 RVA: 0x0004EB6C File Offset: 0x0004CD6C
		private void OnHookResolveEntity(Hook hook, GoldMinerEntity entity)
		{
			entity.NotifyResolved(this);
			entity.gameObject.SetActive(false);
			this.activeEntities.Remove(entity);
			this.resolvedEntities.Add(entity);
			if (this.run.IsRock(entity))
			{
				entity.Value = Mathf.CeilToInt((float)entity.Value * this.run.rockValueFactor.Value);
			}
			if (this.run.IsGold(entity))
			{
				entity.Value = Mathf.CeilToInt((float)entity.Value * this.run.goldValueFactor.Value);
			}
			this.popText.Pop(string.Format("${0}", entity.Value), hook.Axis.position);
			Action<GoldMiner, GoldMinerEntity> action = this.onResolveEntity;
			if (action != null)
			{
				action(this, entity);
			}
			Action<GoldMiner, GoldMinerEntity> action2 = this.onAfterResolveEntity;
			if (action2 == null)
			{
				return;
			}
			action2(this, entity);
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x0004EC57 File Offset: 0x0004CE57
		private void OnHookBeginRetrieve(Hook hook)
		{
			Action<GoldMiner, Hook> action = this.onHookBeginRetrieve;
			if (action == null)
			{
				return;
			}
			action(this, hook);
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x0004EC6B File Offset: 0x0004CE6B
		private void OnHookEndRetrieve(Hook hook)
		{
			Action<GoldMiner, Hook> action = this.onHookEndRetrieve;
			if (action != null)
			{
				action(this, hook);
			}
			if (this.run.StrengthPotionActivated)
			{
				this.run.DeactivateStrengthPotion();
			}
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x0004EC98 File Offset: 0x0004CE98
		private void OnHookLaunch(Hook hook)
		{
			Action<GoldMiner, Hook> action = this.onHookLaunch;
			if (action != null)
			{
				action(this, hook);
			}
			if (this.run.EagleEyeActivated)
			{
				this.run.DeactivateEagleEye();
			}
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x0004ECC5 File Offset: 0x0004CEC5
		private void OnHookAttach(Hook hook, GoldMinerEntity entity)
		{
			Action<GoldMiner, Hook, GoldMinerEntity> action = this.onHookAttach;
			if (action == null)
			{
				return;
			}
			action(this, hook, entity);
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x0004ECDA File Offset: 0x0004CEDA
		public bool UseStrengthPotion()
		{
			if (this.run.strengthPotion <= 0)
			{
				return false;
			}
			if (this.run.StrengthPotionActivated)
			{
				return false;
			}
			this.run.strengthPotion--;
			this.run.ActivateStrengthPotion();
			return true;
		}

		// Token: 0x06001545 RID: 5445 RVA: 0x0004ED1A File Offset: 0x0004CF1A
		public bool UseEagleEyePotion()
		{
			if (this.run.eagleEyePotion <= 0)
			{
				return false;
			}
			if (this.run.EagleEyeActivated)
			{
				return false;
			}
			this.run.eagleEyePotion--;
			this.run.ActivateEagleEye();
			return true;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x0004ED5C File Offset: 0x0004CF5C
		public GoldMinerArtifact GetArtifactPrefab(string id)
		{
			return this.artifactPrefabs.Find((GoldMinerArtifact e) => e != null && e.ID == id);
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x0004ED90 File Offset: 0x0004CF90
		internal bool UseBomb()
		{
			if (this.run.bomb <= 0)
			{
				return false;
			}
			this.run.bomb--;
			global::UnityEngine.Object.Instantiate<Bomb>(this.bombPrefab, this.hook.Axis.transform.position, Quaternion.FromToRotation(Vector3.up, -this.hook.Axis.transform.up), base.transform);
			return true;
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x0004EE0C File Offset: 0x0004D00C
		internal void NotifyArtifactChange()
		{
			Action<GoldMiner> action = this.onArtifactChange;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x0004EE48 File Offset: 0x0004D048
		[CompilerGenerated]
		private void <GenerateLevel>g__Generate|58_0(GoldMinerEntity entityPrefab, ref GoldMiner.<>c__DisplayClass58_0 A_2)
		{
			if (entityPrefab == null)
			{
				return;
			}
			Vector2 vector = new Vector2((float)A_2.levelGenRandom.NextDouble(), (float)A_2.levelGenRandom.NextDouble());
			GoldMinerEntity goldMinerEntity = global::UnityEngine.Object.Instantiate<GoldMinerEntity>(entityPrefab, this.levelLayout);
			Vector3 vector2 = this.NormalizedPosToLocalPos(vector);
			Quaternion quaternion = Quaternion.AngleAxis((float)A_2.levelGenRandom.NextDouble() * 360f, Vector3.forward);
			goldMinerEntity.transform.localPosition = vector2;
			goldMinerEntity.transform.localRotation = quaternion;
			goldMinerEntity.SetMaster(this);
			this.activeEntities.Add(goldMinerEntity);
		}

		// Token: 0x04000F9D RID: 3997
		[SerializeField]
		private Hook hook;

		// Token: 0x04000F9E RID: 3998
		[SerializeField]
		private GoldMinerShop shop;

		// Token: 0x04000F9F RID: 3999
		[SerializeField]
		private LevelSettlementUI settlementUI;

		// Token: 0x04000FA0 RID: 4000
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x04000FA1 RID: 4001
		[SerializeField]
		private GameObject gameoverScreen;

		// Token: 0x04000FA2 RID: 4002
		[SerializeField]
		private GoldMiner_PopText popText;

		// Token: 0x04000FA3 RID: 4003
		[SerializeField]
		private Transform levelLayout;

		// Token: 0x04000FA4 RID: 4004
		[SerializeField]
		private Bounds bounds;

		// Token: 0x04000FA5 RID: 4005
		[SerializeField]
		private Bomb bombPrefab;

		// Token: 0x04000FA6 RID: 4006
		[SerializeField]
		private RandomContainer<GoldMinerEntity> entities;

		// Token: 0x04000FA7 RID: 4007
		[SerializeField]
		private List<GoldMinerArtifact> artifactPrefabs = new List<GoldMinerArtifact>();

		// Token: 0x04000FA8 RID: 4008
		private ReadOnlyCollection<GoldMinerArtifact> artifactPrefabs_ReadOnly;

		// Token: 0x04000FA9 RID: 4009
		public Action<GoldMiner> onLevelBegin;

		// Token: 0x04000FAA RID: 4010
		public Action<GoldMiner> onLevelEnd;

		// Token: 0x04000FAB RID: 4011
		public Action<GoldMiner> onShopBegin;

		// Token: 0x04000FAC RID: 4012
		public Action<GoldMiner> onShopEnd;

		// Token: 0x04000FAD RID: 4013
		public Action<GoldMiner> onEarlyLevelPlayTick;

		// Token: 0x04000FAE RID: 4014
		public Action<GoldMiner> onLateLevelPlayTick;

		// Token: 0x04000FAF RID: 4015
		public Action<GoldMiner, Hook> onHookLaunch;

		// Token: 0x04000FB0 RID: 4016
		public Action<GoldMiner, Hook> onHookBeginRetrieve;

		// Token: 0x04000FB1 RID: 4017
		public Action<GoldMiner, Hook> onHookEndRetrieve;

		// Token: 0x04000FB2 RID: 4018
		public Action<GoldMiner, Hook, GoldMinerEntity> onHookAttach;

		// Token: 0x04000FB3 RID: 4019
		public Action<GoldMiner, GoldMinerEntity> onResolveEntity;

		// Token: 0x04000FB4 RID: 4020
		public Action<GoldMiner, GoldMinerEntity> onAfterResolveEntity;

		// Token: 0x04000FB5 RID: 4021
		public Action<GoldMiner> onArtifactChange;

		// Token: 0x04000FB6 RID: 4022
		private const string HighLevelSaveKey = "MiniGame/GoldMiner/HighLevel";

		// Token: 0x04000FB9 RID: 4025
		private bool titleConfirmed;

		// Token: 0x04000FBA RID: 4026
		private bool gameOverConfirmed;

		// Token: 0x04000FBB RID: 4027
		public List<GoldMinerEntity> activeEntities = new List<GoldMinerEntity>();

		// Token: 0x04000FBC RID: 4028
		private bool levelPlaying;

		// Token: 0x04000FBD RID: 4029
		public List<GoldMinerEntity> resolvedEntities = new List<GoldMinerEntity>();

		// Token: 0x04000FBE RID: 4030
		private bool launchHook;
	}
}
