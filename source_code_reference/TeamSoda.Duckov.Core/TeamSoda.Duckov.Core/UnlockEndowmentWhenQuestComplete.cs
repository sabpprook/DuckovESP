using System;
using Duckov.Endowment;
using Duckov.Quests;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class UnlockEndowmentWhenQuestComplete : MonoBehaviour
{
	// Token: 0x06000986 RID: 2438 RVA: 0x000296D0 File Offset: 0x000278D0
	private void Awake()
	{
		if (this.quest == null)
		{
			this.quest = base.GetComponent<Quest>();
		}
		if (this.quest != null)
		{
			this.quest.onCompleted += this.OnQuestCompleted;
		}
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0002971C File Offset: 0x0002791C
	private void Start()
	{
		if (this.quest.Complete && !EndowmentManager.GetEndowmentUnlocked(this.endowmentToUnlock))
		{
			EndowmentManager.UnlockEndowment(this.endowmentToUnlock);
		}
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x00029744 File Offset: 0x00027944
	private void OnDestroy()
	{
		if (this.quest != null)
		{
			this.quest.onCompleted -= this.OnQuestCompleted;
		}
	}

	// Token: 0x06000989 RID: 2441 RVA: 0x0002976B File Offset: 0x0002796B
	private void OnQuestCompleted(Quest quest)
	{
		if (!EndowmentManager.GetEndowmentUnlocked(this.endowmentToUnlock))
		{
			EndowmentManager.UnlockEndowment(this.endowmentToUnlock);
		}
	}

	// Token: 0x04000866 RID: 2150
	[SerializeField]
	private Quest quest;

	// Token: 0x04000867 RID: 2151
	[SerializeField]
	private EndowmentIndex endowmentToUnlock;
}
