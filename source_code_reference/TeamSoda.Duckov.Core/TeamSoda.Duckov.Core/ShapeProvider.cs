using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public abstract class ShapeProvider : MonoBehaviour
{
	// Token: 0x06000A26 RID: 2598
	public abstract PipeRenderer.OrientedPoint[] GenerateShape();
}
