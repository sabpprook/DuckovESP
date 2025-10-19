using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014D RID: 333
[RequireComponent(typeof(Points))]
[ExecuteInEditMode]
public class PrefabLineGenrator : MonoBehaviour, IOnPointsChanged
{
	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06000A5B RID: 2651 RVA: 0x0002D9B3 File Offset: 0x0002BBB3
	private List<Vector3> originPoints
	{
		get
		{
			return this.points.points;
		}
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0002D9C0 File Offset: 0x0002BBC0
	public void OnPointsChanged()
	{
	}

	// Token: 0x0400090C RID: 2316
	[SerializeField]
	private float prefabLength = 2f;

	// Token: 0x0400090D RID: 2317
	public PrefabLineGenrator.SapwnInfo spawnPrefab;

	// Token: 0x0400090E RID: 2318
	public PrefabLineGenrator.SapwnInfo startPrefab;

	// Token: 0x0400090F RID: 2319
	public PrefabLineGenrator.SapwnInfo endPrefab;

	// Token: 0x04000910 RID: 2320
	[SerializeField]
	private Points points;

	// Token: 0x04000911 RID: 2321
	[SerializeField]
	[HideInInspector]
	private List<BoxCollider> colliderObjects;

	// Token: 0x04000912 RID: 2322
	[SerializeField]
	private float updateTick = 0.5f;

	// Token: 0x04000913 RID: 2323
	private float lastModifyTime;

	// Token: 0x04000914 RID: 2324
	private bool dirty;

	// Token: 0x04000915 RID: 2325
	public List<Vector3> searchedPointsLocalSpace;

	// Token: 0x020004A3 RID: 1187
	[Serializable]
	public struct SapwnInfo
	{
		// Token: 0x0600269A RID: 9882 RVA: 0x0008ACFC File Offset: 0x00088EFC
		public GameObject GetRandomPrefab()
		{
			if (this.prefabs.Count < 1)
			{
				return null;
			}
			float num = 0f;
			for (int i = 0; i < this.prefabs.Count; i++)
			{
				num += this.prefabs[i].weight;
			}
			float num2 = global::UnityEngine.Random.Range(0f, num);
			for (int j = 0; j < this.prefabs.Count; j++)
			{
				if (num2 <= this.prefabs[j].weight)
				{
					return this.prefabs[j].prefab;
				}
				num2 -= this.prefabs[j].weight;
			}
			return this.prefabs[this.prefabs.Count - 1].prefab;
		}

		// Token: 0x04001C21 RID: 7201
		public List<PrefabLineGenrator.PrefabPair> prefabs;

		// Token: 0x04001C22 RID: 7202
		public float rotateOffset;

		// Token: 0x04001C23 RID: 7203
		[Range(0f, 1f)]
		public float flatten;

		// Token: 0x04001C24 RID: 7204
		public Vector3 posOffset;
	}

	// Token: 0x020004A4 RID: 1188
	[Serializable]
	public struct PrefabPair
	{
		// Token: 0x04001C25 RID: 7205
		public GameObject prefab;

		// Token: 0x04001C26 RID: 7206
		public float weight;
	}
}
