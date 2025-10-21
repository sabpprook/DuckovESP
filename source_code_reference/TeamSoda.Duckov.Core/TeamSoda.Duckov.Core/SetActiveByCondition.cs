using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Duckov.Quests;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class SetActiveByCondition : MonoBehaviour
{
	// Token: 0x060005B9 RID: 1465 RVA: 0x00019A00 File Offset: 0x00017C00
	private void Update()
	{
		if (!LevelManager.LevelInited && this.requireLevelInited)
		{
			return;
		}
		this.Set();
		if (this.update)
		{
			this.CheckAndLoop().Forget();
		}
		base.enabled = false;
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00019A40 File Offset: 0x00017C40
	public void Set()
	{
		if (this.targetObject)
		{
			bool flag = this.conditions.Satisfied();
			if (this.inverse)
			{
				flag = !flag;
			}
			this.targetObject.SetActive(flag);
		}
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00019A80 File Offset: 0x00017C80
	private async UniTaskVoid CheckAndLoop()
	{
		await UniTask.WaitForSeconds(this.checkTimeSpace, false, PlayerLoopTiming.Update, default(CancellationToken), false);
		if (!(this == null))
		{
			this.Set();
			this.CheckAndLoop().Forget();
		}
	}

	// Token: 0x0400053B RID: 1339
	public GameObject targetObject;

	// Token: 0x0400053C RID: 1340
	public bool inverse;

	// Token: 0x0400053D RID: 1341
	public bool requireLevelInited = true;

	// Token: 0x0400053E RID: 1342
	public List<Condition> conditions;

	// Token: 0x0400053F RID: 1343
	public bool update;

	// Token: 0x04000540 RID: 1344
	private float checkTimeSpace = 1f;
}
