using System;
using Saves;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class BaseSceneUtilities : MonoBehaviour
{
	// Token: 0x060009DC RID: 2524 RVA: 0x0002A6BF File Offset: 0x000288BF
	private void Save()
	{
		LevelManager.Instance.SaveMainCharacter();
		SavesSystem.CollectSaveData();
		SavesSystem.SaveFile(true);
		this.lastTimeSaved = Time.realtimeSinceStartup;
	}

	// Token: 0x17000207 RID: 519
	// (get) Token: 0x060009DD RID: 2525 RVA: 0x0002A6E1 File Offset: 0x000288E1
	private float TimeSinceLastSave
	{
		get
		{
			return Time.realtimeSinceStartup - this.lastTimeSaved;
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0002A6EF File Offset: 0x000288EF
	private void Awake()
	{
		this.lastTimeSaved = Time.realtimeSinceStartup;
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x0002A6FC File Offset: 0x000288FC
	private void Update()
	{
		if (!LevelManager.LevelInited)
		{
			return;
		}
		if (this.TimeSinceLastSave > this.saveInterval)
		{
			this.Save();
		}
	}

	// Token: 0x040008B1 RID: 2225
	[SerializeField]
	private float saveInterval = 5f;

	// Token: 0x040008B2 RID: 2226
	private float lastTimeSaved = float.MinValue;
}
