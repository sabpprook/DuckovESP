using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.UI.DialogueBubbles;
using SodaCraft.Localizations;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class BunkerDoorVisual : MonoBehaviour
{
	// Token: 0x06000B58 RID: 2904 RVA: 0x000301CC File Offset: 0x0002E3CC
	private void Awake()
	{
		this.animator.SetBool("InRange", this.inRange);
	}

	// Token: 0x06000B59 RID: 2905 RVA: 0x000301E4 File Offset: 0x0002E3E4
	public void OnEnter()
	{
		if (this.inRange)
		{
			return;
		}
		this.inRange = true;
		this.animator.SetBool("InRange", this.inRange);
		this.PopText(this.welcomeText.ToPlainText(), 0.5f, this.inRange).Forget();
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x00030238 File Offset: 0x0002E438
	public void OnExit()
	{
		if (!this.inRange)
		{
			return;
		}
		this.inRange = false;
		this.animator.SetBool("InRange", this.inRange);
		this.PopText(this.leaveText.ToPlainText(), 0f, this.inRange).Forget();
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003028C File Offset: 0x0002E48C
	private async UniTask PopText(string text, float delay, bool _inRange)
	{
		await UniTask.WaitForSeconds(delay, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (this.inRange == _inRange)
		{
			DialogueBubblesManager.Show(text, this.textBubblePoint, -1f, false, false, -1f, 2f).Forget();
		}
	}

	// Token: 0x040009AA RID: 2474
	[LocalizationKey("Dialogues")]
	public string welcomeText;

	// Token: 0x040009AB RID: 2475
	[LocalizationKey("Dialogues")]
	public string leaveText;

	// Token: 0x040009AC RID: 2476
	public Transform textBubblePoint;

	// Token: 0x040009AD RID: 2477
	public bool inRange = true;

	// Token: 0x040009AE RID: 2478
	public Animator animator;
}
