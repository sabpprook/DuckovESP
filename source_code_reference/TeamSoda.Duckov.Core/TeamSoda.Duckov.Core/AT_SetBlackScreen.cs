using System;
using Cysharp.Threading.Tasks;
using Duckov.UI;
using NodeCanvas.Framework;

// Token: 0x020001B0 RID: 432
public class AT_SetBlackScreen : ActionTask
{
	// Token: 0x06000CD4 RID: 3284 RVA: 0x00035786 File Offset: 0x00033986
	protected override void OnExecute()
	{
		if (this.show)
		{
			this.task = BlackScreen.ShowAndReturnTask(null, 0f, 0.5f);
			return;
		}
		this.task = BlackScreen.HideAndReturnTask(null, 0f, 0.5f);
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000357BD File Offset: 0x000339BD
	protected override void OnUpdate()
	{
		if (this.task.Status != UniTaskStatus.Pending)
		{
			base.EndAction();
		}
	}

	// Token: 0x04000B10 RID: 2832
	public bool show;

	// Token: 0x04000B11 RID: 2833
	private UniTask task;
}
