using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Saves
{
	// Token: 0x02000223 RID: 547
	public class SavesSystem
	{
		// Token: 0x170002EA RID: 746
		// (get) Token: 0x06001067 RID: 4199 RVA: 0x0003F9F4 File Offset: 0x0003DBF4
		// (set) Token: 0x06001068 RID: 4200 RVA: 0x0003FA53 File Offset: 0x0003DC53
		public static int CurrentSlot
		{
			get
			{
				if (SavesSystem._currentSlot == null)
				{
					SavesSystem._currentSlot = new int?(PlayerPrefs.GetInt("CurrentSlot", 1));
					int? currentSlot = SavesSystem._currentSlot;
					int num = 1;
					if ((currentSlot.GetValueOrDefault() < num) & (currentSlot != null))
					{
						SavesSystem._currentSlot = new int?(1);
					}
				}
				return SavesSystem._currentSlot.Value;
			}
			private set
			{
				SavesSystem._currentSlot = new int?(value);
				PlayerPrefs.SetInt("CurrentSlot", value);
				SavesSystem.CacheFile();
			}
		}

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x06001069 RID: 4201 RVA: 0x0003FA70 File Offset: 0x0003DC70
		public static string CurrentFilePath
		{
			get
			{
				return SavesSystem.GetFilePath(SavesSystem.CurrentSlot);
			}
		}

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x0003FA7C File Offset: 0x0003DC7C
		public static bool IsSaving
		{
			get
			{
				return SavesSystem.saving;
			}
		}

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x0003FA83 File Offset: 0x0003DC83
		public static string SavesFolder
		{
			get
			{
				return "Saves";
			}
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x0003FA8A File Offset: 0x0003DC8A
		public static string GetFullPathToSavesFolder()
		{
			return Path.Combine(Application.persistentDataPath, SavesSystem.SavesFolder);
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0003FA9B File Offset: 0x0003DC9B
		public static string GetFilePath(int slot)
		{
			return Path.Combine(SavesSystem.SavesFolder, SavesSystem.GetSaveFileName(slot));
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0003FAAD File Offset: 0x0003DCAD
		public static string GetSaveFileName(int slot)
		{
			return string.Format("Save_{0}.sav", slot);
		}

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x0600106F RID: 4207 RVA: 0x0003FAC0 File Offset: 0x0003DCC0
		// (remove) Token: 0x06001070 RID: 4208 RVA: 0x0003FAF4 File Offset: 0x0003DCF4
		public static event Action OnSetFile;

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x06001071 RID: 4209 RVA: 0x0003FB28 File Offset: 0x0003DD28
		// (remove) Token: 0x06001072 RID: 4210 RVA: 0x0003FB5C File Offset: 0x0003DD5C
		public static event Action OnSaveDeleted;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06001073 RID: 4211 RVA: 0x0003FB90 File Offset: 0x0003DD90
		// (remove) Token: 0x06001074 RID: 4212 RVA: 0x0003FBC4 File Offset: 0x0003DDC4
		public static event Action OnCollectSaveData;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06001075 RID: 4213 RVA: 0x0003FBF8 File Offset: 0x0003DDF8
		// (remove) Token: 0x06001076 RID: 4214 RVA: 0x0003FC2C File Offset: 0x0003DE2C
		public static event Action OnRestoreFailureDetected;

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x0003FC5F File Offset: 0x0003DE5F
		// (set) Token: 0x06001078 RID: 4216 RVA: 0x0003FC66 File Offset: 0x0003DE66
		public static bool RestoreFailureMarker { get; private set; }

		// Token: 0x06001079 RID: 4217 RVA: 0x0003FC6E File Offset: 0x0003DE6E
		public static bool IsOldSave(int index)
		{
			return !SavesSystem.KeyExisits("CreatedWithVersion", index);
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0003FC7E File Offset: 0x0003DE7E
		public static void SetFile(int index)
		{
			SavesSystem.cached = false;
			SavesSystem.CurrentSlot = index;
			Action onSetFile = SavesSystem.OnSetFile;
			if (onSetFile == null)
			{
				return;
			}
			onSetFile();
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0003FC9B File Offset: 0x0003DE9B
		public static SavesSystem.BackupInfo[] GetBackupList()
		{
			return SavesSystem.GetBackupList(SavesSystem.CurrentSlot);
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0003FCA7 File Offset: 0x0003DEA7
		public static SavesSystem.BackupInfo[] GetBackupList(int slot)
		{
			return SavesSystem.GetBackupList(SavesSystem.GetFilePath(slot), slot);
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0003FCB8 File Offset: 0x0003DEB8
		public static SavesSystem.BackupInfo[] GetBackupList(string mainPath, int slot = -1)
		{
			SavesSystem.BackupInfo[] array = new SavesSystem.BackupInfo[10];
			for (int i = 0; i < 10; i++)
			{
				try
				{
					string backupPathByIndex = SavesSystem.GetBackupPathByIndex(mainPath, i);
					ES3Settings es3Settings = new ES3Settings(backupPathByIndex, null);
					es3Settings.location = ES3.Location.File;
					bool flag = ES3.FileExists(backupPathByIndex, es3Settings);
					long num = 0L;
					if (flag && ES3.KeyExists("SaveTime", backupPathByIndex, es3Settings))
					{
						num = ES3.Load<long>("SaveTime", backupPathByIndex, es3Settings);
					}
					DateTime.FromBinary(num);
					SavesSystem.BackupInfo backupInfo = new SavesSystem.BackupInfo
					{
						slot = slot,
						index = i,
						path = backupPathByIndex,
						exists = flag,
						time_raw = num
					};
					array[i] = backupInfo;
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					array[i] = default(SavesSystem.BackupInfo);
				}
			}
			return array;
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x0003FD94 File Offset: 0x0003DF94
		private static int GetEmptyOrOldestBackupIndex()
		{
			SavesSystem.BackupInfo[] backupList = SavesSystem.GetBackupList();
			int num = -1;
			DateTime dateTime = DateTime.MaxValue;
			foreach (SavesSystem.BackupInfo backupInfo in backupList)
			{
				if (!backupInfo.exists)
				{
					return backupInfo.index;
				}
				if (backupInfo.Time < dateTime)
				{
					num = backupInfo.index;
					dateTime = backupInfo.Time;
				}
			}
			return num;
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0003FDF8 File Offset: 0x0003DFF8
		private static int GetOldestBackupIndex()
		{
			SavesSystem.BackupInfo[] backupList = SavesSystem.GetBackupList();
			int num = -1;
			DateTime dateTime = DateTime.MaxValue;
			foreach (SavesSystem.BackupInfo backupInfo in backupList)
			{
				if (backupInfo.exists && backupInfo.Time < dateTime)
				{
					num = backupInfo.index;
					dateTime = backupInfo.Time;
				}
			}
			return num;
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0003FE54 File Offset: 0x0003E054
		private static int GetNewestBackupIndex()
		{
			SavesSystem.BackupInfo[] backupList = SavesSystem.GetBackupList();
			int num = -1;
			DateTime dateTime = DateTime.MinValue;
			foreach (SavesSystem.BackupInfo backupInfo in backupList)
			{
				if (backupInfo.exists && backupInfo.Time > dateTime)
				{
					num = backupInfo.index;
					dateTime = backupInfo.Time;
				}
			}
			return num;
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x0003FEAF File Offset: 0x0003E0AF
		private static string GetBackupPathByIndex(int index)
		{
			return SavesSystem.GetBackupPathByIndex(SavesSystem.CurrentSlot, index);
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0003FEBC File Offset: 0x0003E0BC
		private static string GetBackupPathByIndex(int slot, int index)
		{
			return SavesSystem.GetBackupPathByIndex(SavesSystem.GetFilePath(slot), index);
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0003FECA File Offset: 0x0003E0CA
		private static string GetBackupPathByIndex(string path, int index)
		{
			return string.Format("{0}.bac.{1:00}", path, index + 1);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0003FEE0 File Offset: 0x0003E0E0
		private static void CreateIndexedBackup(int index = -1)
		{
			SavesSystem.LastIndexedBackupTime = DateTime.UtcNow;
			try
			{
				if (index < 0)
				{
					index = SavesSystem.GetEmptyOrOldestBackupIndex();
				}
				string backupPathByIndex = SavesSystem.GetBackupPathByIndex(index);
				ES3.DeleteFile(backupPathByIndex, SavesSystem.settings);
				ES3.CopyFile(SavesSystem.CurrentFilePath, backupPathByIndex);
				ES3.StoreCachedFile(backupPathByIndex);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				Debug.Log("[Saves] Failed creating indexed backup");
			}
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x0003FF48 File Offset: 0x0003E148
		private static void CreateBackup()
		{
			try
			{
				SavesSystem.CreateBackup(SavesSystem.CurrentFilePath);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				Debug.Log("[Saves] Failed creating backup");
			}
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x0003FF84 File Offset: 0x0003E184
		private static void CreateBackup(string path)
		{
			try
			{
				string text = path + ".bac";
				ES3.DeleteFile(text, SavesSystem.settings);
				ES3.CreateBackup(path);
				ES3.StoreCachedFile(text);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				Debug.Log("[Saves] Failed creating backup for path " + path);
			}
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0003FFDC File Offset: 0x0003E1DC
		public static void UpgradeSaveFileAssemblyInfo(string path)
		{
			if (!File.Exists(path))
			{
				Debug.Log("没有找到存档文件：" + path);
				return;
			}
			string text;
			using (StreamReader streamReader = File.OpenText(path))
			{
				text = streamReader.ReadToEnd();
				if (text.Contains("TeamSoda.Duckov.Core"))
				{
					streamReader.Close();
					return;
				}
				text = text.Replace("Assembly-CSharp", "TeamSoda.Duckov.Core");
				streamReader.Close();
			}
			File.Delete(path);
			using (FileStream fileStream = File.OpenWrite(path))
			{
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(text);
				streamWriter.Close();
				fileStream.Close();
			}
			Debug.Log("存档格式已更新：" + path);
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x000400A8 File Offset: 0x0003E2A8
		public static void RestoreIndexedBackup(int slot, int index)
		{
			string backupPathByIndex = SavesSystem.GetBackupPathByIndex(slot, index);
			SavesSystem.UpgradeSaveFileAssemblyInfo(Path.Combine(Application.persistentDataPath, backupPathByIndex));
			string filePath = SavesSystem.GetFilePath(slot);
			string text = filePath + ".bac";
			try
			{
				ES3.CacheFile(backupPathByIndex);
				ES3.DeleteFile(text, SavesSystem.settings);
				ES3.CopyFile(backupPathByIndex, text);
				ES3.DeleteFile(filePath, SavesSystem.settings);
				ES3.RestoreBackup(filePath, SavesSystem.settings);
				ES3.StoreCachedFile(filePath);
				ES3.CacheFile(filePath);
				Action onSetFile = SavesSystem.OnSetFile;
				if (onSetFile != null)
				{
					onSetFile();
				}
			}
			catch
			{
				SavesSystem.RestoreFailureMarker = true;
				Debug.LogError("文件损坏，且无法修复。");
				ES3.DeleteFile(filePath);
				File.Delete(filePath);
				ES3.Save<bool>("Created", true, filePath);
				ES3.StoreCachedFile(filePath);
				ES3.CacheFile(filePath);
				Action onRestoreFailureDetected = SavesSystem.OnRestoreFailureDetected;
				if (onRestoreFailureDetected != null)
				{
					onRestoreFailureDetected();
				}
			}
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00040184 File Offset: 0x0003E384
		private static bool RestoreBackup(string path)
		{
			bool flag = false;
			try
			{
				string text = path + ".bac";
				SavesSystem.UpgradeSaveFileAssemblyInfo(Path.Combine(Application.persistentDataPath, text));
				ES3.CacheFile(text);
				ES3.DeleteFile(path, SavesSystem.settings);
				ES3.RestoreBackup(path, SavesSystem.settings);
				ES3.StoreCachedFile(path);
				ES3.CacheFile(path);
				ES3.CacheFile(path);
				flag = true;
			}
			catch
			{
				Debug.Log("默认备份损坏。");
			}
			if (!flag)
			{
				SavesSystem.RestoreFailureMarker = true;
				Debug.LogError("恢复默认备份失败");
				ES3.DeleteFile(path);
				ES3.Save<bool>("Created", true, path);
				ES3.StoreCachedFile(path);
				ES3.CacheFile(path);
				Action onRestoreFailureDetected = SavesSystem.OnRestoreFailureDetected;
				if (onRestoreFailureDetected != null)
				{
					onRestoreFailureDetected();
				}
			}
			return flag;
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600108A RID: 4234 RVA: 0x00040244 File Offset: 0x0003E444
		// (set) Token: 0x0600108B RID: 4235 RVA: 0x0004026B File Offset: 0x0003E46B
		private static DateTime LastSavedTime
		{
			get
			{
				if (SavesSystem._lastSavedTime > DateTime.UtcNow)
				{
					SavesSystem._lastSavedTime = DateTime.UtcNow;
					GameManager.TimeTravelDetected();
				}
				return SavesSystem._lastSavedTime;
			}
			set
			{
				SavesSystem._lastSavedTime = value;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x0600108C RID: 4236 RVA: 0x00040273 File Offset: 0x0003E473
		private static TimeSpan TimeSinceLastSave
		{
			get
			{
				return DateTime.UtcNow - SavesSystem.LastSavedTime;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x0600108D RID: 4237 RVA: 0x00040284 File Offset: 0x0003E484
		// (set) Token: 0x0600108E RID: 4238 RVA: 0x000402AB File Offset: 0x0003E4AB
		private static DateTime LastIndexedBackupTime
		{
			get
			{
				if (SavesSystem._lastIndexedBackupTime > DateTime.UtcNow)
				{
					SavesSystem._lastIndexedBackupTime = DateTime.UtcNow;
					GameManager.TimeTravelDetected();
				}
				return SavesSystem._lastIndexedBackupTime;
			}
			set
			{
				SavesSystem._lastIndexedBackupTime = value;
			}
		}

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x000402B3 File Offset: 0x0003E4B3
		private static TimeSpan TimeSinceLastIndexedBackup
		{
			get
			{
				return DateTime.UtcNow - SavesSystem.LastIndexedBackupTime;
			}
		}

		// Token: 0x06001090 RID: 4240 RVA: 0x000402C4 File Offset: 0x0003E4C4
		public DateTime GetSaveTimeUTC(int slot = -1)
		{
			if (slot < 0)
			{
				slot = SavesSystem.CurrentSlot;
			}
			if (!SavesSystem.KeyExisits("SaveTime", slot))
			{
				return default(DateTime);
			}
			return DateTime.FromBinary(SavesSystem.Load<long>("SaveTime", slot));
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00040304 File Offset: 0x0003E504
		public DateTime GetSaveTimeLocal(int slot = -1)
		{
			if (slot < 0)
			{
				slot = SavesSystem.CurrentSlot;
			}
			DateTime saveTimeUTC = this.GetSaveTimeUTC(slot);
			if (saveTimeUTC == default(DateTime))
			{
				return default(DateTime);
			}
			return saveTimeUTC.ToLocalTime();
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x00040348 File Offset: 0x0003E548
		public static void SaveFile(bool writeSaveTime = true)
		{
			TimeSpan timeSinceLastIndexedBackup = SavesSystem.TimeSinceLastIndexedBackup;
			SavesSystem.LastSavedTime = DateTime.UtcNow;
			if (writeSaveTime)
			{
				SavesSystem.Save<long>("SaveTime", DateTime.UtcNow.ToBinary());
			}
			SavesSystem.saving = true;
			SavesSystem.CreateBackup();
			if (timeSinceLastIndexedBackup > TimeSpan.FromMinutes(5.0))
			{
				SavesSystem.CreateIndexedBackup(-1);
			}
			SavesSystem.SetAsOldGame();
			ES3.StoreCachedFile(SavesSystem.CurrentFilePath);
			SavesSystem.saving = false;
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000403B9 File Offset: 0x0003E5B9
		private static void CacheFile()
		{
			SavesSystem.CacheFile(SavesSystem.CurrentSlot);
			SavesSystem.cached = true;
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x000403CC File Offset: 0x0003E5CC
		private static void CacheFile(int slot)
		{
			if (slot == SavesSystem.CurrentSlot && SavesSystem.cached)
			{
				return;
			}
			string filePath = SavesSystem.GetFilePath(slot);
			if (!SavesSystem.CacheFile(filePath))
			{
				Debug.Log("尝试恢复 indexed backups");
				List<SavesSystem.BackupInfo> list = (from e in SavesSystem.GetBackupList(filePath, slot)
					where e.exists
					select e).ToList<SavesSystem.BackupInfo>();
				list.Sort(delegate(SavesSystem.BackupInfo a, SavesSystem.BackupInfo b)
				{
					if (!(a.Time > b.Time))
					{
						return 1;
					}
					return -1;
				});
				if (list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						SavesSystem.BackupInfo backupInfo = list[i];
						try
						{
							Debug.Log(string.Format("Restoreing {0}.bac.{1} \t", slot, backupInfo.index) + backupInfo.Time.ToString("MM/dd HH:mm:ss"));
							SavesSystem.RestoreIndexedBackup(slot, backupInfo.index);
							break;
						}
						catch
						{
							Debug.LogError(string.Format("slot:{0} backup_index:{1} 恢复失败。", slot, backupInfo.index));
						}
					}
				}
			}
			if (!ES3.FileExists(filePath))
			{
				ES3.Save<bool>("Created", true, filePath);
				ES3.StoreCachedFile(filePath);
				ES3.CacheFile(filePath);
			}
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x00040528 File Offset: 0x0003E728
		private static bool CacheFile(string path)
		{
			bool flag;
			try
			{
				ES3.CacheFile(path);
				flag = true;
			}
			catch
			{
				flag = SavesSystem.RestoreBackup(path);
			}
			return flag;
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x0004055C File Offset: 0x0003E75C
		public static void Save<T>(string prefix, string key, T value)
		{
			SavesSystem.Save<T>(prefix + key, value);
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x0004056B File Offset: 0x0003E76B
		public static void Save<T>(string realKey, T value)
		{
			if (!SavesSystem.cached)
			{
				SavesSystem.CacheFile();
			}
			if (string.IsNullOrWhiteSpace(SavesSystem.CurrentFilePath))
			{
				Debug.Log("Save failed " + realKey);
				return;
			}
			ES3.Save<T>(realKey, value, SavesSystem.CurrentFilePath);
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x000405A2 File Offset: 0x0003E7A2
		public static T Load<T>(string prefix, string key)
		{
			return SavesSystem.Load<T>(prefix + key);
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x000405B0 File Offset: 0x0003E7B0
		public static T Load<T>(string realKey)
		{
			if (!SavesSystem.cached)
			{
				SavesSystem.CacheFile();
			}
			string.IsNullOrWhiteSpace(realKey);
			if (ES3.KeyExists(realKey, SavesSystem.CurrentFilePath))
			{
				return ES3.Load<T>(realKey, SavesSystem.CurrentFilePath);
			}
			return default(T);
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x000405F2 File Offset: 0x0003E7F2
		public static bool KeyExisits(string prefix, string key)
		{
			return ES3.KeyExists(prefix + key);
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x00040600 File Offset: 0x0003E800
		public static bool KeyExisits(string realKey)
		{
			if (!SavesSystem.cached)
			{
				SavesSystem.CacheFile();
			}
			return ES3.KeyExists(realKey, SavesSystem.CurrentFilePath);
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x0004061C File Offset: 0x0003E81C
		public static bool KeyExisits(string realKey, int slotIndex)
		{
			if (slotIndex == SavesSystem.CurrentSlot)
			{
				return SavesSystem.KeyExisits(realKey);
			}
			string filePath = SavesSystem.GetFilePath(slotIndex);
			SavesSystem.CacheFile(slotIndex);
			return ES3.KeyExists(realKey, filePath);
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0004064C File Offset: 0x0003E84C
		public static T Load<T>(string realKey, int slotIndex)
		{
			if (slotIndex == SavesSystem.CurrentSlot)
			{
				return SavesSystem.Load<T>(realKey);
			}
			string filePath = SavesSystem.GetFilePath(slotIndex);
			SavesSystem.CacheFile(slotIndex);
			if (ES3.KeyExists(realKey, filePath))
			{
				return ES3.Load<T>(realKey, filePath);
			}
			return default(T);
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600109E RID: 4254 RVA: 0x0004068F File Offset: 0x0003E88F
		public static string GlobalSaveDataFilePath
		{
			get
			{
				return Path.Combine(SavesSystem.SavesFolder, SavesSystem.GlobalSaveDataFileName);
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600109F RID: 4255 RVA: 0x000406A0 File Offset: 0x0003E8A0
		public static string GlobalSaveDataFileName
		{
			get
			{
				return "Global.json";
			}
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000406A7 File Offset: 0x0003E8A7
		public static void SaveGlobal<T>(string key, T value)
		{
			if (!SavesSystem.globalCached)
			{
				SavesSystem.CacheFile(SavesSystem.GlobalSaveDataFilePath);
				SavesSystem.globalCached = true;
			}
			ES3.Save<T>(key, value, SavesSystem.GlobalSaveDataFilePath);
			SavesSystem.CreateBackup(SavesSystem.GlobalSaveDataFilePath);
			ES3.StoreCachedFile(SavesSystem.GlobalSaveDataFilePath);
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x000406E1 File Offset: 0x0003E8E1
		public static T LoadGlobal<T>(string key, T defaultValue = default(T))
		{
			if (!SavesSystem.globalCached)
			{
				SavesSystem.CacheFile(SavesSystem.GlobalSaveDataFilePath);
				SavesSystem.globalCached = true;
			}
			if (ES3.KeyExists(key, SavesSystem.GlobalSaveDataFilePath))
			{
				return ES3.Load<T>(key, SavesSystem.GlobalSaveDataFilePath);
			}
			return defaultValue;
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x00040715 File Offset: 0x0003E915
		public static void CollectSaveData()
		{
			Action onCollectSaveData = SavesSystem.OnCollectSaveData;
			if (onCollectSaveData == null)
			{
				return;
			}
			onCollectSaveData();
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00040726 File Offset: 0x0003E926
		public static bool IsOldGame()
		{
			return SavesSystem.Load<bool>("IsOldGame");
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x00040732 File Offset: 0x0003E932
		public static bool IsOldGame(int index)
		{
			return SavesSystem.Load<bool>("IsOldGame", index);
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0004073F File Offset: 0x0003E93F
		private static void SetAsOldGame()
		{
			SavesSystem.Save<bool>("IsOldGame", true);
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x0004074C File Offset: 0x0003E94C
		public static void DeleteCurrentSave()
		{
			ES3.CacheFile(SavesSystem.CurrentFilePath);
			ES3.DeleteFile(SavesSystem.CurrentFilePath);
			ES3.Save<bool>("Created", false, SavesSystem.CurrentFilePath);
			ES3.StoreCachedFile(SavesSystem.CurrentFilePath);
			Debug.Log(string.Format("已删除存档{0}", SavesSystem.CurrentSlot));
			Action onSaveDeleted = SavesSystem.OnSaveDeleted;
			if (onSaveDeleted == null)
			{
				return;
			}
			onSaveDeleted();
		}

		// Token: 0x04000D0F RID: 3343
		private static int? _currentSlot = null;

		// Token: 0x04000D10 RID: 3344
		private static bool saving;

		// Token: 0x04000D11 RID: 3345
		private static ES3Settings settings = ES3Settings.defaultSettings;

		// Token: 0x04000D12 RID: 3346
		private static bool cached;

		// Token: 0x04000D18 RID: 3352
		private const int BackupListCount = 10;

		// Token: 0x04000D19 RID: 3353
		private static DateTime _lastSavedTime = DateTime.MinValue;

		// Token: 0x04000D1A RID: 3354
		private static DateTime _lastIndexedBackupTime = DateTime.MinValue;

		// Token: 0x04000D1B RID: 3355
		private static bool globalCached;

		// Token: 0x04000D1C RID: 3356
		private static ES3Settings GlobalFileSetting = new ES3Settings(null, null)
		{
			location = ES3.Location.File
		};

		// Token: 0x02000506 RID: 1286
		public struct BackupInfo
		{
			// Token: 0x17000749 RID: 1865
			// (get) Token: 0x06002751 RID: 10065 RVA: 0x0008FA82 File Offset: 0x0008DC82
			public bool TimeValid
			{
				get
				{
					return this.time_raw > 0L;
				}
			}

			// Token: 0x1700074A RID: 1866
			// (get) Token: 0x06002752 RID: 10066 RVA: 0x0008FA8E File Offset: 0x0008DC8E
			public DateTime Time
			{
				get
				{
					return DateTime.FromBinary(this.time_raw);
				}
			}

			// Token: 0x04001DBA RID: 7610
			public int slot;

			// Token: 0x04001DBB RID: 7611
			public int index;

			// Token: 0x04001DBC RID: 7612
			public string path;

			// Token: 0x04001DBD RID: 7613
			public bool exists;

			// Token: 0x04001DBE RID: 7614
			public long time_raw;
		}
	}
}
