using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014B RID: 331
public class Points : MonoBehaviour
{
	// Token: 0x06000A52 RID: 2642 RVA: 0x0002D83C File Offset: 0x0002BA3C
	public void SetYtoZero()
	{
		for (int i = 0; i < this.points.Count; i++)
		{
			this.points[i] = new Vector3(this.points[i].x, 0f, this.points[i].z);
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x0002D897 File Offset: 0x0002BA97
	public void RemoveAllPoints()
	{
		this.points = new List<Vector3>();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x0002D8A4 File Offset: 0x0002BAA4
	public List<Vector3> GetRandomPoints(int count)
	{
		List<Vector3> list = new List<Vector3>();
		list.AddRange(this.points);
		List<Vector3> list2 = new List<Vector3>();
		while (list2.Count < count && list.Count > 0)
		{
			int num = global::UnityEngine.Random.Range(0, list.Count);
			Vector3 vector = this.PointToWorld(list[num]);
			list2.Add(vector);
			list.RemoveAt(num);
		}
		return list2;
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x0002D908 File Offset: 0x0002BB08
	public Vector3 GetRandomPoint()
	{
		int num = global::UnityEngine.Random.Range(0, this.points.Count);
		return this.GetPoint(num);
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x0002D930 File Offset: 0x0002BB30
	public Vector3 GetPoint(int index)
	{
		if (index >= this.points.Count)
		{
			return Vector3.zero;
		}
		Vector3 vector = this.points[index];
		return this.PointToWorld(vector);
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x0002D965 File Offset: 0x0002BB65
	private Vector3 PointToWorld(Vector3 point)
	{
		if (!this.worldSpace)
		{
			point = base.transform.TransformPoint(point);
		}
		return point;
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x0002D980 File Offset: 0x0002BB80
	public void SendPointsChangedMessage()
	{
		IOnPointsChanged component = base.GetComponent<IOnPointsChanged>();
		if (component != null)
		{
			component.OnPointsChanged();
		}
	}

	// Token: 0x04000908 RID: 2312
	public List<Vector3> points;

	// Token: 0x04000909 RID: 2313
	[HideInInspector]
	public int lastSelectedPointIndex = -1;

	// Token: 0x0400090A RID: 2314
	public bool worldSpace = true;

	// Token: 0x0400090B RID: 2315
	public bool syncToSelectedPoint;
}
