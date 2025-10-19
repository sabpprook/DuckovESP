using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Saves;
using Sirenix.Utilities;
using UnityEngine;

namespace Duckov.Modding
{
	// Token: 0x02000268 RID: 616
	public class ModManager : MonoBehaviour
	{
		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001320 RID: 4896 RVA: 0x00047276 File Offset: 0x00045476
		// (set) Token: 0x06001321 RID: 4897 RVA: 0x00047283 File Offset: 0x00045483
		public static bool AllowActivatingMod
		{
			get
			{
				return SavesSystem.LoadGlobal<bool>("AllowLoadingMod", false);
			}
			set
			{
				SavesSystem.SaveGlobal<bool>("AllowLoadingMod", value);
				if (ModManager.Instance != null)
				{
					ModManager.Instance.ScanAndActivateMods();
				}
			}
		}

		// Token: 0x06001322 RID: 4898 RVA: 0x000472A7 File Offset: 0x000454A7
		private void Awake()
		{
			if (this.modParent == null)
			{
				this.modParent = base.transform;
			}
		}

		// Token: 0x06001323 RID: 4899 RVA: 0x000472C3 File Offset: 0x000454C3
		private void Start()
		{
			this.ScanAndActivateMods();
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x000472CC File Offset: 0x000454CC
		public void ScanAndActivateMods()
		{
			if (!ModManager.AllowActivatingMod)
			{
				return;
			}
			ModManager.Rescan();
			foreach (ModInfo modInfo in ModManager.modInfos)
			{
				if (!this.activeMods.ContainsKey(modInfo.name))
				{
					bool flag = this.ShouldActivateMod(modInfo);
					Debug.Log(string.Format("ModActive_{0}: {1}", modInfo.name, flag));
					if (flag && this.ActivateMod(modInfo) == null)
					{
						this.SetShouldActivateMod(modInfo, false);
					}
				}
			}
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x00047374 File Offset: 0x00045574
		private void SetShouldActivateMod(ModInfo info, bool value)
		{
			SavesSystem.SaveGlobal<bool>("ModActive_" + info.name, value);
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0004738C File Offset: 0x0004558C
		private bool ShouldActivateMod(ModInfo info)
		{
			return SavesSystem.LoadGlobal<bool>("ModActive_" + info.name, false);
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001327 RID: 4903 RVA: 0x000473A4 File Offset: 0x000455A4
		private static string DefaultModFolderPath
		{
			get
			{
				return Path.Combine(Application.dataPath, "Mods");
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001328 RID: 4904 RVA: 0x000473B5 File Offset: 0x000455B5
		public static ModManager Instance
		{
			get
			{
				return GameManager.ModManager;
			}
		}

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06001329 RID: 4905 RVA: 0x000473BC File Offset: 0x000455BC
		// (remove) Token: 0x0600132A RID: 4906 RVA: 0x000473F0 File Offset: 0x000455F0
		public static event Action<List<ModInfo>> OnScan;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x0600132B RID: 4907 RVA: 0x00047424 File Offset: 0x00045624
		// (remove) Token: 0x0600132C RID: 4908 RVA: 0x00047458 File Offset: 0x00045658
		public static event Action<ModInfo, ModBehaviour> OnModActivated;

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x0600132D RID: 4909 RVA: 0x0004748C File Offset: 0x0004568C
		// (remove) Token: 0x0600132E RID: 4910 RVA: 0x000474C0 File Offset: 0x000456C0
		public static event Action<ModInfo, ModBehaviour> OnModWillBeDeactivated;

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x0600132F RID: 4911 RVA: 0x000474F4 File Offset: 0x000456F4
		// (remove) Token: 0x06001330 RID: 4912 RVA: 0x00047528 File Offset: 0x00045728
		public static event Action OnModStatusChanged;

		// Token: 0x06001331 RID: 4913 RVA: 0x0004755C File Offset: 0x0004575C
		public static void Rescan()
		{
			ModManager.modInfos.Clear();
			if (Directory.Exists(ModManager.DefaultModFolderPath))
			{
				string[] directories = Directory.GetDirectories(ModManager.DefaultModFolderPath);
				for (int i = 0; i < directories.Length; i++)
				{
					ModInfo modInfo;
					if (ModManager.TryProcessModFolder(directories[i], out modInfo, false, 0UL))
					{
						ModManager.modInfos.Add(modInfo);
					}
				}
			}
			Action<List<ModInfo>> onScan = ModManager.OnScan;
			if (onScan == null)
			{
				return;
			}
			onScan(ModManager.modInfos);
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x000475C8 File Offset: 0x000457C8
		public static bool TryProcessModFolder(string path, out ModInfo info, bool isSteamItem = false, ulong publishedFileId = 0UL)
		{
			info = default(ModInfo);
			info.path = path;
			string text = Path.Combine(path, "info.ini");
			if (!File.Exists(text))
			{
				return false;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			using (StreamReader streamReader = File.OpenText(text))
			{
				while (!streamReader.EndOfStream)
				{
					string text2 = streamReader.ReadLine().Trim();
					if (!string.IsNullOrWhiteSpace(text2) && !text2.StartsWith('['))
					{
						string[] array = text2.Split('=', StringSplitOptions.None);
						if (array.Length == 2)
						{
							string text3 = array[0].Trim();
							string text4 = array[1].Trim();
							dictionary[text3] = text4;
						}
					}
				}
			}
			string text5;
			if (!dictionary.TryGetValue("name", out text5))
			{
				Debug.LogError("Failed to get name value in mod info.ini file. Aborting.\n" + path);
				return false;
			}
			string text6;
			if (!dictionary.TryGetValue("displayName", out text6))
			{
				text6 = text5;
				Debug.LogError("Failed to get displayName value in mod info.ini file.\n" + path);
			}
			string text7;
			if (!dictionary.TryGetValue("description", out text7))
			{
				text7 = "?";
				Debug.LogError("Failed to get description value in mod info.ini file.\n" + path);
			}
			ulong num = 0UL;
			string text8;
			if (dictionary.TryGetValue("publishedFileId", out text8) && !ulong.TryParse(text8, out num))
			{
				Debug.LogError("Invalid publishedFileId");
			}
			if (!isSteamItem)
			{
				publishedFileId = num;
			}
			else if (publishedFileId != num)
			{
				Debug.LogError("PublishFileId not match.\npath:" + path);
			}
			info.name = text5;
			info.displayName = text6;
			info.description = text7;
			info.publishedFileId = publishedFileId;
			info.isSteamItem = isSteamItem;
			string dllPath = info.dllPath;
			info.dllFound = File.Exists(dllPath);
			if (!info.dllFound)
			{
				Debug.LogError("Dll for mod " + text5 + " not found.\nExpecting: " + dllPath);
			}
			string text9 = Path.Combine(path, "preview.png");
			if (File.Exists(text9))
			{
				using (FileStream fileStream = File.OpenRead(text9))
				{
					Texture2D texture2D = new Texture2D(256, 256);
					byte[] array2 = new byte[fileStream.Length];
					fileStream.Read(array2);
					if (texture2D.LoadImage(array2))
					{
						info.preview = texture2D;
					}
				}
			}
			return true;
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x00047808 File Offset: 0x00045A08
		public static bool IsModActive(ModInfo info, out ModBehaviour instance)
		{
			instance = null;
			return !(ModManager.Instance == null) && ModManager.Instance.activeMods.TryGetValue(info.name, out instance) && instance != null;
		}

		// Token: 0x06001334 RID: 4916 RVA: 0x00047840 File Offset: 0x00045A40
		public ModBehaviour GetActiveModBehaviour(ModInfo info)
		{
			ModBehaviour modBehaviour;
			if (this.activeMods.TryGetValue(info.name, out modBehaviour))
			{
				return modBehaviour;
			}
			return null;
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x00047868 File Offset: 0x00045A68
		public void DeactivateMod(ModInfo info)
		{
			ModBehaviour activeModBehaviour = this.GetActiveModBehaviour(info);
			if (activeModBehaviour == null)
			{
				return;
			}
			try
			{
				activeModBehaviour.NotifyBeforeDeactivate();
				Action<ModInfo, ModBehaviour> onModWillBeDeactivated = ModManager.OnModWillBeDeactivated;
				if (onModWillBeDeactivated != null)
				{
					onModWillBeDeactivated(info, activeModBehaviour);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			this.activeMods.Remove(info.name);
			try
			{
				global::UnityEngine.Object.Destroy(activeModBehaviour.gameObject);
				Action onModStatusChanged = ModManager.OnModStatusChanged;
				if (onModStatusChanged != null)
				{
					onModStatusChanged();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogException(ex2);
			}
			this.SetShouldActivateMod(info, false);
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x00047904 File Offset: 0x00045B04
		public ModBehaviour ActivateMod(ModInfo info)
		{
			if (!ModManager.AllowActivatingMod)
			{
				Debug.LogError("Activating mod not allowed! \nUser must first interact with the agreement UI in order to allow activating mods.");
				return null;
			}
			string dllPath = info.dllPath;
			string name = info.name;
			ModBehaviour modBehaviour;
			if (ModManager.IsModActive(info, out modBehaviour))
			{
				Debug.LogError("Mod " + info.name + " instance already exists! Abort. Path: " + info.path, modBehaviour);
				return null;
			}
			Debug.Log("Loading mod dll at path: " + dllPath);
			Type type;
			try
			{
				type = Assembly.LoadFrom(dllPath).GetType(name + ".ModBehaviour");
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				string text = "Mod loading failed: " + name + "\n" + ex.Message;
				Action<string, string> onModLoadingFailed = ModManager.OnModLoadingFailed;
				if (onModLoadingFailed != null)
				{
					onModLoadingFailed(info.dllPath, text);
				}
				return null;
			}
			if (type == null || !type.InheritsFrom<ModBehaviour>())
			{
				Debug.LogError("Cannot load mod.\nA type named " + name + ".Mod is expected, and it should inherit from Duckov.Modding.Mod.");
				return null;
			}
			GameObject gameObject = new GameObject(name);
			ModBehaviour modBehaviour2;
			try
			{
				modBehaviour2 = gameObject.AddComponent(type) as ModBehaviour;
			}
			catch (Exception ex2)
			{
				Debug.LogException(ex2);
				Debug.LogError("Failed to create component for mod " + name);
				return null;
			}
			if (modBehaviour2 == null)
			{
				global::UnityEngine.Object.Destroy(gameObject);
				Debug.LogError("Failed to create component for mod " + name);
				return null;
			}
			gameObject.transform.SetParent(base.transform);
			Debug.Log("Mod Loaded: " + info.name);
			modBehaviour2.Setup(this, info);
			this.activeMods[info.name] = modBehaviour2;
			try
			{
				Action<ModInfo, ModBehaviour> onModActivated = ModManager.OnModActivated;
				if (onModActivated != null)
				{
					onModActivated(info, modBehaviour2);
				}
				Action onModStatusChanged = ModManager.OnModStatusChanged;
				if (onModStatusChanged != null)
				{
					onModStatusChanged();
				}
			}
			catch (Exception ex3)
			{
				Debug.LogException(ex3);
			}
			this.SetShouldActivateMod(info, true);
			return modBehaviour2;
		}

		// Token: 0x06001337 RID: 4919 RVA: 0x00047AF4 File Offset: 0x00045CF4
		internal static void WriteModInfoINI(ModInfo modInfo)
		{
			string text = Path.Combine(modInfo.path, "info.ini");
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			using (FileStream fileStream = File.Create(text))
			{
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.WriteLine("name = " + modInfo.name);
				streamWriter.WriteLine("displayName = " + modInfo.displayName);
				streamWriter.WriteLine("description = " + modInfo.description);
				streamWriter.WriteLine("");
				streamWriter.WriteLine(string.Format("publishedFileId = {0}", modInfo.publishedFileId));
				streamWriter.Close();
			}
		}

		// Token: 0x04000E44 RID: 3652
		[SerializeField]
		private Transform modParent;

		// Token: 0x04000E45 RID: 3653
		public static Action<string, string> OnModLoadingFailed;

		// Token: 0x04000E46 RID: 3654
		public static List<ModInfo> modInfos = new List<ModInfo>();

		// Token: 0x04000E47 RID: 3655
		private Dictionary<string, ModBehaviour> activeMods = new Dictionary<string, ModBehaviour>();
	}
}
