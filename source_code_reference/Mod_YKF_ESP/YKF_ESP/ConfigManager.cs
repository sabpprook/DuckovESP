using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x0200000A RID: 10
	[NullableContext(1)]
	[Nullable(0)]
	public class ConfigManager
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002884 File Offset: 0x00000A84
		public ConfigManager(string modPath)
		{
			this.configPath = Path.Combine(modPath, "YKF_ESP.ini");
			this.windowPosPath = Path.Combine(modPath, "WindowPosition.ini");
			this.backupPath = Path.Combine(modPath, "Backups");
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000028C0 File Offset: 0x00000AC0
		public ESPSettings LoadSettings(ESPSettings currentSettings = null)
		{
			ESPSettings espsettings = new ESPSettings();
			try
			{
				if (!File.Exists(this.configPath))
				{
					Debug.Log("[YKF_ESP] Config file not found, creating default config with version 1.2.0");
					this.CreateDefaultConfig();
					if (currentSettings != null)
					{
						espsettings.MagicBulletActive = currentSettings.MagicBulletActive;
					}
					return espsettings;
				}
				string[] array = File.ReadAllLines(this.configPath);
				ConfigManager.VersionCheckResult versionCheckResult = this.CheckVersionCompatibility(array);
				if (versionCheckResult.NeedsUpdate)
				{
					Debug.Log("[YKF_ESP] Version update detected: " + versionCheckResult.CurrentVersion + " -> 1.2.0");
					this.BackupOldConfig(versionCheckResult.CurrentVersion);
					Dictionary<string, string> dictionary = this.ParseUserSettings(array);
					this.CreateDefaultConfig();
					this.ApplyUserSettings(dictionary, espsettings);
					Debug.Log("[YKF_ESP] Configuration updated to version 1.2.0, user settings preserved");
				}
				else
				{
					if (this.CheckForReset(array))
					{
						this.CreateDefaultConfig();
						array = File.ReadAllLines(this.configPath);
					}
					this.ParseConfigLines(array, espsettings);
				}
				this.LoadWindowPosition(espsettings);
				if (currentSettings != null)
				{
					espsettings.MagicBulletActive = currentSettings.MagicBulletActive;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error loading config: {0}", ex));
				this.CreateDefaultConfig();
				if (currentSettings != null)
				{
					espsettings.MagicBulletActive = currentSettings.MagicBulletActive;
				}
			}
			return espsettings;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000029E4 File Offset: 0x00000BE4
		private ConfigManager.VersionCheckResult CheckVersionCompatibility(string[] lines)
		{
			ConfigManager.VersionCheckResult versionCheckResult = new ConfigManager.VersionCheckResult
			{
				CurrentVersion = "Unknown",
				TargetVersion = "1.2.0",
				NeedsUpdate = true
			};
			try
			{
				foreach (string text in lines)
				{
					if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#") && !text.StartsWith(";"))
					{
						string[] array = text.Split('=', StringSplitOptions.None);
						if (array.Length == 2 && array[0].Trim().ToLower() == "version")
						{
							versionCheckResult.CurrentVersion = array[1].Trim();
							break;
						}
					}
				}
				if (versionCheckResult.CurrentVersion == "1.2.0")
				{
					versionCheckResult.NeedsUpdate = false;
				}
				else
				{
					versionCheckResult.NeedsUpdate = true;
					Debug.Log("[YKF_ESP] Version mismatch: Config=" + versionCheckResult.CurrentVersion + ", Required=1.2.0");
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[YKF_ESP] Error checking version compatibility: {0}", ex));
				versionCheckResult.NeedsUpdate = true;
			}
			return versionCheckResult;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002B04 File Offset: 0x00000D04
		private Dictionary<string, string> ParseUserSettings(string[] lines)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				"ShowEnemyList", "ShowLines", "LineColor", "EnablePMCAlert", "EnableTraderAlert", "MaxDistance", "MaxLineDistance", "MaxUIDistance", "LogToFile", "LineWidth",
				"EnemyListDistanceFontSize", "WindowOpacity", "WindowWidth", "WindowHeight", "HighValueThreshold", "ToggleKey", "PlayBGM", "MagicBulletRange", "MagicBulletToggleKey"
			};
			try
			{
				foreach (string text in lines)
				{
					if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#") && !text.StartsWith(";"))
					{
						string[] array = text.Split('=', StringSplitOptions.None);
						if (array.Length == 2)
						{
							string text2 = array[0].Trim();
							string text3 = array[1].Trim();
							if (hashSet.Contains(text2))
							{
								dictionary[text2] = text3;
								Debug.Log("[YKF_ESP] Preserving user setting: " + text2 + "=" + text3);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[YKF_ESP] Error parsing user settings: {0}", ex));
			}
			return dictionary;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002CC8 File Offset: 0x00000EC8
		private void ApplyUserSettings(Dictionary<string, string> userSettings, ESPSettings settings)
		{
			try
			{
				if (File.Exists(this.configPath))
				{
					List<string> list = File.ReadAllLines(this.configPath).ToList<string>();
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						string text = list[i];
						if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#") && !text.StartsWith(";"))
						{
							string[] array = text.Split('=', StringSplitOptions.None);
							if (array.Length == 2)
							{
								string text2 = array[0].Trim();
								if (userSettings.ContainsKey(text2))
								{
									list[i] = text2 + "=" + userSettings[text2];
									flag = true;
								}
							}
						}
					}
					if (flag)
					{
						File.WriteAllLines(this.configPath, list);
						Debug.Log(string.Format("[YKF_ESP] Applied {0} preserved user settings to new config", userSettings.Count));
						this.ParseConfigLines(list.ToArray(), settings);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[YKF_ESP] Error applying user settings: {0}", ex));
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002DD4 File Offset: 0x00000FD4
		private void BackupOldConfig(string version)
		{
			try
			{
				if (!Directory.Exists(this.backupPath))
				{
					Directory.CreateDirectory(this.backupPath);
				}
				string text = DateTime.Now.ToString("yyyyMMdd_HHmmss");
				string text2 = string.Concat(new string[] { "YKF_ESP_v", version, "_", text, ".ini.backup" });
				string text3 = Path.Combine(this.backupPath, text2);
				File.Copy(this.configPath, text3, true);
				Debug.Log("[YKF_ESP] Old config backed up to: " + text2);
				this.CleanupOldBackups();
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[YKF_ESP] Error backing up old config: {0}", ex));
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002E90 File Offset: 0x00001090
		private void CleanupOldBackups()
		{
			try
			{
				if (Directory.Exists(this.backupPath))
				{
					List<FileInfo> list = (from f in Directory.GetFiles(this.backupPath, "*.backup")
						select new FileInfo(f) into f
						orderby f.CreationTime descending
						select f).ToList<FileInfo>();
					if (list.Count > 10)
					{
						for (int i = 10; i < list.Count; i++)
						{
							try
							{
								list[i].Delete();
								Debug.Log("[YKF_ESP] Cleaned up old backup: " + list[i].Name);
							}
							catch (Exception ex)
							{
								Debug.LogError(string.Format("[YKF_ESP] Error deleting backup file {0}: {1}", list[i].Name, ex));
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Debug.LogError(string.Format("[YKF_ESP] Error cleaning up backups: {0}", ex2));
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002FA4 File Offset: 0x000011A4
		public void SaveWindowPosition(float x, float y)
		{
			try
			{
				string[] array = new string[]
				{
					"# YKF_ESP 窗口位置保存文件",
					"# 此文件会自动更新，请勿手动修改",
					"",
					string.Format("WindowPosX={0}", x),
					string.Format("WindowPosY={0}", y)
				};
				File.WriteAllLines(this.windowPosPath, array);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error saving window position: {0}", ex));
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003028 File Offset: 0x00001228
		private void LoadWindowPosition(ESPSettings settings)
		{
			try
			{
				if (File.Exists(this.windowPosPath))
				{
					foreach (string text in File.ReadAllLines(this.windowPosPath))
					{
						if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#"))
						{
							string[] array2 = text.Split('=', StringSplitOptions.None);
							if (array2.Length == 2)
							{
								string text2 = array2[0].Trim();
								string text3 = array2[1].Trim();
								string text4 = text2.ToLower();
								float num2;
								if (!(text4 == "windowposx"))
								{
									if (text4 == "windowposy")
									{
										float num;
										if (float.TryParse(text3, out num))
										{
											settings.WindowPosY = Mathf.Clamp(num, 0f, (float)(Screen.height - 100));
										}
									}
								}
								else if (float.TryParse(text3, out num2))
								{
									settings.WindowPosX = Mathf.Clamp(num2, 0f, (float)(Screen.width - 100));
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error loading window position: {0}", ex));
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000314C File Offset: 0x0000134C
		private bool CheckForReset(string[] lines)
		{
			foreach (string text in lines)
			{
				if (!string.IsNullOrWhiteSpace(text) && text.StartsWith("ResetToDefault=", StringComparison.OrdinalIgnoreCase))
				{
					string[] array = text.Split('=', StringSplitOptions.None);
					bool flag;
					if (array.Length == 2 && bool.TryParse(array[1].Trim(), out flag))
					{
						return flag;
					}
				}
			}
			return false;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000031A8 File Offset: 0x000013A8
		private void ParseConfigLines(string[] lines, ESPSettings settings)
		{
			foreach (string text in lines)
			{
				if (!string.IsNullOrWhiteSpace(text) && !text.StartsWith("#") && !text.StartsWith(";"))
				{
					string[] array = text.Split('=', StringSplitOptions.None);
					if (array.Length == 2)
					{
						string text2 = array[0].Trim();
						string text3 = array[1].Trim();
						string text4 = text2.ToLower();
						if (text4 != null)
						{
							switch (text4.Length)
							{
							case 7:
							{
								char c = text4[0];
								if (c != 'p')
								{
									if (c == 'v')
									{
										if (text4 == "version")
										{
											settings.Version = text3;
										}
									}
								}
								else if (text4 == "playbgm")
								{
									settings.PlayBGM = bool.Parse(text3);
								}
								break;
							}
							case 9:
							{
								char c = text4[6];
								if (c != 'd')
								{
									switch (c)
									{
									case 'i':
										if (text4 == "logtofile")
										{
											settings.LogToFile = bool.Parse(text3);
										}
										break;
									case 'k':
										if (text4 == "togglekey")
										{
											settings.ToggleKey = text3;
										}
										break;
									case 'l':
										if (text4 == "linecolor")
										{
											settings.LineColor = this.ParseColor(text3, Color.green);
										}
										break;
									case 'n':
										if (text4 == "showlines")
										{
											settings.ShowLines = bool.Parse(text3);
										}
										break;
									}
								}
								else if (text4 == "linewidth")
								{
									settings.LineWidth = float.Parse(text3);
								}
								break;
							}
							case 11:
							{
								char c = text4[0];
								if (c != 'm')
								{
									if (c == 'w')
									{
										if (text4 == "windowwidth")
										{
											float num;
											if (float.TryParse(text3, out num))
											{
												settings.WindowWidth = Mathf.Clamp(num, 250f, 800f);
											}
										}
									}
								}
								else if (text4 == "maxdistance")
								{
									settings.MaxDistance = float.Parse(text3);
								}
								break;
							}
							case 12:
								if (text4 == "windowheight")
								{
									float num2;
									if (float.TryParse(text3, out num2))
									{
										settings.WindowHeight = Mathf.Clamp(num2, 200f, 600f);
									}
								}
								break;
							case 13:
							{
								char c = text4[0];
								if (c != 'm')
								{
									if (c != 's')
									{
										if (c == 'w')
										{
											if (text4 == "windowopacity")
											{
												float num3;
												if (float.TryParse(text3, out num3))
												{
													settings.WindowOpacity = Mathf.Clamp01(num3);
												}
											}
										}
									}
									else if (text4 == "showenemylist")
									{
										settings.ShowEnemyList = bool.Parse(text3);
									}
								}
								else if (text4 == "maxuidistance")
								{
									settings.MaxUIDistance = float.Parse(text3);
								}
								break;
							}
							case 14:
								if (text4 == "enablepmcalert")
								{
									settings.EnablePMCAlert = bool.Parse(text3);
								}
								break;
							case 15:
								if (text4 == "maxlinedistance")
								{
									settings.MaxLineDistance = float.Parse(text3);
								}
								break;
							case 16:
								if (text4 == "magicbulletrange")
								{
									float num4;
									if (float.TryParse(text3, out num4))
									{
										settings.MagicBulletRange = Mathf.Clamp(num4, 5f, 100f);
									}
								}
								break;
							case 17:
								if (text4 == "enabletraderalert")
								{
									settings.EnableTraderAlert = bool.Parse(text3);
								}
								break;
							case 18:
								if (text4 == "highvaluethreshold")
								{
									long num5;
									if (long.TryParse(text3, out num5))
									{
										settings.HighValueThreshold = Math.Max(0L, num5);
									}
								}
								break;
							case 20:
								if (text4 == "magicbullettogglekey")
								{
									settings.MagicBulletToggleKey = text3;
								}
								break;
							case 25:
								if (text4 == "enemylistdistancefontsize")
								{
									int num6;
									if (int.TryParse(text3, out num6))
									{
										settings.EnemyListDistanceFontSize = num6;
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000036A8 File Offset: 0x000018A8
		private Color ParseColor(string colorString, Color defaultColor)
		{
			try
			{
				string[] array = colorString.Split(',', StringSplitOptions.None);
				if (array.Length == 4)
				{
					return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error parsing color '{0}': {1}", colorString, ex));
			}
			return defaultColor;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003718 File Offset: 0x00001918
		private void CreateDefaultConfig()
		{
			try
			{
				string[] array = new string[]
				{
					"# YKF_ESP 配置文件", "# 版本控制 - 请勿手动修改此项", "Version=1.2.0", "", "# 所有设置默认为true/开启状态", "", "# 是否显示敌人列表（true=显示，false=隐藏）", "ShowEnemyList=true", "", "# 是否显示敌人连线（true=显示，false=隐藏）",
					"ShowLines=true", "", "# 连线颜色（RGBA格式,取值范围0-1）", "LineColor=0,1,0,1", "", "# 是否启用PMC警报（true=启用，false=禁用）", "EnablePMCAlert=true", "", "# 是否启用商人警报（true=启用，false=禁用）", "EnableTraderAlert=true",
					"", "# 高价值目标阈值（单位：卢布，当敌人身上物品总价值超过此值时触发高价值警报，建议值1000-100000）", "HighValueThreshold=5000", "", "# 最大显示距离（单位：米，建议值50-500）", "MaxDistance=100", "", "# 连线最大显示距离（单位：米，建议值20-50）", "MaxLineDistance=50", "",
					"# UI最大显示距离（单位：米，建议值50-80）", "MaxUIDistance=60", "", "# 是否启用日志记录（true=启用，false=禁用）", "LogToFile=true", "", "# 连线宽度（建议值1-5）", "LineWidth=2.0", "", "# 敌人列表中距离显示的字体大小（整数，建议值12-20）",
					"EnemyListDistanceFontSize=14", "", "# 窗口透明度（取值范围0.1-1.0，1.0为完全不透明）", "WindowOpacity=0.85", "", "# 窗口宽度（单位：像素，建议值250-800）", "WindowWidth=310", "", "# 窗口高度（单位：像素，建议值200-600）", "WindowHeight=240",
					"", "# 切换窗口显示的按键（支持字母、数字、功能键等，例如：F1, F2, TAB, SPACE等）", "# 常用按键示例：F1-F12, A-Z, 0-9, TAB, ENTER, SPACE, ESC, SHIFT, CTRL, ALT", "ToggleKey=F1", "", "# 是否播放BGM（true=播放，false=不播放）", "PlayBGM=true", "", "# 魔法子弹功能配置", "# 魔法子弹攻击范围（单位：米，建议值10-50）",
					"MagicBulletRange=30", "", "# 魔法子弹功能切换按键", "MagicBulletToggleKey=F2", "", "# 重置配置为默认值（设置为true后，下次加载配置时会重置所有设置并自动恢复为false）", "ResetToDefault=false"
				};
				File.WriteAllLines(this.configPath, array);
				Debug.Log("[YKF_ESP] Created default config with version 1.2.0");
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error creating default config: {0}", ex));
			}
		}

		// Token: 0x04000013 RID: 19
		private readonly string configPath;

		// Token: 0x04000014 RID: 20
		private readonly string windowPosPath;

		// Token: 0x04000015 RID: 21
		private readonly string backupPath;

		// Token: 0x04000016 RID: 22
		private const string CURRENT_VERSION = "1.2.0";

		// Token: 0x0200001C RID: 28
		[Nullable(0)]
		private struct VersionCheckResult
		{
			// Token: 0x040000A2 RID: 162
			public bool NeedsUpdate;

			// Token: 0x040000A3 RID: 163
			public string CurrentVersion;

			// Token: 0x040000A4 RID: 164
			public string TargetVersion;
		}
	}
}
