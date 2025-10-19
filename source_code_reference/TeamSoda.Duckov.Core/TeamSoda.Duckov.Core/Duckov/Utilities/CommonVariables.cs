using System;
using Saves;
using UnityEngine;

namespace Duckov.Utilities
{
	// Token: 0x020003F3 RID: 1011
	public class CommonVariables : MonoBehaviour
	{
		// Token: 0x170006F4 RID: 1780
		// (get) Token: 0x0600247B RID: 9339 RVA: 0x0007E8BB File Offset: 0x0007CABB
		public CustomDataCollection Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x0600247C RID: 9340 RVA: 0x0007E8C4 File Offset: 0x0007CAC4
		private void Awake()
		{
			if (CommonVariables.instance == null)
			{
				CommonVariables.instance = this;
			}
			else
			{
				Debug.LogWarning("检测到多个Common Variables");
			}
			SavesSystem.OnCollectSaveData += this.OnCollectSaveData;
			SavesSystem.OnSetFile += this.OnSetSaveFile;
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x0007E912 File Offset: 0x0007CB12
		private void OnDestroy()
		{
			SavesSystem.OnCollectSaveData -= this.OnCollectSaveData;
			SavesSystem.OnSetFile -= this.OnSetSaveFile;
		}

		// Token: 0x0600247E RID: 9342 RVA: 0x0007E936 File Offset: 0x0007CB36
		private void OnSetSaveFile()
		{
			this.Load();
		}

		// Token: 0x0600247F RID: 9343 RVA: 0x0007E93E File Offset: 0x0007CB3E
		private void OnCollectSaveData()
		{
			this.Save();
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x0007E946 File Offset: 0x0007CB46
		private void Start()
		{
			this.Load();
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x0007E94E File Offset: 0x0007CB4E
		private void Save()
		{
			SavesSystem.Save<CustomDataCollection>("CommonVariables", "Data", this.data);
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x0007E965 File Offset: 0x0007CB65
		private void Load()
		{
			this.data = SavesSystem.Load<CustomDataCollection>("CommonVariables", "Data");
			if (this.data == null)
			{
				this.data = new CustomDataCollection();
			}
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x0007E98F File Offset: 0x0007CB8F
		public static void SetFloat(string key, float value)
		{
			if (CommonVariables.instance)
			{
				CommonVariables.instance.Data.SetFloat(key, value, true);
			}
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x0007E9AF File Offset: 0x0007CBAF
		public static void SetInt(string key, int value)
		{
			if (CommonVariables.instance)
			{
				CommonVariables.instance.Data.SetInt(key, value, true);
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x0007E9CF File Offset: 0x0007CBCF
		public static void SetBool(string key, bool value)
		{
			if (CommonVariables.instance)
			{
				CommonVariables.instance.Data.SetBool(key, value, true);
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x0007E9EF File Offset: 0x0007CBEF
		public static void SetString(string key, string value)
		{
			if (CommonVariables.instance)
			{
				CommonVariables.instance.Data.SetString(key, value, true);
			}
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x0007EA0F File Offset: 0x0007CC0F
		public static float GetFloat(string key, float defaultValue = 0f)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetFloat(key, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x0007EA30 File Offset: 0x0007CC30
		public static int GetInt(string key, int defaultValue = 0)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetInt(key, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x0007EA51 File Offset: 0x0007CC51
		public static bool GetBool(string key, bool defaultValue = false)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetBool(key, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x0007EA72 File Offset: 0x0007CC72
		public static string GetString(string key, string defaultValue = "")
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetString(key, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x0007EA93 File Offset: 0x0007CC93
		public static float GetFloat(int hash, float defaultValue = 0f)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetFloat(hash, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x0600248C RID: 9356 RVA: 0x0007EAB4 File Offset: 0x0007CCB4
		public static int GetInt(int hash, int defaultValue = 0)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetInt(hash, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x0600248D RID: 9357 RVA: 0x0007EAD5 File Offset: 0x0007CCD5
		public static bool GetBool(int hash, bool defaultValue = false)
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetBool(hash, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x0007EAF6 File Offset: 0x0007CCF6
		public static string GetString(int hash, string defaultValue = "")
		{
			if (CommonVariables.instance)
			{
				return CommonVariables.instance.Data.GetString(hash, defaultValue);
			}
			return defaultValue;
		}

		// Token: 0x040018DF RID: 6367
		private static CommonVariables instance;

		// Token: 0x040018E0 RID: 6368
		[SerializeField]
		private CustomDataCollection data;

		// Token: 0x040018E1 RID: 6369
		private const string saves_prefix = "CommonVariables";

		// Token: 0x040018E2 RID: 6370
		private const string saves_key = "Data";
	}
}
