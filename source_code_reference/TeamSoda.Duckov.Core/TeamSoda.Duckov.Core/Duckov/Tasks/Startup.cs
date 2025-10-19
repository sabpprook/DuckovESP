using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Duckov.Utilities;
using Saves;
using UnityEngine;

namespace Duckov.Tasks
{
	// Token: 0x0200036C RID: 876
	public class Startup : MonoBehaviour
	{
		// Token: 0x06001E4F RID: 7759 RVA: 0x0006AC1A File Offset: 0x00068E1A
		private void Awake()
		{
			this.MoveOldSaves();
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x0006AC24 File Offset: 0x00068E24
		private void MoveOldSaves()
		{
			string fullPathToSavesFolder = SavesSystem.GetFullPathToSavesFolder();
			if (!Directory.Exists(fullPathToSavesFolder))
			{
				Directory.CreateDirectory(fullPathToSavesFolder);
			}
			for (int i = 1; i <= 3; i++)
			{
				string saveFileName = SavesSystem.GetSaveFileName(i);
				string text = Path.Combine(Application.persistentDataPath, saveFileName);
				string text2 = Path.Combine(fullPathToSavesFolder, saveFileName);
				if (File.Exists(text) && !File.Exists(text2))
				{
					Debug.Log("Transporting:\n" + text + "\n->\n" + text2);
					SavesSystem.UpgradeSaveFileAssemblyInfo(text);
					File.Move(text, text2);
				}
			}
			string text3 = "Options.ES3";
			string text4 = Path.Combine(Application.persistentDataPath, text3);
			string text5 = Path.Combine(fullPathToSavesFolder, text3);
			if (File.Exists(text4) && !File.Exists(text5))
			{
				Debug.Log("Transporting:\n" + text4 + "\n->\n" + text5);
				SavesSystem.UpgradeSaveFileAssemblyInfo(text4);
				File.Move(text4, text5);
			}
			string globalSaveDataFileName = SavesSystem.GlobalSaveDataFileName;
			string text6 = Path.Combine(Application.persistentDataPath, globalSaveDataFileName);
			string text7 = Path.Combine(fullPathToSavesFolder, globalSaveDataFileName);
			if (!File.Exists(text6))
			{
				text6 = Path.Combine(Application.persistentDataPath, "Global.csv");
			}
			if (File.Exists(text6) && !File.Exists(text7))
			{
				Debug.Log("Transporting:\n" + text6 + "\n->\n" + text7);
				SavesSystem.UpgradeSaveFileAssemblyInfo(text6);
				File.Move(text6, text7);
			}
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0006AD70 File Offset: 0x00068F70
		private void Start()
		{
			this.StartupFlow().Forget();
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x0006AD80 File Offset: 0x00068F80
		private async UniTask StartupFlow()
		{
			foreach (MonoBehaviour monoBehaviour in this.beforeSequence)
			{
				if (!(monoBehaviour == null))
				{
					ITaskBehaviour task = monoBehaviour as ITaskBehaviour;
					if (task != null)
					{
						task.Begin();
						while (task.IsPending())
						{
							await UniTask.Yield();
						}
						task = null;
					}
				}
			}
			List<MonoBehaviour>.Enumerator enumerator = default(List<MonoBehaviour>.Enumerator);
			while (!this.EvaluateWaitList())
			{
				await UniTask.Yield();
			}
			SceneLoader.StaticLoadSingle(GameplayDataSettings.SceneManagement.MainMenuScene);
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x0006ADC4 File Offset: 0x00068FC4
		private bool EvaluateWaitList()
		{
			foreach (MonoBehaviour monoBehaviour in this.waitForTasks)
			{
				if (!(monoBehaviour == null))
				{
					ITaskBehaviour taskBehaviour = monoBehaviour as ITaskBehaviour;
					if (taskBehaviour != null && !taskBehaviour.IsComplete())
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x040014B5 RID: 5301
		public List<MonoBehaviour> beforeSequence = new List<MonoBehaviour>();

		// Token: 0x040014B6 RID: 5302
		public List<MonoBehaviour> waitForTasks = new List<MonoBehaviour>();
	}
}
