using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000202 RID: 514
public class ForceUnmaskable : MonoBehaviour
{
	// Token: 0x06000F0F RID: 3855 RVA: 0x0003B88C File Offset: 0x00039A8C
	private void OnEnable()
	{
		MaskableGraphic[] components = base.GetComponents<MaskableGraphic>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].maskable = false;
		}
	}
}
