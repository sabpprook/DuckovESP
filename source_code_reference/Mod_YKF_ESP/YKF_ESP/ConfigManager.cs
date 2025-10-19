using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YKF_ESP
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	public class ConfigManager
	{
		// Token: 0x0600000D RID: 13 RVA: 0x0000215C File Offset: 0x0000035C
		public ConfigManager(string modPath)
		{
			this.configPath = Path.Combine(modPath, "YKF_ESP.ini");
			this.windowPosPath = Path.Combine(modPath, "WindowPosition.ini");
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002186 File Offset: 0x00000386
		public bool ShouldReloadConfig()
		{
			if (Time.time - this.lastConfigReloadTime > 60f)
			{
				this.lastConfigReloadTime = Time.time;
				return true;
			}
			return false;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021AC File Offset: 0x000003AC
		public ESPSettings LoadSettings()
		{
			ESPSettings espsettings = new ESPSettings();
			try
			{
				if (!File.Exists(this.configPath))
				{
					this.CreateDefaultConfig();
					return espsettings;
				}
				string[] array = File.ReadAllLines(this.configPath);
				if (this.CheckForReset(array))
				{
					this.CreateDefaultConfig();
					array = File.ReadAllLines(this.configPath);
				}
				this.ParseConfigLines(array, espsettings);
				this.LoadWindowPosition(espsettings);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error loading config: {0}", ex));
				this.CreateDefaultConfig();
			}
			return espsettings;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x0000223C File Offset: 0x0000043C
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

		// Token: 0x06000011 RID: 17 RVA: 0x000022C0 File Offset: 0x000004C0
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

		// Token: 0x06000012 RID: 18 RVA: 0x000023E4 File Offset: 0x000005E4
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

		// Token: 0x06000013 RID: 19 RVA: 0x00002440 File Offset: 0x00000640
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
							int length = text4.Length;
							switch (length)
							{
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
							case 10:
							case 16:
								break;
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
							case 17:
								if (text4 == "enabletraderalert")
								{
									settings.EnableTraderAlert = bool.Parse(text3);
								}
								break;
							default:
								if (length == 25)
								{
									if (text4 == "enemylistdistancefontsize")
									{
										int num4;
										if (int.TryParse(text3, out num4))
										{
											settings.EnemyListDistanceFontSize = num4;
										}
									}
								}
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002810 File Offset: 0x00000A10
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

		// Token: 0x06000015 RID: 21 RVA: 0x00002880 File Offset: 0x00000A80
		private void CreateDefaultConfig()
		{
			try
			{
				string[] array = new string[]
				{
					"# YKF_ESP 配置文件", "# 所有设置默认为true/开启状态", "", "# 是否显示敌人列表（true=显示，false=隐藏）", "ShowEnemyList=true", "", "# 是否显示敌人连线（true=显示，false=隐藏）", "ShowLines=true", "", "# 连线颜色（RGBA格式,取值范围0-1）",
					"LineColor=0,1,0,1", "", "# 是否启用PMC警报（true=启用，false=禁用）", "EnablePMCAlert=true", "", "# 是否启用商人警报（true=启用，false=禁用）", "EnableTraderAlert=true", "", "# 最大显示距离（单位：米，建议值50-500）", "MaxDistance=100",
					"", "# 连线最大显示距离（单位：米，建议值20-50）", "MaxLineDistance=50", "", "# UI最大显示距离（单位：米，建议值50-80）", "MaxUIDistance=60", "", "# 是否启用日志记录（true=启用，false=禁用）", "LogToFile=true", "",
					"# 连线宽度（建议值1-5）", "LineWidth=2.0", "", "# 敌人列表中距离显示的字体大小（整数，建议值12-20）", "EnemyListDistanceFontSize=18", "", "# 窗口透明度（取值范围0.1-1.0，1.0为完全不透明）", "WindowOpacity=0.85", "", "# 窗口宽度（单位：像素，建议值250-800）",
					"WindowWidth=350", "", "# 窗口高度（单位：像素，建议值200-600）", "WindowHeight=280", "", "# 切换窗口显示的按键（支持字母、数字、功能键等，例如：F1, F2, TAB, SPACE等）", "# 常用按键示例：F1-F12, A-Z, 0-9, TAB, ENTER, SPACE, ESC, SHIFT, CTRL, ALT", "ToggleKey=F1", "", "# 重置配置为默认值（设置为true后，下次加载配置时会重置所有设置并自动恢复为false）",
					"ResetToDefault=false"
				};
				File.WriteAllLines(this.configPath, array);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Error creating default config: {0}", ex));
			}
		}

		// Token: 0x04000006 RID: 6
		private readonly string configPath;

		// Token: 0x04000007 RID: 7
		private readonly string windowPosPath;

		// Token: 0x04000008 RID: 8
		private float lastConfigReloadTime;

		// Token: 0x04000009 RID: 9
		private const float configReloadInterval = 60f;
	}
}
