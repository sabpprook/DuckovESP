using System;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class RectStructRefTest : MonoBehaviour
{
	// Token: 0x060004B5 RID: 1205 RVA: 0x00015888 File Offset: 0x00013A88
	private void Test()
	{
		Rect rect = new Rect(Vector2.up, Vector2.one);
		Debug.Log("original: " + rect.size.ToString());
		Rect rect2 = rect;
		rect2.xMax = 20f;
		Debug.Log("Changed");
		Debug.Log("rect: " + rect.size.ToString());
		Debug.Log("rect2: " + rect2.size.ToString());
	}
}
