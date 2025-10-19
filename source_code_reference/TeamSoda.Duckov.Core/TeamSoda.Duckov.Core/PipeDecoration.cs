using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000141 RID: 321
[RequireComponent(typeof(PipeRenderer))]
public class PipeDecoration : MonoBehaviour
{
	// Token: 0x06000A29 RID: 2601 RVA: 0x0002B7CC File Offset: 0x000299CC
	private void OnDrawGizmosSelected()
	{
		if (this.pipeRenderer == null)
		{
			this.pipeRenderer = base.GetComponent<PipeRenderer>();
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0002B7E8 File Offset: 0x000299E8
	private void Refresh()
	{
		if (this.pipeRenderer.splineInUse == null || this.pipeRenderer.splineInUse.Length < 1)
		{
			return;
		}
		for (int i = 0; i < this.decorations.Count; i++)
		{
			PipeDecoration.GameObjectOffset gameObjectOffset = this.decorations[i];
			Quaternion quaternion;
			Vector3 positionByOffset = this.pipeRenderer.GetPositionByOffset(gameObjectOffset.offset, out quaternion);
			Vector3 vector = this.pipeRenderer.transform.localToWorldMatrix.MultiplyPoint(positionByOffset);
			if (!(gameObjectOffset.gameObject == null))
			{
				gameObjectOffset.gameObject.transform.position = vector;
				gameObjectOffset.gameObject.transform.localRotation = quaternion;
				gameObjectOffset.gameObject.transform.Rotate(this.rotate);
				gameObjectOffset.gameObject.transform.localScale = this.scale * this.uniformScale;
			}
		}
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0002B8D4 File Offset: 0x00029AD4
	public void OnValidate()
	{
		if (this.pipeRenderer == null)
		{
			this.pipeRenderer = base.GetComponent<PipeRenderer>();
		}
		this.Refresh();
	}

	// Token: 0x040008DB RID: 2267
	public PipeRenderer pipeRenderer;

	// Token: 0x040008DC RID: 2268
	[HideInInspector]
	public List<PipeDecoration.GameObjectOffset> decorations = new List<PipeDecoration.GameObjectOffset>();

	// Token: 0x040008DD RID: 2269
	public Vector3 rotate;

	// Token: 0x040008DE RID: 2270
	public Vector3 scale = Vector3.one;

	// Token: 0x040008DF RID: 2271
	public float uniformScale = 1f;

	// Token: 0x020004A0 RID: 1184
	[Serializable]
	public class GameObjectOffset
	{
		// Token: 0x04001C17 RID: 7191
		public GameObject gameObject;

		// Token: 0x04001C18 RID: 7192
		public float offset;
	}
}
