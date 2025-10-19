using System;
using Cysharp.Threading.Tasks;
using Duckov.UI.DialogueBubbles;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class DialogueBubbleProxy : MonoBehaviour
{
	// Token: 0x06000CB0 RID: 3248 RVA: 0x000352B3 File Offset: 0x000334B3
	public void Pop()
	{
		DialogueBubblesManager.Show(this.textKey.ToPlainText(), base.transform, this.yOffset, false, false, -1f, this.duration).Forget();
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x000352E3 File Offset: 0x000334E3
	public void Pop(string text, float speed = -1f)
	{
		DialogueBubblesManager.Show(text, base.transform, this.yOffset, false, false, speed, 2f).Forget();
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00035304 File Offset: 0x00033504
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawCube(base.transform.position + Vector3.up * this.yOffset, Vector3.one * 0.2f);
	}

	// Token: 0x04000AFE RID: 2814
	[LocalizationKey("Dialogues")]
	public string textKey;

	// Token: 0x04000AFF RID: 2815
	public float yOffset;

	// Token: 0x04000B00 RID: 2816
	public float duration = 2f;
}
