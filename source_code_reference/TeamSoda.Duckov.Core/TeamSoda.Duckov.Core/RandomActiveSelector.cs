using System;
using System.Collections.Generic;
using Duckov.Scenes;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class RandomActiveSelector : MonoBehaviour
{
	// Token: 0x060005B0 RID: 1456 RVA: 0x00019754 File Offset: 0x00017954
	private void Awake()
	{
		foreach (GameObject gameObject in this.selections)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x000197B0 File Offset: 0x000179B0
	private void Update()
	{
		if (!this.setted && LevelManager.LevelInited)
		{
			this.Set();
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x000197C8 File Offset: 0x000179C8
	private void Set()
	{
		if (MultiSceneCore.Instance == null)
		{
			return;
		}
		object obj;
		if (MultiSceneCore.Instance.inLevelData.TryGetValue(this.guid, out obj))
		{
			this.activeIndex = (int)obj;
		}
		else
		{
			if (global::UnityEngine.Random.Range(0f, 1f) > this.activeChance)
			{
				this.activeIndex = -1;
			}
			else
			{
				this.activeIndex = global::UnityEngine.Random.Range(0, this.selections.Count);
			}
			MultiSceneCore.Instance.inLevelData.Add(this.guid, this.activeIndex);
		}
		if (this.activeIndex >= 0)
		{
			GameObject gameObject = this.selections[this.activeIndex];
			if (gameObject)
			{
				gameObject.SetActive(true);
			}
		}
		this.setted = true;
		base.enabled = false;
	}

	// Token: 0x04000531 RID: 1329
	[Range(0f, 1f)]
	public float activeChance = 1f;

	// Token: 0x04000532 RID: 1330
	private int activeIndex;

	// Token: 0x04000533 RID: 1331
	private int guid;

	// Token: 0x04000534 RID: 1332
	private bool setted;

	// Token: 0x04000535 RID: 1333
	public List<GameObject> selections;
}
