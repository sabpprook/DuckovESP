using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class BoundaryGenerator : MonoBehaviour
{
	// Token: 0x06000A12 RID: 2578 RVA: 0x0002B188 File Offset: 0x00029388
	public void UpdateColliderParameters()
	{
		if (this.colliderObjects == null || this.colliderObjects.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this.colliderObjects.Count; i++)
		{
			if (i < this.points.Count - 1)
			{
				BoxCollider boxCollider = this.colliderObjects[i];
				if (!(boxCollider == null))
				{
					boxCollider.gameObject.layer = base.gameObject.layer;
					Vector3 vector = base.transform.TransformPoint(this.points[i]);
					Vector3 vector2 = base.transform.TransformPoint(this.points[i + 1]);
					float num = Mathf.Min(vector.y, vector2.y);
					vector.y = num;
					vector2.y = num;
					Vector3 normalized = (vector2 - vector).normalized;
					float num2 = Vector3.Distance(vector, vector2);
					boxCollider.size = new Vector3(this.colliderThickness, this.colliderHeight, num2);
					boxCollider.transform.forward = normalized;
					boxCollider.transform.position = (vector + vector2) / 2f + Vector3.up * 0.5f * this.colliderHeight + Vector3.up * this.yOffset + boxCollider.transform.right * 0.5f * this.colliderThickness * (this.inverseFaceDirection ? (-1f) : 1f);
					if (this.provideContects)
					{
						boxCollider.providesContacts = true;
					}
				}
			}
		}
	}

	// Token: 0x06000A13 RID: 2579 RVA: 0x0002B340 File Offset: 0x00029540
	private void DestroyAllChildren()
	{
		while (base.transform.childCount > 0)
		{
			Transform child = base.transform.GetChild(0);
			child.SetParent(null);
			if (Application.isPlaying)
			{
				global::UnityEngine.Object.Destroy(child.gameObject);
			}
			else
			{
				global::UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0002B390 File Offset: 0x00029590
	public void UpdateColliders()
	{
		this.DestroyAllChildren();
		if (this.colliderObjects == null)
		{
			this.colliderObjects = new List<BoxCollider>();
		}
		this.colliderObjects.Clear();
		for (int i = 0; i < this.points.Count - 1; i++)
		{
			BoxCollider boxCollider = new GameObject(string.Format("Collider_{0}", i))
			{
				transform = 
				{
					parent = base.transform
				}
			}.AddComponent<BoxCollider>();
			this.colliderObjects.Add(boxCollider);
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x0002B414 File Offset: 0x00029614
	public void SetYtoZero()
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			this.points[i] = new Vector3(this.points[i].x, 0f, this.points[i].z);
		}
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x0002B46F File Offset: 0x0002966F
	public void OnPointsUpdated(bool OnValidate = false)
	{
		if (this.points == null)
		{
			this.points = new List<Vector3>();
		}
		if (base.transform.childCount != this.points.Count - 1 && !OnValidate)
		{
			this.UpdateColliders();
		}
		this.UpdateColliderParameters();
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x0002B4AD File Offset: 0x000296AD
	public void RemoveAllPoints()
	{
		this.points.Clear();
		this.OnPointsUpdated(false);
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x0002B4C1 File Offset: 0x000296C1
	public void RespawnColliders()
	{
		this.DestroyAllChildren();
		this.colliderObjects.Clear();
		this.OnPointsUpdated(false);
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x0002B4DB File Offset: 0x000296DB
	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this.OnPointsUpdated(true);
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0002B4EC File Offset: 0x000296EC
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if (this.colliderObjects == null)
		{
			return;
		}
		if (this.colliderObjects.Count > 0)
		{
			foreach (Vector3 vector in this.points)
			{
				Gizmos.DrawCube(base.transform.TransformPoint(vector), Vector3.one * 0.15f);
			}
		}
	}

	// Token: 0x040008CE RID: 2254
	public List<Vector3> points;

	// Token: 0x040008CF RID: 2255
	[HideInInspector]
	public int lastSelectedPointIndex = -1;

	// Token: 0x040008D0 RID: 2256
	public float colliderHeight = 1f;

	// Token: 0x040008D1 RID: 2257
	public float yOffset;

	// Token: 0x040008D2 RID: 2258
	public float colliderThickness = 0.1f;

	// Token: 0x040008D3 RID: 2259
	public bool inverseFaceDirection;

	// Token: 0x040008D4 RID: 2260
	public bool provideContects;

	// Token: 0x040008D5 RID: 2261
	[SerializeField]
	[HideInInspector]
	private List<BoxCollider> colliderObjects;
}
