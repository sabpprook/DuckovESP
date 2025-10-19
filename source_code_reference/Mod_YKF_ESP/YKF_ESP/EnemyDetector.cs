using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	public class EnemyDetector
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002A94 File Offset: 0x00000C94
		public EnemyDetector(ESPSettings settings, LogManager logManager)
		{
			this.settings = settings;
			this.logManager = logManager;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002B0D File Offset: 0x00000D0D
		public void UpdateSettings(ESPSettings newSettings)
		{
			this.settings = newSettings;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002B16 File Offset: 0x00000D16
		public List<EnemyInfo> GetEnemyInfoList()
		{
			return this.enemyInfoList;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002B20 File Offset: 0x00000D20
		public void Update(CharacterMainControl player, CharacterMainControl[] allCharactersCache)
		{
			if (player == null)
			{
				return;
			}
			if (Time.time - this.lastAIControllerUpdateTime > 5f)
			{
				this.UpdateAIControllers();
				this.lastAIControllerUpdateTime = Time.time;
			}
			if (Time.time - this.lastEnemyUpdateTime > 0.5f)
			{
				this.UpdateEnemiesList(player, allCharactersCache);
				this.UpdateESPData(player);
				this.lastEnemyUpdateTime = Time.time;
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002B88 File Offset: 0x00000D88
		private void UpdateAIControllers()
		{
			this.aiControllers.Clear();
			foreach (AICharacterController aicharacterController in Object.FindObjectsOfType<AICharacterController>())
			{
				if (aicharacterController.CharacterMainControl != null && !this.aiControllers.ContainsKey(aicharacterController.CharacterMainControl))
				{
					this.aiControllers[aicharacterController.CharacterMainControl] = aicharacterController;
				}
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002BEC File Offset: 0x00000DEC
		private void UpdateEnemiesList(CharacterMainControl player, CharacterMainControl[] allCharactersCache)
		{
			try
			{
				List<CharacterMainControl> list = new List<CharacterMainControl>();
				HashSet<CharacterMainControl> hashSet = new HashSet<CharacterMainControl>();
				foreach (CharacterMainControl characterMainControl in allCharactersCache)
				{
					if (!(characterMainControl == null) && !(characterMainControl == player))
					{
						hashSet.Add(characterMainControl);
						if (this.processedEnemies.Contains(characterMainControl))
						{
							if (!characterMainControl.IsDead() && characterMainControl.IsValidEnemy(player))
							{
								list.Add(characterMainControl);
							}
						}
						else
						{
							try
							{
								if (!characterMainControl.IsDead())
								{
									if (characterMainControl.IsValidEnemy(player))
									{
										list.Add(characterMainControl);
										this.processedEnemies.Add(characterMainControl);
										this.ProcessNewEnemy(characterMainControl, player);
									}
								}
							}
							catch (Exception ex)
							{
								this.logManager.WriteLog(string.Format("Error processing new enemy: {0}", ex), true);
							}
						}
					}
				}
				this.CleanupOldEnemies(hashSet);
				this.enemies = list;
			}
			catch (Exception ex2)
			{
				this.logManager.WriteLog(string.Format("Error in UpdateEnemiesList: {0}", ex2), true);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002D0C File Offset: 0x00000F0C
		private void ProcessNewEnemy(CharacterMainControl character, CharacterMainControl player)
		{
			long num = EnemyValueCalculator.CalculateEnemyInventoryValue(character);
			this.enemyValues[character] = num;
			if (num >= this.settings.HighValueThreshold && !this.alertedHighValue.Contains(character) && player != null)
			{
				player.PopText(string.Format("!!! 高价值目标: ${0:N0} !!!", num), 10f);
				this.alertedHighValue.Add(character);
			}
			string weaponName = EnemyInfoHelper.GetWeaponName(character);
			string enemyName = EnemyInfoHelper.GetEnemyName(character);
			if (this.settings.EnableTraderAlert && enemyName == "Enemy" && weaponName == "无" && !this.alertedTraders.Contains(character) && player != null)
			{
				player.PopText("发现神秘商人,准备好你的钱哦!", 10f);
				this.alertedTraders.Add(character);
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002DE4 File Offset: 0x00000FE4
		private void CleanupOldEnemies(HashSet<CharacterMainControl> currentEnemies)
		{
			foreach (CharacterMainControl characterMainControl in this.processedEnemies.ToList<CharacterMainControl>())
			{
				if (!currentEnemies.Contains(characterMainControl))
				{
					this.processedEnemies.Remove(characterMainControl);
					this.enemyValues.Remove(characterMainControl);
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002E58 File Offset: 0x00001058
		private void UpdateESPData(CharacterMainControl player)
		{
			this.enemyInfoList.Clear();
			if (((player != null) ? player.mainDamageReceiver : null) == null)
			{
				return;
			}
			Vector3 playerPosition = EnemyInfoHelper.GetPlayerPosition(player);
			foreach (CharacterMainControl characterMainControl in this.enemies)
			{
				if (!(characterMainControl == null) && !characterMainControl.IsDead())
				{
					long num = (this.enemyValues.ContainsKey(characterMainControl) ? this.enemyValues[characterMainControl] : 0L);
					Vector3 enemyHeadPosition = EnemyInfoHelper.GetEnemyHeadPosition(characterMainControl);
					float num2 = Vector3.Distance(playerPosition, enemyHeadPosition);
					if (num2 <= this.settings.MaxUIDistance)
					{
						bool flag = this.CheckIfAimingAtPlayer(characterMainControl, player);
						Health health = characterMainControl.Health;
						float? num3 = ((health != null) ? new float?(health.CurrentHealth) : null);
						Health health2 = characterMainControl.Health;
						float valueOrDefault = (num3 / ((health2 != null) ? new float?(health2.MaxHealth) : null)).GetValueOrDefault();
						Health health3 = characterMainControl.Health;
						float num4 = ((health3 != null) ? health3.CurrentHealth : 0f);
						Health health4 = characterMainControl.Health;
						float num5 = ((health4 != null) ? health4.MaxHealth : 0f);
						string weaponName = EnemyInfoHelper.GetWeaponName(characterMainControl);
						string enemyName = EnemyInfoHelper.GetEnemyName(characterMainControl);
						Color specialColor = EnemyInfoHelper.GetSpecialColor(characterMainControl, num, this.settings.HighValueThreshold);
						this.enemyInfoList.Add(new EnemyInfo
						{
							Name = enemyName,
							HealthPercent = valueOrDefault,
							CurrentHealth = num4,
							MaxHealth = num5,
							Distance = num2,
							Weapon = weaponName,
							Value = num,
							IsAimingAtPlayer = flag,
							SpecialColor = specialColor,
							WorldPosition = enemyHeadPosition
						});
					}
				}
			}
			this.enemyInfoList.Sort(delegate(EnemyInfo a, EnemyInfo b)
			{
				int num6 = b.Value.CompareTo(a.Value);
				if (num6 != 0)
				{
					return num6;
				}
				return a.Distance.CompareTo(b.Distance);
			});
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000030A0 File Offset: 0x000012A0
		private bool CheckIfAimingAtPlayer(CharacterMainControl enemy, CharacterMainControl player)
		{
			AICharacterController aicharacterController;
			return this.aiControllers.TryGetValue(enemy, out aicharacterController) && aicharacterController.aimTarget != null && aicharacterController.aimTarget.gameObject == player.mainDamageReceiver.gameObject;
		}

		// Token: 0x0400000A RID: 10
		private ESPSettings settings;

		// Token: 0x0400000B RID: 11
		private LogManager logManager;

		// Token: 0x0400000C RID: 12
		private List<CharacterMainControl> enemies = new List<CharacterMainControl>();

		// Token: 0x0400000D RID: 13
		private List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();

		// Token: 0x0400000E RID: 14
		private Dictionary<CharacterMainControl, AICharacterController> aiControllers = new Dictionary<CharacterMainControl, AICharacterController>();

		// Token: 0x0400000F RID: 15
		private Dictionary<CharacterMainControl, long> enemyValues = new Dictionary<CharacterMainControl, long>();

		// Token: 0x04000010 RID: 16
		private HashSet<CharacterMainControl> processedEnemies = new HashSet<CharacterMainControl>();

		// Token: 0x04000011 RID: 17
		private HashSet<CharacterMainControl> alertedHighValue = new HashSet<CharacterMainControl>();

		// Token: 0x04000012 RID: 18
		private HashSet<CharacterMainControl> alertedPMCs = new HashSet<CharacterMainControl>();

		// Token: 0x04000013 RID: 19
		private HashSet<CharacterMainControl> alertedTraders = new HashSet<CharacterMainControl>();

		// Token: 0x04000014 RID: 20
		private float lastEnemyUpdateTime;

		// Token: 0x04000015 RID: 21
		private float lastAIControllerUpdateTime;

		// Token: 0x04000016 RID: 22
		private const float enemyUpdateInterval = 0.5f;

		// Token: 0x04000017 RID: 23
		private const float aiControllerUpdateInterval = 5f;
	}
}
