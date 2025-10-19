using System;
using System.IO;
using Saves;
using UnityEngine;

namespace Duckov.Options
{
	// Token: 0x0200025A RID: 602
	public class OptionsManager : MonoBehaviour
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x060012AD RID: 4781 RVA: 0x000464A7 File Offset: 0x000446A7
		private static string Folder
		{
			get
			{
				return SavesSystem.SavesFolder;
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x060012AE RID: 4782 RVA: 0x000464AE File Offset: 0x000446AE
		public static string FilePath
		{
			get
			{
				return Path.Combine(OptionsManager.Folder, "Options.ES3");
			}
		}

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x060012AF RID: 4783 RVA: 0x000464C0 File Offset: 0x000446C0
		// (remove) Token: 0x060012B0 RID: 4784 RVA: 0x000464F4 File Offset: 0x000446F4
		public static event Action<string> OnOptionsChanged;

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x00046527 File Offset: 0x00044727
		private static ES3Settings SaveSettings
		{
			get
			{
				if (OptionsManager._saveSettings == null)
				{
					OptionsManager._saveSettings = new ES3Settings(true);
					OptionsManager._saveSettings.path = OptionsManager.FilePath;
					OptionsManager._saveSettings.location = ES3.Location.File;
				}
				return OptionsManager._saveSettings;
			}
		}

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x0004655A File Offset: 0x0004475A
		// (set) Token: 0x060012B3 RID: 4787 RVA: 0x0004656B File Offset: 0x0004476B
		public static float MouseSensitivity
		{
			get
			{
				return OptionsManager.Load<float>("MouseSensitivity", 10f);
			}
			set
			{
				OptionsManager.Save<float>("MouseSensitivity", value);
			}
		}

		// Token: 0x060012B4 RID: 4788 RVA: 0x00046578 File Offset: 0x00044778
		public static void Save<T>(string key, T obj)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}
			try
			{
				ES3.Save<T>(key, obj, OptionsManager.SaveSettings);
				Action<string> onOptionsChanged = OptionsManager.OnOptionsChanged;
				if (onOptionsChanged != null)
				{
					onOptionsChanged(key);
				}
				ES3.CreateBackup(OptionsManager.SaveSettings);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				Debug.LogError("Error: Failed saving options: " + key);
			}
		}

		// Token: 0x060012B5 RID: 4789 RVA: 0x000465E0 File Offset: 0x000447E0
		public static T Load<T>(string key, T defaultValue = default(T))
		{
			T t;
			if (string.IsNullOrEmpty(key))
			{
				t = default(T);
				return t;
			}
			try
			{
				if (ES3.KeyExists(key, OptionsManager.SaveSettings))
				{
					t = ES3.Load<T>(key, OptionsManager.SaveSettings);
				}
				else
				{
					ES3.Save<T>(key, defaultValue, OptionsManager.SaveSettings);
					t = defaultValue;
				}
			}
			catch
			{
				if (ES3.RestoreBackup(OptionsManager.SaveSettings))
				{
					try
					{
						if (ES3.KeyExists(key, OptionsManager.SaveSettings))
						{
							return ES3.Load<T>(key, OptionsManager.SaveSettings);
						}
						ES3.Save<T>(key, defaultValue, OptionsManager.SaveSettings);
						return defaultValue;
					}
					catch
					{
						Debug.LogError("[OPTIONS MANAGER] Failed restoring backup");
					}
				}
				ES3.DeleteFile(OptionsManager.SaveSettings);
				t = defaultValue;
			}
			return t;
		}

		// Token: 0x04000E1B RID: 3611
		public const string FileName = "Options.ES3";

		// Token: 0x04000E1D RID: 3613
		private static ES3Settings _saveSettings;
	}
}
