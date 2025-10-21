using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duckov.MiniGames.BubblePoppers
{
	// Token: 0x020002D8 RID: 728
	public class BubblePopperLayout : MiniGameBehaviour
	{
		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06001736 RID: 5942 RVA: 0x000553AE File Offset: 0x000535AE
		public Vector2 XPositionBorder
		{
			get
			{
				return new Vector2((float)this.xBorder.x * this.BubbleRadius * 2f - this.BubbleRadius, (float)this.xBorder.y * this.BubbleRadius * 2f);
			}
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x000553F0 File Offset: 0x000535F0
		public Vector2 CoordToLocalPosition(Vector2Int coord)
		{
			float bubbleRadius = this.BubbleRadius;
			return new Vector2(((coord.y % 2 != 0) ? bubbleRadius : 0f) + (float)coord.x * bubbleRadius * 2f, (float)coord.y * bubbleRadius * BubblePopperLayout.YOffsetFactor);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x00055440 File Offset: 0x00053640
		public Vector2Int LocalPositionToCoord(Vector2 localPosition)
		{
			float bubbleRadius = this.BubbleRadius;
			int num = Mathf.RoundToInt(localPosition.y / bubbleRadius / BubblePopperLayout.YOffsetFactor);
			float num2 = ((num % 2 != 0) ? bubbleRadius : 0f);
			return new Vector2Int(Mathf.RoundToInt((localPosition.x - num2) / bubbleRadius / 2f), num);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x00055494 File Offset: 0x00053694
		public Vector2Int WorldPositionToCoord(Vector2 position)
		{
			Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(position);
			return this.LocalPositionToCoord(vector);
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000554C8 File Offset: 0x000536C8
		public Vector2Int[] GetAllNeighbourCoords(Vector2Int center, bool includeCenter)
		{
			int num = ((center.y % 2 != 0) ? 0 : (-1));
			Vector2Int[] array;
			if (includeCenter)
			{
				array = new Vector2Int[]
				{
					new Vector2Int(center.x + num, center.y + 1),
					new Vector2Int(center.x + num + 1, center.y + 1),
					new Vector2Int(center.x - 1, center.y),
					center,
					new Vector2Int(center.x + 1, center.y),
					new Vector2Int(center.x + num, center.y - 1),
					new Vector2Int(center.x + num + 1, center.y - 1)
				};
			}
			else
			{
				array = new Vector2Int[]
				{
					new Vector2Int(center.x + num, center.y + 1),
					new Vector2Int(center.x + num + 1, center.y + 1),
					new Vector2Int(center.x - 1, center.y),
					new Vector2Int(center.x + 1, center.y),
					new Vector2Int(center.x + num, center.y - 1),
					new Vector2Int(center.x + num + 1, center.y - 1)
				};
			}
			return array;
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00055674 File Offset: 0x00053874
		public List<Vector2Int> GetAllPassingCoords(Vector2 localOrigin, Vector2 direction, float length)
		{
			float num = this.BubbleRadius * 2f;
			List<Vector2Int> list = new List<Vector2Int> { this.LocalPositionToCoord(localOrigin) };
			if (num > 0f)
			{
				float num2 = -num;
				while (num2 < length)
				{
					num2 += num;
					Vector2 vector = localOrigin + num2 * direction;
					Vector2Int vector2Int = this.LocalPositionToCoord(vector);
					list.AddRange(this.GetAllNeighbourCoords(vector2Int, true));
				}
			}
			return list;
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x000556E0 File Offset: 0x000538E0
		private void OnDrawGizmos()
		{
			float bubbleRadius = this.BubbleRadius;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(new Vector3(this.XPositionBorder.x, 0f), new Vector3(this.XPositionBorder.x, -100f));
			Gizmos.DrawLine(new Vector3(this.XPositionBorder.y, 0f), new Vector3(this.XPositionBorder.y, -100f));
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0005576C File Offset: 0x0005396C
		public void GizmosDrawCoord(Vector2Int coord, float ratio)
		{
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawSphere(this.CoordToLocalPosition(coord), this.BubbleRadius * ratio);
			Gizmos.matrix = matrix;
		}

		// Token: 0x04001103 RID: 4355
		[SerializeField]
		private Vector2Int xBorder;

		// Token: 0x04001104 RID: 4356
		public Vector2Int XCoordBorder;

		// Token: 0x04001105 RID: 4357
		public float BubbleRadius = 8f;

		// Token: 0x04001106 RID: 4358
		public static readonly float YOffsetFactor = Mathf.Tan(1.0471976f);

		// Token: 0x04001107 RID: 4359
		[SerializeField]
		private Transform tester;

		// Token: 0x04001108 RID: 4360
		[SerializeField]
		private float distance = 10f;

		// Token: 0x04001109 RID: 4361
		[SerializeField]
		private Vector2Int min;

		// Token: 0x0400110A RID: 4362
		[SerializeField]
		private Vector2Int max;
	}
}
