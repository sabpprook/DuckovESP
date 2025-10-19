using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Duckov.Modding;
using FMOD;
using FMODUnity;
using HarmonyLib;
using UnityEngine;

namespace ItemLevelAndSearchSoundMod
{
	// Token: 0x02000006 RID: 6
	[NullableContext(1)]
	[Nullable(0)]
	public class ModBehaviour : ModBehaviour
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002094 File Offset: 0x00000294
		private void OnEnable()
		{
			Debug.Log("ItemLevelAndSearchSoundMod OnEnable");
			try
			{
				foreach (object obj in Enum.GetValues(typeof(ItemValueLevel)))
				{
					ItemValueLevel itemValueLevel = (ItemValueLevel)obj;
					string text = string.Format("ItemLevelAndSearchSoundMod/{0}.mp3", (int)itemValueLevel);
					bool flag = File.Exists(text);
					if (flag)
					{
						Sound sound;
						RESULT result = RuntimeManager.CoreSystem.createSound(text, 1, ref sound);
						bool flag2 = result > 0;
						if (flag2)
						{
							ModBehaviour.ErrorMessage = ModBehaviour.ErrorMessage + "FMOD failed to create sound: " + result.ToString() + "\n";
						}
						else
						{
							ModBehaviour.ItemValueLevelSound.Add(itemValueLevel, sound);
							Debug.Log("ItemLevelAndSearchSoundMod Load Custom Sound Success: " + itemValueLevel.ToString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				ModBehaviour.ErrorMessage = ModBehaviour.ErrorMessage + ex.ToString() + "\n";
			}
			ColorUtility.TryParseHtmlString("#FFFFFF00", out ModBehaviour.White);
			ColorUtility.TryParseHtmlString("#7cff7c40", out ModBehaviour.Green);
			ColorUtility.TryParseHtmlString("#7cd5ff40", out ModBehaviour.Blue);
			ColorUtility.TryParseHtmlString("#d0acff40", out ModBehaviour.Purple);
			ColorUtility.TryParseHtmlString("#ffe60096", out ModBehaviour.Orange);
			ColorUtility.TryParseHtmlString("#ff585896", out ModBehaviour.LightRed);
			ColorUtility.TryParseHtmlString("#bb000096", out ModBehaviour.Red);
			this.harmony = new Harmony("Spuddy.ItemLevelAndSearchSoundMod");
			this.harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000225C File Offset: 0x0000045C
		private void OnGUI()
		{
			bool flag = !string.IsNullOrEmpty(ModBehaviour.ErrorMessage);
			if (flag)
			{
				GUIStyle guistyle = new GUIStyle(GUI.skin.label);
				guistyle.normal.textColor = Color.red;
				GUI.Label(new Rect(10f, 10f, (float)(Screen.width - 10), (float)(Screen.height - 10)), "ItemLevelAndSearchSoundMod Error: \n" + ModBehaviour.ErrorMessage, guistyle);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000022D8 File Offset: 0x000004D8
		private void OnDisable()
		{
			this.harmony.UnpatchAll("Spuddy.ItemLevelAndSearchSoundMod");
			foreach (KeyValuePair<ItemValueLevel, Sound> keyValuePair in ModBehaviour.ItemValueLevelSound)
			{
				keyValuePair.Value.release();
			}
			ModBehaviour.ItemValueLevelSound.Clear();
		}

		// Token: 0x0400000B RID: 11
		private const string Id = "Spuddy.ItemLevelAndSearchSoundMod";

		// Token: 0x0400000C RID: 12
		public const string Low = "UI/click";

		// Token: 0x0400000D RID: 13
		public const string Medium = "UI/sceneloader_click";

		// Token: 0x0400000E RID: 14
		public const string High = "UI/game_start";

		// Token: 0x0400000F RID: 15
		public static Dictionary<ItemValueLevel, Sound> ItemValueLevelSound = new Dictionary<ItemValueLevel, Sound>();

		// Token: 0x04000010 RID: 16
		public static string ErrorMessage = "";

		// Token: 0x04000011 RID: 17
		public static Color White;

		// Token: 0x04000012 RID: 18
		public static Color Green;

		// Token: 0x04000013 RID: 19
		public static Color Blue;

		// Token: 0x04000014 RID: 20
		public static Color Purple;

		// Token: 0x04000015 RID: 21
		public static Color Orange;

		// Token: 0x04000016 RID: 22
		public static Color LightRed;

		// Token: 0x04000017 RID: 23
		public static Color Red;

		// Token: 0x04000018 RID: 24
		private Harmony harmony;
	}
}
