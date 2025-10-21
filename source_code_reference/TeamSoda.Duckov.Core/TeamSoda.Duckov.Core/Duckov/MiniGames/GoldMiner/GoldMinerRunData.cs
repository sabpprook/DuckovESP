using System;
using System.Collections.Generic;
using System.Linq;
using ItemStatsSystem;
using ItemStatsSystem.Stats;
using UnityEngine;

namespace Duckov.MiniGames.GoldMiner
{
	// Token: 0x0200028B RID: 651
	[Serializable]
	public class GoldMinerRunData
	{
		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x0004DCF5 File Offset: 0x0004BEF5
		// (set) Token: 0x06001504 RID: 5380 RVA: 0x0004DCFD File Offset: 0x0004BEFD
		public int seed { get; private set; }

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x0004DD06 File Offset: 0x0004BF06
		// (set) Token: 0x06001506 RID: 5382 RVA: 0x0004DD0E File Offset: 0x0004BF0E
		public global::System.Random shopRandom { get; set; }

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x0004DD17 File Offset: 0x0004BF17
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x0004DD1F File Offset: 0x0004BF1F
		public global::System.Random levelRandom { get; private set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x0004DD28 File Offset: 0x0004BF28
		public float GameSpeedFactor
		{
			get
			{
				return this.gameSpeedFactor.Value;
			}
		}

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x0600150A RID: 5386 RVA: 0x0004DD35 File Offset: 0x0004BF35
		// (set) Token: 0x0600150B RID: 5387 RVA: 0x0004DD3D File Offset: 0x0004BF3D
		public float stamina { get; set; }

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x0600150C RID: 5388 RVA: 0x0004DD46 File Offset: 0x0004BF46
		// (set) Token: 0x0600150D RID: 5389 RVA: 0x0004DD4E File Offset: 0x0004BF4E
		public bool gameOver { get; set; }

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x0600150E RID: 5390 RVA: 0x0004DD57 File Offset: 0x0004BF57
		// (set) Token: 0x0600150F RID: 5391 RVA: 0x0004DD5F File Offset: 0x0004BF5F
		public int level { get; set; }

		// Token: 0x06001510 RID: 5392 RVA: 0x0004DD68 File Offset: 0x0004BF68
		public GoldMinerArtifact AttachArtifactFromPrefab(GoldMinerArtifact prefab)
		{
			if (prefab == null)
			{
				return null;
			}
			GoldMinerArtifact goldMinerArtifact = global::UnityEngine.Object.Instantiate<GoldMinerArtifact>(prefab, this.master.transform);
			this.AttachArtifact(goldMinerArtifact);
			return goldMinerArtifact;
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x0004DD9C File Offset: 0x0004BF9C
		private void AttachArtifact(GoldMinerArtifact artifact)
		{
			if (this.artifactCount.ContainsKey(artifact.ID))
			{
				Dictionary<string, int> dictionary = this.artifactCount;
				string id = artifact.ID;
				dictionary[id]++;
			}
			else
			{
				this.artifactCount[artifact.ID] = 1;
			}
			this.artifacts.Add(artifact);
			artifact.Attach(this.master);
			this.master.NotifyArtifactChange();
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x0004DE14 File Offset: 0x0004C014
		public bool DetachArtifact(GoldMinerArtifact artifact)
		{
			bool flag = this.artifacts.Remove(artifact);
			artifact.Detatch(this.master);
			if (this.artifactCount.ContainsKey(artifact.ID))
			{
				Dictionary<string, int> dictionary = this.artifactCount;
				string id = artifact.ID;
				dictionary[id]--;
			}
			else
			{
				Debug.LogError("Artifact counter error.", this.master);
			}
			this.master.NotifyArtifactChange();
			return flag;
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x0004DE88 File Offset: 0x0004C088
		public int GetArtifactCount(string id)
		{
			int num;
			if (this.artifactCount.TryGetValue(id, out num))
			{
				return num;
			}
			return 0;
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x0004DEA8 File Offset: 0x0004C0A8
		public GoldMinerRunData(GoldMiner master, int seed = 0)
		{
			this.master = master;
			if (seed == 0)
			{
				seed = global::UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			this.seed = seed;
			this.levelRandom = new global::System.Random(seed);
			this.strengthPotionModifier = new Modifier(ModifierType.Add, 100f, this);
			this.eagleEyeModifier = new Modifier(ModifierType.PercentageMultiply, -0.5f, this);
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06001515 RID: 5397 RVA: 0x0004E0B7 File Offset: 0x0004C2B7
		// (set) Token: 0x06001516 RID: 5398 RVA: 0x0004E0BF File Offset: 0x0004C2BF
		public bool StrengthPotionActivated { get; private set; }

		// Token: 0x06001517 RID: 5399 RVA: 0x0004E0C8 File Offset: 0x0004C2C8
		public void ActivateStrengthPotion()
		{
			if (this.StrengthPotionActivated)
			{
				return;
			}
			this.strength.AddModifier(this.strengthPotionModifier);
			this.StrengthPotionActivated = true;
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x0004E0EB File Offset: 0x0004C2EB
		public void DeactivateStrengthPotion()
		{
			this.strength.RemoveModifier(this.strengthPotionModifier);
			this.StrengthPotionActivated = false;
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06001519 RID: 5401 RVA: 0x0004E106 File Offset: 0x0004C306
		// (set) Token: 0x0600151A RID: 5402 RVA: 0x0004E10E File Offset: 0x0004C30E
		public bool EagleEyeActivated { get; private set; }

		// Token: 0x0600151B RID: 5403 RVA: 0x0004E117 File Offset: 0x0004C317
		public void ActivateEagleEye()
		{
			if (this.EagleEyeActivated)
			{
				return;
			}
			this.gameSpeedFactor.AddModifier(this.eagleEyeModifier);
			this.EagleEyeActivated = true;
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x0004E13A File Offset: 0x0004C33A
		public void DeactivateEagleEye()
		{
			this.gameSpeedFactor.RemoveModifier(this.eagleEyeModifier);
			this.EagleEyeActivated = false;
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x0004E158 File Offset: 0x0004C358
		internal void Cleanup()
		{
			foreach (GoldMinerArtifact goldMinerArtifact in this.artifacts)
			{
				if (!(goldMinerArtifact == null))
				{
					if (Application.isPlaying)
					{
						global::UnityEngine.Object.Destroy(goldMinerArtifact.gameObject);
					}
					else
					{
						global::UnityEngine.Object.Destroy(goldMinerArtifact.gameObject);
					}
				}
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x0004E1CC File Offset: 0x0004C3CC
		public bool IsGold(GoldMinerEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			using (List<Func<GoldMinerEntity, bool>>.Enumerator enumerator = this.isGoldPredicators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current(entity))
					{
						return true;
					}
				}
			}
			return entity.tags.Contains(GoldMinerEntity.Tag.Gold);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x0004E23C File Offset: 0x0004C43C
		public bool IsRock(GoldMinerEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			using (List<Func<GoldMinerEntity, bool>>.Enumerator enumerator = this.isGoldPredicators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current(entity))
					{
						return true;
					}
				}
			}
			return entity.tags.Contains(GoldMinerEntity.Tag.Rock);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x0004E2AC File Offset: 0x0004C4AC
		internal bool IsPig(GoldMinerEntity entity)
		{
			return entity.tags.Contains(GoldMinerEntity.Tag.Pig);
		}

		// Token: 0x04000F6E RID: 3950
		public readonly GoldMiner master;

		// Token: 0x04000F72 RID: 3954
		public int money;

		// Token: 0x04000F73 RID: 3955
		public int bomb;

		// Token: 0x04000F74 RID: 3956
		public int strengthPotion;

		// Token: 0x04000F75 RID: 3957
		public int eagleEyePotion;

		// Token: 0x04000F76 RID: 3958
		public int shopTicket;

		// Token: 0x04000F77 RID: 3959
		public const int shopDefaultItemAmount = 3;

		// Token: 0x04000F78 RID: 3960
		public const int shopMaxItemAmount = 3;

		// Token: 0x04000F79 RID: 3961
		public int shopCapacity = 3;

		// Token: 0x04000F7A RID: 3962
		public float levelScoreFactor;

		// Token: 0x04000F7B RID: 3963
		public Stat maxStamina = new Stat("maxStamina", 15f, false);

		// Token: 0x04000F7C RID: 3964
		public Stat extraStamina = new Stat("extraStamina", 2f, false);

		// Token: 0x04000F7D RID: 3965
		public Stat staminaDrain = new Stat("staminaDrain", 1f, false);

		// Token: 0x04000F7E RID: 3966
		public Stat gameSpeedFactor = new Stat("gameSpeedFactor", 1f, false);

		// Token: 0x04000F7F RID: 3967
		public Stat emptySpeed = new Stat("emptySpeed", 300f, false);

		// Token: 0x04000F80 RID: 3968
		public Stat strength = new Stat("strength", 0f, false);

		// Token: 0x04000F81 RID: 3969
		public Stat scoreFactorBase = new Stat("scoreFactor", 1f, false);

		// Token: 0x04000F82 RID: 3970
		public Stat rockValueFactor = new Stat("rockValueFactor", 1f, false);

		// Token: 0x04000F83 RID: 3971
		public Stat goldValueFactor = new Stat("goldValueFactor", 1f, false);

		// Token: 0x04000F84 RID: 3972
		public Stat charm = new Stat("charm", 1f, false);

		// Token: 0x04000F85 RID: 3973
		public Stat shopRefreshPrice = new Stat("shopRefreshPrice", 100f, false);

		// Token: 0x04000F86 RID: 3974
		public Stat shopRefreshPriceIncrement = new Stat("shopRefreshPriceIncrement", 50f, false);

		// Token: 0x04000F87 RID: 3975
		public Stat shopRefreshChances = new Stat("shopRefreshChances", 2f, false);

		// Token: 0x04000F88 RID: 3976
		public Stat shopPriceCut = new Stat("shopPriceCut", 0.7f, false);

		// Token: 0x04000F89 RID: 3977
		public Stat defuse = new Stat("defuse", 0f, false);

		// Token: 0x04000F8A RID: 3978
		public float extraRocks;

		// Token: 0x04000F8B RID: 3979
		public float extraGold;

		// Token: 0x04000F8C RID: 3980
		public float extraDiamond;

		// Token: 0x04000F8D RID: 3981
		public List<GoldMinerArtifact> artifacts = new List<GoldMinerArtifact>();

		// Token: 0x04000F91 RID: 3985
		private Dictionary<string, int> artifactCount = new Dictionary<string, int>();

		// Token: 0x04000F92 RID: 3986
		private Modifier strengthPotionModifier;

		// Token: 0x04000F94 RID: 3988
		private Modifier eagleEyeModifier;

		// Token: 0x04000F95 RID: 3989
		internal int targetScore = 100;

		// Token: 0x04000F97 RID: 3991
		public List<Func<GoldMinerEntity, bool>> isGoldPredicators = new List<Func<GoldMinerEntity, bool>>();

		// Token: 0x04000F98 RID: 3992
		public List<Func<GoldMinerEntity, bool>> isRockPredicators = new List<Func<GoldMinerEntity, bool>>();

		// Token: 0x04000F99 RID: 3993
		public List<Func<float>> additionalFactorFuncs = new List<Func<float>>();

		// Token: 0x04000F9A RID: 3994
		public List<Func<int, int>> settleValueProcessor = new List<Func<int, int>>();

		// Token: 0x04000F9B RID: 3995
		public List<Func<bool>> forceLevelSuccessFuncs = new List<Func<bool>>();

		// Token: 0x04000F9C RID: 3996
		internal int minMoneySum;
	}
}
