using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000B RID: 11
	[NullableContext(1)]
	[Nullable(0)]
	public class EnemyDetector
	{
		// Token: 0x06000028 RID: 40 RVA: 0x000039C8 File Offset: 0x00001BC8
		public EnemyDetector(ESPSettings settings, LogManager logManager)
		{
			this.settings = settings;
			this.logManager = logManager;
			this.RegisterEvents();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003A47 File Offset: 0x00001C47
		public void UpdateSettings(ESPSettings newSettings)
		{
			this.settings = newSettings;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003A50 File Offset: 0x00001C50
		public List<EnemyInfo> GetEnemyInfoList()
		{
			return this.enemyInfoList;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003A58 File Offset: 0x00001C58
		private void RegisterEvents()
		{
			if (this.eventsRegistered)
			{
				return;
			}
			try
			{
				AIMainBrain.OnSoundSpawned += this.OnAISoundDetected;
				this.eventsRegistered = true;
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("Successfully registered AI events for enemy detection optimization", false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Failed to register AI events: {0}", ex), true);
				}
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003AD0 File Offset: 0x00001CD0
		private void UnregisterEvents()
		{
			if (!this.eventsRegistered)
			{
				return;
			}
			try
			{
				AIMainBrain.OnSoundSpawned -= this.OnAISoundDetected;
				this.eventsRegistered = false;
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog("Unregistered AI events", false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error unregistering events: {0}", ex), true);
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003B48 File Offset: 0x00001D48
		private void OnAISoundDetected(AISound sound)
		{
			try
			{
				if (sound.fromCharacter != null && !this.trackedEnemies.Contains(sound.fromCharacter))
				{
					CharacterMainControl main = CharacterMainControl.Main;
					if (main != null && sound.fromCharacter.IsValidEnemy(main))
					{
						this.AddNewEnemy(sound.fromCharacter, main);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error in OnAISoundDetected: {0}", ex), true);
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003BD4 File Offset: 0x00001DD4
		public void Update(CharacterMainControl player, CharacterMainControl[] allCharactersCache)
		{
			if (player == null)
			{
				return;
			}
			try
			{
				float time = Time.time;
				if (time - this.lastAIControllerUpdateTime > 3f)
				{
					this.UpdateAIControllers();
					this.lastAIControllerUpdateTime = time;
				}
				if (time - this.lastCleanupTime > 10f)
				{
					this.CleanupDeadEnemies();
					this.lastCleanupTime = time;
				}
				if (time - this.lastFullScanTime > 30f)
				{
					this.PerformFullScan(player, allCharactersCache);
					this.lastFullScanTime = time;
				}
				this.UpdateESPData(player);
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error in EnemyDetector.Update: {0}", ex), true);
				}
			}
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003C84 File Offset: 0x00001E84
		private void UpdateAIControllers()
		{
			try
			{
				foreach (CharacterMainControl characterMainControl in this.trackedEnemies.ToList<CharacterMainControl>())
				{
					if (characterMainControl == null || characterMainControl.IsDead())
					{
						this.aiControllers.Remove(characterMainControl);
					}
					else if (!this.aiControllers.ContainsKey(characterMainControl))
					{
						AICharacterController component = characterMainControl.GetComponent<AICharacterController>();
						if (component != null)
						{
							this.aiControllers[characterMainControl] = component;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error updating AI controllers: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003D50 File Offset: 0x00001F50
		private void PerformFullScan(CharacterMainControl player, CharacterMainControl[] allCharactersCache)
		{
			try
			{
				if (allCharactersCache != null)
				{
					int num = 0;
					foreach (CharacterMainControl characterMainControl in allCharactersCache)
					{
						if (!(characterMainControl == null) && !(characterMainControl == player) && !this.trackedEnemies.Contains(characterMainControl) && !characterMainControl.IsDead() && characterMainControl.IsValidEnemy(player))
						{
							this.AddNewEnemy(characterMainControl, player);
							num++;
						}
					}
					if (num > 0)
					{
						LogManager logManager = this.logManager;
						if (logManager != null)
						{
							logManager.WriteLog(string.Format("Full scan found {0} new enemies", num), false);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error in full scan: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003E14 File Offset: 0x00002014
		private void AddNewEnemy(CharacterMainControl enemy, CharacterMainControl player)
		{
			try
			{
				if (!(enemy == null) && !this.trackedEnemies.Contains(enemy))
				{
					this.trackedEnemies.Add(enemy);
					this.ProcessNewEnemy(enemy, player);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error adding new enemy: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003E84 File Offset: 0x00002084
		private void ProcessNewEnemy(CharacterMainControl character, CharacterMainControl player)
		{
			try
			{
				long num = EnemyValueCalculator.CalculateEnemyInventoryValue(character);
				this.enemyValues[character] = num;
				if (num >= this.settings.HighValueThreshold && !this.alertedHighValue.Contains(character) && player != null)
				{
					player.PopText(string.Format("!!! 高价值目标: ${0:N0} !!!", num), 10f);
					this.alertedHighValue.Add(character);
				}
				if (this.settings.EnablePMCAlert && (character.Team == Teams.usec || character.Team == Teams.bear) && !this.alertedPMCs.Contains(character) && player != null)
				{
					string text = ((character.Team == Teams.usec) ? "USEC" : "BEAR");
					player.PopText("发现PMC: " + text, 8f);
					this.alertedPMCs.Add(character);
				}
				string weaponName = EnemyInfoHelper.GetWeaponName(character);
				string enemyName = EnemyInfoHelper.GetEnemyName(character);
				if (this.settings.EnableTraderAlert && enemyName == "Enemy" && weaponName == "无" && !this.alertedTraders.Contains(character) && player != null)
				{
					player.PopText("发现神秘商人,准备好你的钱哦!", 10f);
					this.alertedTraders.Add(character);
				}
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("New enemy tracked: {0}, Value: ${1:N0}", enemyName, num), false);
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error processing new enemy: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004030 File Offset: 0x00002230
		private void CleanupDeadEnemies()
		{
			try
			{
				List<CharacterMainControl> list = new List<CharacterMainControl>();
				foreach (CharacterMainControl characterMainControl in this.trackedEnemies)
				{
					if (characterMainControl == null || characterMainControl.IsDead())
					{
						list.Add(characterMainControl);
					}
				}
				foreach (CharacterMainControl characterMainControl2 in list)
				{
					this.RemoveEnemy(characterMainControl2);
				}
				if (list.Count > 0)
				{
					LogManager logManager = this.logManager;
					if (logManager != null)
					{
						logManager.WriteLog(string.Format("Cleaned up {0} dead enemies", list.Count), false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error in cleanup: {0}", ex), true);
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000413C File Offset: 0x0000233C
		private void RemoveEnemy(CharacterMainControl enemy)
		{
			this.trackedEnemies.Remove(enemy);
			this.aiControllers.Remove(enemy);
			this.enemyValues.Remove(enemy);
			this.alertedHighValue.Remove(enemy);
			this.alertedPMCs.Remove(enemy);
			this.alertedTraders.Remove(enemy);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004198 File Offset: 0x00002398
		private void UpdateESPData(CharacterMainControl player)
		{
			try
			{
				this.enemyInfoList.Clear();
				if (!(((player != null) ? player.mainDamageReceiver : null) == null))
				{
					Vector3 playerPosition = EnemyInfoHelper.GetPlayerPosition(player);
					foreach (CharacterMainControl characterMainControl in this.trackedEnemies.ToList<CharacterMainControl>())
					{
						if (!(characterMainControl == null) && !characterMainControl.IsDead())
						{
							try
							{
								long num = (this.enemyValues.ContainsKey(characterMainControl) ? this.enemyValues[characterMainControl] : 0L);
								Vector3 enemyHeadPosition = EnemyInfoHelper.GetEnemyHeadPosition(characterMainControl);
								float num2 = Vector3.Distance(playerPosition, enemyHeadPosition);
								if (num2 <= this.settings.MaxUIDistance)
								{
									bool flag = this.CheckIfAimingAtPlayer(characterMainControl, player);
									Health health = characterMainControl.Health;
									float num3 = ((health != null) ? health.CurrentHealth : 0f);
									Health health2 = characterMainControl.Health;
									float num4 = num3 / Mathf.Max((health2 != null) ? health2.MaxHealth : 1f, 1f);
									Health health3 = characterMainControl.Health;
									float num5 = ((health3 != null) ? health3.CurrentHealth : 0f);
									Health health4 = characterMainControl.Health;
									float num6 = ((health4 != null) ? health4.MaxHealth : 0f);
									string weaponName = EnemyInfoHelper.GetWeaponName(characterMainControl);
									string enemyName = EnemyInfoHelper.GetEnemyName(characterMainControl);
									Color specialColor = EnemyInfoHelper.GetSpecialColor(characterMainControl, num, this.settings.HighValueThreshold);
									this.enemyInfoList.Add(new EnemyInfo
									{
										Name = enemyName,
										HealthPercent = num4,
										CurrentHealth = num5,
										MaxHealth = num6,
										Distance = num2,
										Weapon = weaponName,
										Value = num,
										IsAimingAtPlayer = flag,
										SpecialColor = specialColor,
										WorldPosition = enemyHeadPosition,
										Character = characterMainControl
									});
								}
							}
							catch (Exception ex)
							{
								LogManager logManager = this.logManager;
								if (logManager != null)
								{
									logManager.WriteLog(string.Format("Error processing enemy info: {0}", ex), true);
								}
							}
						}
					}
					this.enemyInfoList.Sort(delegate(EnemyInfo a, EnemyInfo b)
					{
						int num7 = b.Value.CompareTo(a.Value);
						if (num7 != 0)
						{
							return num7;
						}
						return a.Distance.CompareTo(b.Distance);
					});
				}
			}
			catch (Exception ex2)
			{
				LogManager logManager2 = this.logManager;
				if (logManager2 != null)
				{
					logManager2.WriteLog(string.Format("Error updating ESP data: {0}", ex2), true);
				}
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004424 File Offset: 0x00002624
		private bool CheckIfAimingAtPlayer(CharacterMainControl enemy, CharacterMainControl player)
		{
			try
			{
				AICharacterController aicharacterController;
				if (this.aiControllers.TryGetValue(enemy, out aicharacterController))
				{
					return aicharacterController.aimTarget != null && aicharacterController.aimTarget.gameObject == player.mainDamageReceiver.gameObject;
				}
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error checking aim: {0}", ex), true);
				}
			}
			return false;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000044A8 File Offset: 0x000026A8
		public void Dispose()
		{
			try
			{
				this.UnregisterEvents();
				this.trackedEnemies.Clear();
				this.aiControllers.Clear();
				this.enemyValues.Clear();
				this.enemyInfoList.Clear();
				this.alertedHighValue.Clear();
				this.alertedPMCs.Clear();
				this.alertedTraders.Clear();
			}
			catch (Exception ex)
			{
				LogManager logManager = this.logManager;
				if (logManager != null)
				{
					logManager.WriteLog(string.Format("Error disposing EnemyDetector: {0}", ex), true);
				}
			}
		}

		// Token: 0x04000017 RID: 23
		private ESPSettings settings;

		// Token: 0x04000018 RID: 24
		private LogManager logManager;

		// Token: 0x04000019 RID: 25
		private HashSet<CharacterMainControl> trackedEnemies = new HashSet<CharacterMainControl>();

		// Token: 0x0400001A RID: 26
		private HashSet<CharacterMainControl> deadEnemies = new HashSet<CharacterMainControl>();

		// Token: 0x0400001B RID: 27
		private List<EnemyInfo> enemyInfoList = new List<EnemyInfo>();

		// Token: 0x0400001C RID: 28
		private Dictionary<CharacterMainControl, AICharacterController> aiControllers = new Dictionary<CharacterMainControl, AICharacterController>();

		// Token: 0x0400001D RID: 29
		private Dictionary<CharacterMainControl, long> enemyValues = new Dictionary<CharacterMainControl, long>();

		// Token: 0x0400001E RID: 30
		private HashSet<CharacterMainControl> alertedHighValue = new HashSet<CharacterMainControl>();

		// Token: 0x0400001F RID: 31
		private HashSet<CharacterMainControl> alertedPMCs = new HashSet<CharacterMainControl>();

		// Token: 0x04000020 RID: 32
		private HashSet<CharacterMainControl> alertedTraders = new HashSet<CharacterMainControl>();

		// Token: 0x04000021 RID: 33
		private float lastAIControllerUpdateTime;

		// Token: 0x04000022 RID: 34
		private float lastCleanupTime;

		// Token: 0x04000023 RID: 35
		private float lastFullScanTime;

		// Token: 0x04000024 RID: 36
		private const float aiControllerUpdateInterval = 3f;

		// Token: 0x04000025 RID: 37
		private const float cleanupInterval = 10f;

		// Token: 0x04000026 RID: 38
		private const float fullScanInterval = 30f;

		// Token: 0x04000027 RID: 39
		private const float espUpdateInterval = 0.1f;

		// Token: 0x04000028 RID: 40
		private bool eventsRegistered;
	}
}
