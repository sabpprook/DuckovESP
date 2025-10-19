using System;
using System.Collections.Generic;
using Duckov.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020000AF RID: 175
[RequireComponent(typeof(Points))]
public class SimpleTeleporterSpawner : MonoBehaviour
{
	// Token: 0x060005C5 RID: 1477 RVA: 0x00019C50 File Offset: 0x00017E50
	private void Start()
	{
		if (this.points == null)
		{
			this.points = base.GetComponent<Points>();
			if (this.points == null)
			{
				return;
			}
		}
		this.scene = SceneManager.GetActiveScene().buildIndex;
		if (LevelManager.LevelInited)
		{
			this.StartCreate();
			return;
		}
		LevelManager.OnLevelInitialized += this.StartCreate;
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00019CB8 File Offset: 0x00017EB8
	private void OnValidate()
	{
		if (this.points == null)
		{
			this.points = base.GetComponent<Points>();
		}
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00019CD4 File Offset: 0x00017ED4
	private void OnDestroy()
	{
		LevelManager.OnLevelInitialized -= this.StartCreate;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00019CE8 File Offset: 0x00017EE8
	public void StartCreate()
	{
		this.scene = SceneManager.GetActiveScene().buildIndex;
		int key = this.GetKey();
		object obj;
		if (!MultiSceneCore.Instance.inLevelData.TryGetValue(key, out obj))
		{
			MultiSceneCore.Instance.inLevelData.Add(key, true);
			this.Create();
			return;
		}
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00019D40 File Offset: 0x00017F40
	private void Create()
	{
		List<Vector3> randomPoints = this.points.GetRandomPoints(this.pairCount * 2);
		for (int i = 0; i < this.pairCount; i++)
		{
			this.CreateAPair(randomPoints[i * 2], randomPoints[i * 2 + 1]);
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00019D8C File Offset: 0x00017F8C
	private void CreateAPair(Vector3 point1, Vector3 point2)
	{
		SimpleTeleporter simpleTeleporter = this.CreateATeleporter(point1);
		SimpleTeleporter simpleTeleporter2 = this.CreateATeleporter(point2);
		simpleTeleporter.target = simpleTeleporter2.TeleportPoint;
		simpleTeleporter2.target = simpleTeleporter.TeleportPoint;
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00019DC1 File Offset: 0x00017FC1
	private SimpleTeleporter CreateATeleporter(Vector3 point)
	{
		SimpleTeleporter simpleTeleporter = global::UnityEngine.Object.Instantiate<SimpleTeleporter>(this.simpleTeleporterPfb);
		MultiSceneCore.MoveToActiveWithScene(simpleTeleporter.gameObject, this.scene);
		simpleTeleporter.transform.position = point;
		return simpleTeleporter;
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00019DEC File Offset: 0x00017FEC
	private int GetKey()
	{
		Vector3 vector = base.transform.position * 10f;
		int num = Mathf.RoundToInt(vector.x);
		int num2 = Mathf.RoundToInt(vector.y);
		int num3 = Mathf.RoundToInt(vector.z);
		Vector3Int vector3Int = new Vector3Int(num, num2, num3);
		return string.Format("SimpTeles_{0}", vector3Int).GetHashCode();
	}

	// Token: 0x0400054A RID: 1354
	private int scene = -1;

	// Token: 0x0400054B RID: 1355
	[SerializeField]
	private int pairCount = 3;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	private SimpleTeleporter simpleTeleporterPfb;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private Points points;
}
