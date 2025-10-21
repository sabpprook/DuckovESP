using System;
using SodaCraft.Localizations;
using UnityEngine;

namespace Duckov.Rules
{
	// Token: 0x020003EE RID: 1006
	[Serializable]
	public class Ruleset
	{
		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06002446 RID: 9286 RVA: 0x0007E1E9 File Offset: 0x0007C3E9
		// (set) Token: 0x06002447 RID: 9287 RVA: 0x0007E1FB File Offset: 0x0007C3FB
		[LocalizationKey("UIText")]
		internal string descriptionKey
		{
			get
			{
				return this.displayNameKey + "_Desc";
			}
			set
			{
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06002448 RID: 9288 RVA: 0x0007E1FD File Offset: 0x0007C3FD
		public string DisplayName
		{
			get
			{
				return this.displayNameKey.ToPlainText();
			}
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06002449 RID: 9289 RVA: 0x0007E20A File Offset: 0x0007C40A
		public string Description
		{
			get
			{
				return this.descriptionKey.ToPlainText();
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x0600244A RID: 9290 RVA: 0x0007E217 File Offset: 0x0007C417
		public bool SpawnDeadBody
		{
			get
			{
				return this.spawnDeadBody;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x0600244B RID: 9291 RVA: 0x0007E21F File Offset: 0x0007C41F
		public int SaveDeadbodyCount
		{
			get
			{
				return this.saveDeadbodyCount;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x0600244C RID: 9292 RVA: 0x0007E227 File Offset: 0x0007C427
		public bool FogOfWar
		{
			get
			{
				return this.fogOfWar;
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x0600244D RID: 9293 RVA: 0x0007E22F File Offset: 0x0007C42F
		public bool AdvancedDebuffMode
		{
			get
			{
				return this.advancedDebuffMode;
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600244E RID: 9294 RVA: 0x0007E237 File Offset: 0x0007C437
		public float RecoilMultiplier
		{
			get
			{
				return this.recoilMultiplier;
			}
		}

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x0600244F RID: 9295 RVA: 0x0007E23F File Offset: 0x0007C43F
		public float DamageFactor_ToPlayer
		{
			get
			{
				return this.damageFactor_ToPlayer;
			}
		}

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06002450 RID: 9296 RVA: 0x0007E247 File Offset: 0x0007C447
		public float EnemyHealthFactor
		{
			get
			{
				return this.enemyHealthFactor;
			}
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06002451 RID: 9297 RVA: 0x0007E24F File Offset: 0x0007C44F
		public float EnemyReactionTimeFactor
		{
			get
			{
				return this.enemyReactionTimeFactor;
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06002452 RID: 9298 RVA: 0x0007E257 File Offset: 0x0007C457
		public float EnemyAttackTimeSpaceFactor
		{
			get
			{
				return this.enemyAttackTimeSpaceFactor;
			}
		}

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06002453 RID: 9299 RVA: 0x0007E25F File Offset: 0x0007C45F
		public float EnemyAttackTimeFactor
		{
			get
			{
				return this.enemyAttackTimeFactor;
			}
		}

		// Token: 0x040018B8 RID: 6328
		[LocalizationKey("UIText")]
		[SerializeField]
		internal string displayNameKey;

		// Token: 0x040018B9 RID: 6329
		[SerializeField]
		private float damageFactor_ToPlayer = 1f;

		// Token: 0x040018BA RID: 6330
		[SerializeField]
		private float enemyHealthFactor = 1f;

		// Token: 0x040018BB RID: 6331
		[SerializeField]
		private bool spawnDeadBody = true;

		// Token: 0x040018BC RID: 6332
		[SerializeField]
		private bool fogOfWar = true;

		// Token: 0x040018BD RID: 6333
		[SerializeField]
		private bool advancedDebuffMode;

		// Token: 0x040018BE RID: 6334
		[SerializeField]
		private int saveDeadbodyCount = 1;

		// Token: 0x040018BF RID: 6335
		[Range(0f, 1f)]
		[SerializeField]
		private float recoilMultiplier = 1f;

		// Token: 0x040018C0 RID: 6336
		[SerializeField]
		internal float enemyReactionTimeFactor = 1f;

		// Token: 0x040018C1 RID: 6337
		[SerializeField]
		internal float enemyAttackTimeSpaceFactor = 1f;

		// Token: 0x040018C2 RID: 6338
		[SerializeField]
		internal float enemyAttackTimeFactor = 1f;
	}
}
