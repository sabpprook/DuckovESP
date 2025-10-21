using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Duckov.Rules;
using Duckov.Scenes;
using Duckov.Utilities;
using ItemStatsSystem;
using ItemStatsSystem.Data;
using Saves;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Duckov
{
	// Token: 0x0200023B RID: 571
	public class DeadBodyManager : MonoBehaviour
	{
		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060011AE RID: 4526 RVA: 0x00043F6F File Offset: 0x0004216F
		// (set) Token: 0x060011AF RID: 4527 RVA: 0x00043F76 File Offset: 0x00042176
		public static DeadBodyManager Instance { get; private set; }

		// Token: 0x060011B0 RID: 4528 RVA: 0x00043F7E File Offset: 0x0004217E
		private void AppendDeathInfo(DeadBodyManager.DeathInfo deathInfo)
		{
			while (this.deaths.Count >= GameRulesManager.Current.SaveDeadbodyCount)
			{
				this.deaths.RemoveAt(0);
			}
			this.deaths.Add(deathInfo);
			this.Save();
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00043FB7 File Offset: 0x000421B7
		private static List<DeadBodyManager.DeathInfo> LoadDeathInfos()
		{
			return SavesSystem.Load<List<DeadBodyManager.DeathInfo>>("DeathList");
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00043FC4 File Offset: 0x000421C4
		internal static void RecordDeath(CharacterMainControl mainCharacter)
		{
			if (DeadBodyManager.Instance == null)
			{
				Debug.LogError("DeadBodyManager Instance is null");
				return;
			}
			DeadBodyManager.DeathInfo deathInfo = new DeadBodyManager.DeathInfo();
			deathInfo.valid = true;
			deathInfo.raidID = RaidUtilities.CurrentRaid.ID;
			deathInfo.subSceneID = MultiSceneCore.ActiveSubSceneID;
			deathInfo.worldPosition = mainCharacter.transform.position;
			deathInfo.itemTreeData = ItemTreeData.FromItem(mainCharacter.CharacterItem);
			DeadBodyManager.Instance.AppendDeathInfo(deathInfo);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00044040 File Offset: 0x00042240
		private void Awake()
		{
			DeadBodyManager.Instance = this;
			MultiSceneCore.OnSubSceneLoaded += this.OnSubSceneLoaded;
			this.deaths.Clear();
			List<DeadBodyManager.DeathInfo> list = DeadBodyManager.LoadDeathInfos();
			if (list != null)
			{
				this.deaths.AddRange(list);
			}
			SavesSystem.OnCollectSaveData += this.Save;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00044095 File Offset: 0x00042295
		private void OnDestroy()
		{
			MultiSceneCore.OnSubSceneLoaded -= this.OnSubSceneLoaded;
			SavesSystem.OnCollectSaveData -= this.Save;
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x000440B9 File Offset: 0x000422B9
		private void Save()
		{
			SavesSystem.Save<List<DeadBodyManager.DeathInfo>>("DeathList", this.deaths);
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x000440CC File Offset: 0x000422CC
		private void OnSubSceneLoaded(MultiSceneCore core, Scene scene)
		{
			LevelManager.LevelInitializingComment = "Spawning bodies";
			if (!LevelConfig.SpawnTomb)
			{
				return;
			}
			foreach (DeadBodyManager.DeathInfo deathInfo in this.deaths)
			{
				if (this.ShouldSpawnDeadBody(deathInfo))
				{
					this.SpawnDeadBody(deathInfo).Forget();
				}
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00044140 File Offset: 0x00042340
		private async UniTask SpawnDeadBody(DeadBodyManager.DeathInfo info)
		{
			Item item = await ItemTreeData.InstantiateAsync(info.itemTreeData);
			if (!(item == null))
			{
				Vector3 worldPosition = info.worldPosition;
				string subSceneID = info.subSceneID;
				InteractableLootbox.CreateFromItem(item, worldPosition, Quaternion.identity, true, GameplayDataSettings.Prefabs.LootBoxPrefab_Tomb, true).OnInteractStartEvent.AddListener(delegate(CharacterMainControl a, InteractableBase b)
				{
					DeadBodyManager.NotifyDeadbodyTouched(info);
				});
				info.spawned = true;
			}
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00044183 File Offset: 0x00042383
		private static void NotifyDeadbodyTouched(DeadBodyManager.DeathInfo info)
		{
			if (DeadBodyManager.Instance == null)
			{
				return;
			}
			DeadBodyManager.Instance.OnDeadbodyTouched(info);
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x000441A0 File Offset: 0x000423A0
		private void OnDeadbodyTouched(DeadBodyManager.DeathInfo info)
		{
			DeadBodyManager.DeathInfo deathInfo = this.deaths.Find((DeadBodyManager.DeathInfo e) => e.raidID == info.raidID);
			if (deathInfo == null)
			{
				return;
			}
			deathInfo.touched = true;
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000441E0 File Offset: 0x000423E0
		private bool ShouldSpawnDeadBody(DeadBodyManager.DeathInfo info)
		{
			return info != null && GameRulesManager.Current.SpawnDeadBody && LevelManager.Instance && LevelManager.Instance.IsRaidMap && info != null && info.valid && !info.touched && !(MultiSceneCore.ActiveSubSceneID != info.subSceneID);
		}

		// Token: 0x04000DA8 RID: 3496
		private List<DeadBodyManager.DeathInfo> deaths = new List<DeadBodyManager.DeathInfo>();

		// Token: 0x02000525 RID: 1317
		[Serializable]
		public class DeathInfo
		{
			// Token: 0x04001E42 RID: 7746
			public bool valid;

			// Token: 0x04001E43 RID: 7747
			public uint raidID;

			// Token: 0x04001E44 RID: 7748
			public string subSceneID;

			// Token: 0x04001E45 RID: 7749
			public Vector3 worldPosition;

			// Token: 0x04001E46 RID: 7750
			public ItemTreeData itemTreeData;

			// Token: 0x04001E47 RID: 7751
			public bool spawned;

			// Token: 0x04001E48 RID: 7752
			public bool touched;
		}
	}
}
